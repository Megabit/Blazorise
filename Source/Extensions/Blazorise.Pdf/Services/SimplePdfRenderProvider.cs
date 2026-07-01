#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Built-in lightweight PDF render provider for basic document generation.
/// </summary>
public sealed class SimplePdfRenderProvider : IPdfRenderProvider
{
    #region Members

    private const double AverageGlyphWidthRatio = 0.5;

    #endregion

    #region Methods

    /// <inheritdoc />
    public Task<PdfRenderResult> RenderAsync( PdfDocumentDefinition document, PdfGenerateOptions options, CancellationToken cancellationToken = default )
    {
        if ( document is null )
            throw new ArgumentNullException( nameof( document ) );

        options ??= new();

        byte[] content = GeneratePdf( document );

        return Task.FromResult( new PdfRenderResult
        {
            Content = content,
            FileName = options.FileName,
        } );
    }

    private static byte[] GeneratePdf( PdfDocumentDefinition document )
    {
        List<PdfObject> objects = [];
        List<int> pageObjectIds = [];
        int catalogId = ReserveObject( objects );
        int pagesId = ReserveObject( objects );
        int fontId = AddObject( objects, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>" );
        List<PdfPageDefinition> pages = document.Pages.Count > 0 ? document.Pages : [CreateDefaultPage( document )];

        foreach ( PdfPageDefinition page in pages )
        {
            PdfPageContent pageContent = BuildPageContent( page );
            int contentId = AddObject( objects, CreateStreamObject( pageContent.Content ) );
            int pageId = AddObject( objects, BuildPageObject( page, pagesId, fontId, contentId, pageContent ) );
            pageObjectIds.Add( pageId );
        }

        SetObject( objects, pagesId, BuildPagesObject( pageObjectIds ) );
        SetObject( objects, catalogId, $"<< /Type /Catalog /Pages {pagesId.ToString( CultureInfo.InvariantCulture )} 0 R >>" );

        return WriteDocument( objects );
    }

    private static PdfPageDefinition CreateDefaultPage( PdfDocumentDefinition document )
    {
        (double width, double height) = PdfPageMetrics.Resolve( document.PageSize, document.Orientation, document.PageWidth, document.PageHeight );

        return new()
        {
            Size = document.PageSize,
            Orientation = document.Orientation,
            Width = width,
            Height = height,
        };
    }

    private static int ReserveObject( List<PdfObject> objects )
    {
        objects.Add( new() );

        return objects.Count;
    }

    private static int AddObject( List<PdfObject> objects, string content )
    {
        objects.Add( new()
        {
            Content = content,
        } );

        return objects.Count;
    }

    private static void SetObject( List<PdfObject> objects, int objectId, string content )
    {
        objects[objectId - 1].Content = content;
    }

    private static string BuildPagesObject( IReadOnlyList<int> pageObjectIds )
    {
        string kids = string.Join( " ", pageObjectIds.Select( pageObjectId => $"{pageObjectId.ToString( CultureInfo.InvariantCulture )} 0 R" ) );

        return $"<< /Type /Pages /Kids [ {kids} ] /Count {pageObjectIds.Count.ToString( CultureInfo.InvariantCulture )} >>";
    }

    private static string BuildPageObject( PdfPageDefinition page, int pagesId, int fontId, int contentId, PdfPageContent pageContent )
    {
        return FormattableString.Invariant( $"<< /Type /Page /Parent {pagesId} 0 R /MediaBox [ 0 0 {page.Width} {page.Height} ] /Resources {BuildPageResources( fontId, pageContent )} /Contents {contentId} 0 R >>" );
    }

    private static string BuildPageResources( int fontId, PdfPageContent pageContent )
    {
        StringBuilder builder = new();
        builder.Append( FormattableString.Invariant( $"<< /Font << /F1 {fontId} 0 R >>" ) );

        if ( pageContent.AlphaStates.Count > 0 )
        {
            builder.Append( " /ExtGState <<" );

            foreach ( PdfAlphaState alphaState in pageContent.AlphaStates )
            {
                string alphaProperty = alphaState.Stroke ? "CA" : "ca";
                builder.Append( FormattableString.Invariant( $" /{alphaState.Name} << /{alphaProperty} {alphaState.Alpha} >>" ) );
            }

            builder.Append( " >>" );
        }

        builder.Append( " >>" );

        return builder.ToString();
    }

    private static string CreateStreamObject( string content )
    {
        int length = Encoding.ASCII.GetByteCount( content );

        return FormattableString.Invariant( $"<< /Length {length} >>\nstream\n{content}\nendstream" );
    }

    private static PdfPageContent BuildPageContent( PdfPageDefinition page )
    {
        PdfPageContentContext context = new();

        foreach ( PdfElementDefinition element in page.Elements )
        {
            AppendElement( context, page, element, 0, 0 );
        }

        return new()
        {
            Content = context.Builder.ToString(),
            AlphaStates = context.AlphaStates,
        };
    }

    private static void AppendElement( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double offsetX, double offsetY )
    {
        if ( element is null )
            return;

        double x = offsetX + element.X;
        double y = offsetY + element.Y;

        switch ( element.Type )
        {
            case PdfElementType.Text:
                AppendText( context, page, element, x, y );
                break;
            case PdfElementType.Line:
                AppendLine( context, page, element, x, y );
                break;
            case PdfElementType.Rectangle:
                AppendRectangle( context, page, element, x, y );
                break;
            case PdfElementType.Table:
                AppendTable( context, page, element, x, y );
                break;
        }
    }

    private static void AppendText( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        AppendRectangle( context, page, element, x, y );

        PdfFontDefinition font = element.Font ?? new();
        double fontSize = Math.Max( 1, font.Size );
        double textX = ResolveTextX( element, x );
        double textY = ResolveTextY( page, element, y, fontSize );

        AppendColor( context, font.Color, stroke: false );
        context.Builder.AppendLine( FormattableString.Invariant( $"BT /F1 {fontSize} Tf {textX} {textY} Td ({EscapeText( element.Text )}) Tj ET" ) );
    }

    private static double ResolveTextX( PdfElementDefinition element, double x )
    {
        PdfFontDefinition font = element.Font ?? new();
        double textWidth = EstimateTextWidth( element.Text, Math.Max( 1, font.Size ) );

        if ( font.Alignment == PdfTextAlignment.Center )
            return x + Math.Max( 0, element.Width - textWidth ) / 2;

        if ( font.Alignment == PdfTextAlignment.End )
            return x + Math.Max( 0, element.Width - textWidth );

        return x;
    }

    private static double ResolveTextY( PdfPageDefinition page, PdfElementDefinition element, double y, double fontSize )
    {
        PdfFontDefinition font = element.Font ?? new();
        double offsetY = font.VerticalAlignment switch
        {
            PdfVerticalAlignment.Middle => Math.Max( 0, element.Height - fontSize ) / 2,
            PdfVerticalAlignment.Bottom => Math.Max( 0, element.Height - fontSize ),
            _ => 0,
        };

        return page.Height - y - offsetY - fontSize;
    }

    private static double EstimateTextWidth( string text, double fontSize )
    {
        return string.IsNullOrEmpty( text )
            ? 0
            : text.Length * fontSize * AverageGlyphWidthRatio;
    }

    private static void AppendLine( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        PdfBorderDefinition border = element.Border ?? new();
        double startY = page.Height - y;
        double endY = page.Height - y - element.Height;

        AppendStroke( context, border );
        context.Builder.AppendLine( FormattableString.Invariant( $"{x} {startY} m {x + element.Width} {endY} l S" ) );
    }

    private static void AppendRectangle( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        PdfBorderDefinition border = element.Border ?? new();
        double rectangleY = page.Height - y - element.Height;
        bool hasFill = HasFill( element.Appearance?.BackgroundColor );
        bool hasStroke = HasStroke( border );

        if ( !hasFill && !hasStroke )
            return;

        if ( hasFill )
            AppendColor( context, element.Appearance.BackgroundColor, stroke: false );

        if ( hasStroke )
            AppendStroke( context, border );

        context.Builder.AppendLine( FormattableString.Invariant( $"{x} {rectangleY} {element.Width} {element.Height} re {ResolveRectanglePaintOperator( hasFill, hasStroke )}" ) );
    }

    private static void AppendTable( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        if ( HasFill( element.Appearance?.BackgroundColor ) )
            AppendRectangle( context, page, element, x, y );

        double currentY = y;

        foreach ( PdfTableRowDefinition row in element.Rows )
        {
            double currentX = x;
            double rowHeight = Math.Max( 1, row.Height );

            foreach ( PdfTableCellDefinition cell in row.Cells )
            {
                double cellWidth = Math.Max( 1, cell.Width );
                AppendRectangle( context, page, new()
                {
                    Type = PdfElementType.Rectangle,
                    Width = cellWidth,
                    Height = rowHeight,
                    Border = element.Border,
                    Appearance = new(),
                }, currentX, currentY );

                foreach ( PdfElementDefinition child in cell.Elements )
                {
                    AppendElement( context, page, child, currentX, currentY );
                }

                currentX += cellWidth;
            }

            currentY += rowHeight;
        }
    }

    private static void AppendStroke( PdfPageContentContext context, PdfBorderDefinition border )
    {
        AppendColor( context, border?.Color, stroke: true );
        context.Builder.AppendLine( FormattableString.Invariant( $"{Math.Max( 0, border?.Width ?? 1 )} w" ) );
    }

    private static bool HasStroke( PdfBorderDefinition border )
    {
        return border?.Width > 0;
    }

    private static bool HasFill( string color )
    {
        return !string.IsNullOrWhiteSpace( color ) && !IsTransparentColor( color );
    }

    private static string ResolveRectanglePaintOperator( bool hasFill, bool hasStroke )
    {
        if ( hasFill && hasStroke )
            return "B";

        return hasFill ? "f" : "S";
    }

    private static void AppendColor( PdfPageContentContext context, string color, bool stroke )
    {
        PdfColor pdfColor = ParseColor( color );
        context.Builder.AppendLine( FormattableString.Invariant( $"{pdfColor.Red} {pdfColor.Green} {pdfColor.Blue} {( stroke ? "RG" : "rg" )}" ) );
        context.Builder.AppendLine( FormattableString.Invariant( $"/{context.GetAlphaStateName( pdfColor.Alpha, stroke )} gs" ) );
    }

    private static PdfColor ParseColor( string color )
    {
        if ( string.IsNullOrWhiteSpace( color ) )
            return new( 0, 0, 0, 1 );

        string value = ResolveKnownColor( color.Trim() ).TrimStart( '#' );

        if ( TryParseRgbFunction( value, out double redComponent, out double greenComponent, out double blueComponent, out double alphaComponent ) )
            return new( redComponent, greenComponent, blueComponent, alphaComponent );

        if ( value.Length == 3 )
            value = $"{value[0]}{value[0]}{value[1]}{value[1]}{value[2]}{value[2]}";
        else if ( value.Length == 4 )
            value = $"{value[0]}{value[0]}{value[1]}{value[1]}{value[2]}{value[2]}{value[3]}{value[3]}";

        if ( value.Length != 6 && value.Length != 8 )
            return new( 0, 0, 0, 1 );

        if ( !TryParseHexComponent( value, 0, out int red ) || !TryParseHexComponent( value, 2, out int green ) || !TryParseHexComponent( value, 4, out int blue ) )
            return new( 0, 0, 0, 1 );

        double alpha = 1;

        if ( value.Length == 8 )
        {
            if ( !TryParseHexComponent( value, 6, out int alphaValue ) )
                return new( 0, 0, 0, 1 );

            alpha = alphaValue / 255d;
        }

        return new( red / 255d, green / 255d, blue / 255d, alpha );
    }

    private static bool TryParseHexComponent( string value, int startIndex, out int component )
    {
        return int.TryParse( value.Substring( startIndex, 2 ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out component );
    }

    private static bool TryParseRgbFunction( string value, out double red, out double green, out double blue, out double alpha )
    {
        red = 0;
        green = 0;
        blue = 0;
        alpha = 1;

        if ( !value.StartsWith( "rgb", StringComparison.OrdinalIgnoreCase ) )
            return false;

        int startIndex = value.IndexOf( '(' );
        int endIndex = value.LastIndexOf( ')' );

        if ( startIndex < 0 || endIndex <= startIndex )
            return false;

        string[] components = value.Substring( startIndex + 1, endIndex - startIndex - 1 ).Split( ',', StringSplitOptions.TrimEntries );

        if ( components.Length < 3 )
            return false;

        if ( !TryParseColorComponent( components[0], out red ) || !TryParseColorComponent( components[1], out green ) || !TryParseColorComponent( components[2], out blue ) )
            return false;

        if ( components.Length > 3 && !TryParseAlphaComponent( components[3], out alpha ) )
            return false;

        return true;
    }

    private static bool TryParseColorComponent( string value, out double component )
    {
        component = 0;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        string trimmedValue = value.Trim();
        bool percentage = trimmedValue.EndsWith( "%", StringComparison.Ordinal );

        if ( percentage )
            trimmedValue = trimmedValue[..^1];

        if ( !double.TryParse( trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue ) )
            return false;

        component = Math.Clamp( percentage ? parsedValue / 100d : parsedValue / 255d, 0, 1 );

        return true;
    }

    private static bool TryParseAlphaComponent( string value, out double alpha )
    {
        alpha = 1;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        string trimmedValue = value.Trim();
        bool percentage = trimmedValue.EndsWith( "%", StringComparison.Ordinal );

        if ( percentage )
            trimmedValue = trimmedValue[..^1];

        if ( !double.TryParse( trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue ) )
            return false;

        alpha = Math.Clamp( percentage ? parsedValue / 100d : parsedValue, 0, 1 );

        return true;
    }

    private static string ResolveKnownColor( string color )
    {
        string value = NormalizeColorName( color );

        return value switch
        {
            "primary" => "#0d6efd",
            "secondary" => "#6c757d",
            "success" => "#198754",
            "danger" => "#dc3545",
            "warning" => "#ffc107",
            "info" => "#0dcaf0",
            "light" => "#f8f9fa",
            "dark" => "#212529",
            "body" => "#212529",
            "muted" => "#6c757d",
            "white" => "#ffffff",
            "black" => "#000000",
            "black-50" => "rgba(0, 0, 0, .5)",
            "white-50" => "rgba(255, 255, 255, .5)",
            "transparent" => "#00000000",
            _ => color,
        };
    }

    private static string NormalizeColorName( string color )
    {
        string value = color.Trim().ToLowerInvariant();

        if ( value.StartsWith( "text-", StringComparison.Ordinal ) )
            return value[5..];

        if ( value.StartsWith( "bg-", StringComparison.Ordinal ) )
            return value[3..];

        if ( value.StartsWith( "background-", StringComparison.Ordinal ) )
            return value[11..];

        return value;
    }

    private static bool IsTransparentColor( string color )
    {
        return NormalizeColorName( color ) == "transparent";
    }

    private static string EscapeText( string text )
    {
        if ( string.IsNullOrEmpty( text ) )
            return string.Empty;

        StringBuilder builder = new();

        foreach ( char character in text )
        {
            builder.Append( character switch
            {
                '(' => "\\(",
                ')' => "\\)",
                '\\' => "\\\\",
                >= (char)32 and <= (char)126 => character,
                _ => '?',
            } );
        }

        return builder.ToString();
    }

    private static byte[] WriteDocument( IReadOnlyList<PdfObject> objects )
    {
        using MemoryStream stream = new();
        using StreamWriter writer = new( stream, Encoding.ASCII, 1024, leaveOpen: true );
        List<long> offsets = [];

        writer.WriteLine( "%PDF-1.4" );

        for ( int i = 0; i < objects.Count; i++ )
        {
            writer.Flush();
            offsets.Add( stream.Position );
            writer.WriteLine( FormattableString.Invariant( $"{( i + 1 )} 0 obj" ) );
            writer.WriteLine( objects[i].Content );
            writer.WriteLine( "endobj" );
        }

        writer.Flush();
        long xrefOffset = stream.Position;

        writer.WriteLine( "xref" );
        writer.WriteLine( FormattableString.Invariant( $"0 {objects.Count + 1}" ) );
        writer.WriteLine( "0000000000 65535 f " );

        foreach ( long offset in offsets )
        {
            writer.WriteLine( FormattableString.Invariant( $"{offset:0000000000} 00000 n " ) );
        }

        writer.WriteLine( "trailer" );
        writer.WriteLine( FormattableString.Invariant( $"<< /Size {objects.Count + 1} /Root 1 0 R >>" ) );
        writer.WriteLine( "startxref" );
        writer.WriteLine( xrefOffset.ToString( CultureInfo.InvariantCulture ) );
        writer.Write( "%%EOF" );
        writer.Flush();

        return stream.ToArray();
    }

    #endregion

    #region Classes

    private sealed class PdfPageContentContext
    {
        #region Members

        private readonly Dictionary<string, string> alphaStateNames = [];

        #endregion

        #region Methods

        internal string GetAlphaStateName( double alpha, bool stroke )
        {
            double normalizedAlpha = Math.Clamp( Math.Round( alpha, 3 ), 0, 1 );
            string key = FormattableString.Invariant( $"{( stroke ? "S" : "F" )}:{normalizedAlpha}" );

            if ( !alphaStateNames.TryGetValue( key, out string alphaStateName ) )
            {
                alphaStateName = FormattableString.Invariant( $"GS{AlphaStates.Count + 1}" );
                alphaStateNames.Add( key, alphaStateName );
                AlphaStates.Add( new()
                {
                    Name = alphaStateName,
                    Alpha = normalizedAlpha,
                    Stroke = stroke,
                } );
            }

            return alphaStateName;
        }

        #endregion

        #region Properties

        internal StringBuilder Builder { get; } = new();

        internal List<PdfAlphaState> AlphaStates { get; } = [];

        #endregion
    }

    private sealed class PdfPageContent
    {
        internal string Content { get; set; }

        internal List<PdfAlphaState> AlphaStates { get; set; } = [];
    }

    private sealed class PdfAlphaState
    {
        internal string Name { get; set; }

        internal double Alpha { get; set; }

        internal bool Stroke { get; set; }
    }

    private readonly record struct PdfColor( double Red, double Green, double Blue, double Alpha );

    private sealed class PdfObject
    {
        internal string Content { get; set; }
    }

    #endregion
}