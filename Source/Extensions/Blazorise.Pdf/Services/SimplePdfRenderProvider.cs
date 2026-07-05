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

    private const double LineHeightRatio = 1.2;

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
        List<PdfPageDefinition> pages = document.Pages.Count > 0 ? document.Pages : [CreateDefaultPage( document )];
        PdfFontResources fontResources = AddFontResources( objects, pages );

        foreach ( PdfPageDefinition page in pages )
        {
            PdfPageContent pageContent = BuildPageContent( page, objects, fontResources );
            int contentId = AddObject( objects, CreateStreamObject( pageContent.Content ) );
            int pageId = AddObject( objects, BuildPageObject( page, pagesId, fontResources, contentId, pageContent ) );
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

    private static int AddObject( List<PdfObject> objects, byte[] content )
    {
        objects.Add( new()
        {
            ContentBytes = content,
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

    private static PdfFontResources AddFontResources( List<PdfObject> objects, IReadOnlyList<PdfPageDefinition> pages )
    {
        PdfFontResources fontResources = new();

        foreach ( PdfStandardFontFamily family in CollectFontFamilies( pages ) )
        {
            string[] baseFonts = GetBaseFontNames( family );
            PdfFontResource[] resources = new PdfFontResource[baseFonts.Length];

            for ( int i = 0; i < baseFonts.Length; i++ )
            {
                resources[i] = new()
                {
                    Name = FormattableString.Invariant( $"F{fontResources.All.Count + i + 1}" ),
                    ObjectId = AddObject( objects, FormattableString.Invariant( $"<< /Type /Font /Subtype /Type1 /BaseFont /{baseFonts[i]} >>" ) ),
                };
            }

            fontResources.Add( family, resources );
        }

        return fontResources;
    }

    private static IReadOnlyList<PdfStandardFontFamily> CollectFontFamilies( IReadOnlyList<PdfPageDefinition> pages )
    {
        List<PdfStandardFontFamily> families = [PdfStandardFontFamily.Helvetica];

        foreach ( PdfPageDefinition page in pages )
        {
            foreach ( PdfElementDefinition element in page.Elements )
            {
                CollectFontFamilies( families, element );
            }
        }

        return families;
    }

    private static void CollectFontFamilies( List<PdfStandardFontFamily> families, PdfElementDefinition element )
    {
        if ( element is null )
            return;

        if ( element.Type == PdfElementType.Text )
        {
            PdfStandardFontFamily family = ResolveFontFamily( element.Font?.Family );

            if ( !families.Contains( family ) )
                families.Add( family );
        }

        foreach ( PdfTableRowDefinition row in element.Rows ?? [] )
        {
            foreach ( PdfTableCellDefinition cell in row.Cells ?? [] )
            {
                foreach ( PdfElementDefinition child in cell.Elements ?? [] )
                {
                    CollectFontFamilies( families, child );
                }
            }
        }
    }

    private static PdfStandardFontFamily ResolveFontFamily( string family )
    {
        if ( string.IsNullOrWhiteSpace( family ) )
            return PdfStandardFontFamily.Helvetica;

        if ( ContainsFamilyName( family, "courier", "consolas", "mono" ) )
            return PdfStandardFontFamily.Courier;

        if ( ContainsFamilyName( family, "sans" ) )
            return PdfStandardFontFamily.Helvetica;

        if ( ContainsFamilyName( family, "times", "georgia", "garamond", "serif" ) )
            return PdfStandardFontFamily.Times;

        return PdfStandardFontFamily.Helvetica;
    }

    private static bool ContainsFamilyName( string family, params string[] names )
    {
        foreach ( string name in names )
        {
            if ( family.Contains( name, StringComparison.OrdinalIgnoreCase ) )
                return true;
        }

        return false;
    }

    private static string[] GetBaseFontNames( PdfStandardFontFamily family )
    {
        return family switch
        {
            PdfStandardFontFamily.Times => ["Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic"],
            PdfStandardFontFamily.Courier => ["Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique"],
            _ => ["Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique"],
        };
    }

    private static string BuildPageObject( PdfPageDefinition page, int pagesId, PdfFontResources fontResources, int contentId, PdfPageContent pageContent )
    {
        return FormattableString.Invariant( $"<< /Type /Page /Parent {pagesId} 0 R /MediaBox [ 0 0 {page.Width} {page.Height} ] /Resources {BuildPageResources( fontResources, pageContent )} /Contents {contentId} 0 R >>" );
    }

    private static string BuildPageResources( PdfFontResources fontResources, PdfPageContent pageContent )
    {
        StringBuilder builder = new();
        builder.Append( "<< /Font <<" );

        foreach ( PdfFontResource fontResource in fontResources.All )
        {
            builder.Append( FormattableString.Invariant( $" /{fontResource.Name} {fontResource.ObjectId} 0 R" ) );
        }

        builder.Append( " >>" );

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

        if ( pageContent.Images.Count > 0 )
        {
            builder.Append( " /XObject <<" );

            foreach ( PdfImageResource image in pageContent.Images )
            {
                builder.Append( FormattableString.Invariant( $" /{image.Name} {image.ObjectId} 0 R" ) );
            }

            builder.Append( " >>" );
        }

        builder.Append( " >>" );

        return builder.ToString();
    }

    private static byte[] CreateStreamObject( string content )
    {
        byte[] contentBytes = Encoding.ASCII.GetBytes( content );

        return CreateStreamObject( FormattableString.Invariant( $"<< /Length {contentBytes.Length} >>" ), contentBytes );
    }

    private static byte[] CreateStreamObject( string dictionary, byte[] content )
    {
        using MemoryStream stream = new();

        WriteAscii( stream, $"{dictionary}\nstream\n" );
        stream.Write( content, 0, content.Length );
        WriteAscii( stream, "\nendstream" );

        return stream.ToArray();
    }

    private static PdfPageContent BuildPageContent( PdfPageDefinition page, List<PdfObject> objects, PdfFontResources fontResources )
    {
        PdfPageContentContext context = new( objects, fontResources );

        foreach ( PdfElementDefinition element in page.Elements )
        {
            AppendElement( context, page, element, 0, 0 );
        }

        return new()
        {
            Content = context.Builder.ToString(),
            AlphaStates = context.AlphaStates,
            Images = context.Images,
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
            case PdfElementType.Image:
                AppendImage( context, page, element, x, y );
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
        IReadOnlyList<string> lines = WrapText( element.Text, fontSize, element.Width );
        double lineHeight = ResolveLineHeight( fontSize );
        double textY = ResolveTextY( page, element, y, fontSize, lines.Count, lineHeight );

        AppendColor( context, font.Color, stroke: false );

        for ( int i = 0; i < lines.Count; i++ )
        {
            string line = lines[i];
            double textX = ResolveTextX( element, x, line, fontSize );
            double lineY = textY - ( i * lineHeight );

            context.Builder.AppendLine( FormattableString.Invariant( $"BT /{ResolveFontResourceName( context.FontResources, font )} {fontSize} Tf {textX} {lineY} Td ({EscapeText( line )}) Tj ET" ) );
        }
    }

    private static string ResolveFontResourceName( PdfFontResources fontResources, PdfFontDefinition font )
    {
        PdfFontResource[] variants = fontResources.GetFamily( ResolveFontFamily( font.Family ) );

        if ( font.Bold && font.Italic )
            return variants[3].Name;

        if ( font.Bold )
            return variants[1].Name;

        if ( font.Italic )
            return variants[2].Name;

        return variants[0].Name;
    }

    private static double ResolveTextX( PdfElementDefinition element, double x, string text, double fontSize )
    {
        PdfFontDefinition font = element.Font ?? new();
        double textWidth = EstimateTextWidth( text, fontSize );

        if ( font.Alignment == PdfTextAlignment.Center )
            return x + Math.Max( 0, element.Width - textWidth ) / 2;

        if ( font.Alignment == PdfTextAlignment.End )
            return x + Math.Max( 0, element.Width - textWidth );

        return x;
    }

    private static double ResolveTextY( PdfPageDefinition page, PdfElementDefinition element, double y, double fontSize, int lineCount, double lineHeight )
    {
        PdfFontDefinition font = element.Font ?? new();
        double textBlockHeight = ResolveTextBlockHeight( fontSize, lineCount, lineHeight );
        double offsetY = font.VerticalAlignment switch
        {
            PdfVerticalAlignment.Middle => Math.Max( 0, element.Height - textBlockHeight ) / 2,
            PdfVerticalAlignment.Bottom => Math.Max( 0, element.Height - textBlockHeight ),
            _ => 0,
        };

        return page.Height - y - offsetY - fontSize;
    }

    private static double ResolveLineHeight( double fontSize )
    {
        return Math.Max( fontSize, fontSize * LineHeightRatio );
    }

    private static double ResolveTextBlockHeight( double fontSize, int lineCount, double lineHeight )
    {
        return lineCount <= 0
            ? 0
            : fontSize + ( Math.Max( 0, lineCount - 1 ) * lineHeight );
    }

    private static double EstimateTextWidth( string text, double fontSize )
    {
        return EstimateTextWidth( text?.Length ?? 0, fontSize );
    }

    private static double EstimateTextWidth( int textLength, double fontSize )
    {
        return textLength <= 0
            ? 0
            : textLength * fontSize * AverageGlyphWidthRatio;
    }

    private static IReadOnlyList<string> WrapText( string text, double fontSize, double maxWidth )
    {
        if ( string.IsNullOrEmpty( text ) )
            return [string.Empty];

        if ( maxWidth <= 0 )
            return [text];

        List<string> lines = [];
        string[] paragraphs = text.Replace( "\r\n", "\n", StringComparison.Ordinal ).Replace( '\r', '\n' ).Split( '\n' );

        foreach ( string paragraph in paragraphs )
        {
            AppendWrappedParagraph( lines, paragraph, fontSize, maxWidth );
        }

        return lines;
    }

    private static void AppendWrappedParagraph( List<string> lines, string paragraph, double fontSize, double maxWidth )
    {
        if ( string.IsNullOrWhiteSpace( paragraph ) )
        {
            lines.Add( string.Empty );
            return;
        }

        string currentLine = null;

        foreach ( string word in paragraph.Split( ' ', StringSplitOptions.RemoveEmptyEntries ) )
        {
            if ( currentLine is null )
            {
                AppendWordToLines( lines, ref currentLine, word, fontSize, maxWidth );
                continue;
            }

            if ( EstimateTextWidth( currentLine.Length + word.Length + 1, fontSize ) <= maxWidth )
            {
                currentLine = $"{currentLine} {word}";
                continue;
            }

            lines.Add( currentLine );
            currentLine = null;
            AppendWordToLines( lines, ref currentLine, word, fontSize, maxWidth );
        }

        if ( currentLine is not null )
            lines.Add( currentLine );
    }

    private static void AppendWordToLines( List<string> lines, ref string currentLine, string word, double fontSize, double maxWidth )
    {
        if ( EstimateTextWidth( word, fontSize ) <= maxWidth )
        {
            currentLine = word;
            return;
        }

        foreach ( string segment in SplitWord( word, fontSize, maxWidth ) )
        {
            if ( currentLine is null )
            {
                currentLine = segment;
                continue;
            }

            lines.Add( currentLine );
            currentLine = segment;
        }
    }

    private static IEnumerable<string> SplitWord( string word, double fontSize, double maxWidth )
    {
        StringBuilder segment = new();

        foreach ( char character in word )
        {
            if ( segment.Length > 0 && EstimateTextWidth( segment.Length + 1, fontSize ) > maxWidth )
            {
                yield return segment.ToString();
                segment.Clear();
            }

            segment.Append( character );
        }

        if ( segment.Length > 0 )
            yield return segment.ToString();
    }

    private static void AppendLine( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        PdfBorderDefinition border = element.Border ?? new();
        double startY = page.Height - y;

        AppendStroke( context, border );
        context.Builder.AppendLine( FormattableString.Invariant( $"{x} {startY} m {x + element.Width} {startY} l S" ) );
    }

    private static void AppendRectangle( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        AppendRectangleFill( context, page, element, x, y );
        AppendRectangleStroke( context, page, element, x, y );
    }

    private static void AppendImage( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        if ( element.Width <= 0 || element.Height <= 0 )
            return;

        AppendRectangleFill( context, page, element, x, y );

        if ( !string.IsNullOrWhiteSpace( element.Source ) && TryCreateImageResource( context, element.Source, out PdfImageResource imageResource ) )
        {
            PdfImagePlacement placement = ResolveImagePlacement( element, imageResource.Width, imageResource.Height );
            double elementY = page.Height - y - element.Height;
            double imageX = x + placement.X;
            double imageY = page.Height - y - placement.Y - placement.Height;
            bool clipImage = placement.X < 0 || placement.Y < 0 || placement.X + placement.Width > element.Width || placement.Y + placement.Height > element.Height;
            context.Builder.AppendLine( "q" );

            if ( clipImage )
                context.Builder.AppendLine( FormattableString.Invariant( $"{x} {elementY} {element.Width} {element.Height} re W n" ) );

            context.Builder.AppendLine( FormattableString.Invariant( $"{placement.Width} 0 0 {placement.Height} {imageX} {imageY} cm" ) );
            context.Builder.AppendLine( FormattableString.Invariant( $"/{imageResource.Name} Do" ) );
            context.Builder.AppendLine( "Q" );
        }

        AppendRectangleStroke( context, page, element, x, y );
    }

    private static PdfImagePlacement ResolveImagePlacement( PdfElementDefinition element, double imageWidth, double imageHeight )
    {
        double elementWidth = Math.Max( 0, element.Width );
        double elementHeight = Math.Max( 0, element.Height );

        if ( elementWidth <= 0 || elementHeight <= 0 || imageWidth <= 0 || imageHeight <= 0 )
            return new( 0, 0, elementWidth, elementHeight );

        return ResolveImageFit( element.ImageFit ) switch
        {
            PdfImageFit.Cover => CreateScaledImagePlacement( elementWidth, elementHeight, imageWidth, imageHeight, ( first, second ) => Math.Max( first, second ) ),
            PdfImageFit.None => CreateCenteredImagePlacement( elementWidth, elementHeight, imageWidth, imageHeight ),
            PdfImageFit.Scale => CreateScaleDownImagePlacement( elementWidth, elementHeight, imageWidth, imageHeight ),
            PdfImageFit.Contain => CreateScaledImagePlacement( elementWidth, elementHeight, imageWidth, imageHeight, ( first, second ) => Math.Min( first, second ) ),
            _ => new( 0, 0, elementWidth, elementHeight ),
        };
    }

    private static PdfImageFit ResolveImageFit( PdfImageFit fit )
    {
        return fit == PdfImageFit.Default
            ? PdfImageFit.Fill
            : fit;
    }

    private static PdfImagePlacement CreateScaleDownImagePlacement( double elementWidth, double elementHeight, double imageWidth, double imageHeight )
    {
        return imageWidth <= elementWidth && imageHeight <= elementHeight
            ? CreateCenteredImagePlacement( elementWidth, elementHeight, imageWidth, imageHeight )
            : CreateScaledImagePlacement( elementWidth, elementHeight, imageWidth, imageHeight, ( first, second ) => Math.Min( first, second ) );
    }

    private static PdfImagePlacement CreateScaledImagePlacement( double elementWidth, double elementHeight, double imageWidth, double imageHeight, Func<double, double, double> scaleSelector )
    {
        double scale = scaleSelector( elementWidth / imageWidth, elementHeight / imageHeight );
        double width = imageWidth * scale;
        double height = imageHeight * scale;

        return CreateCenteredImagePlacement( elementWidth, elementHeight, width, height );
    }

    private static PdfImagePlacement CreateCenteredImagePlacement( double elementWidth, double elementHeight, double width, double height )
    {
        return new( ( elementWidth - width ) / 2, ( elementHeight - height ) / 2, width, height );
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
        double width = Math.Max( 0, border?.Width ?? 1 );

        AppendColor( context, border?.Color, stroke: true );
        context.Builder.AppendLine( FormattableString.Invariant( $"{width} w" ) );
        AppendStrokeStyle( context, border?.Style ?? PdfBorderStyle.Solid, width );
    }

    private static void AppendStrokeStyle( PdfPageContentContext context, PdfBorderStyle style, double width )
    {
        double safeWidth = Math.Max( 1, width );

        switch ( style )
        {
            case PdfBorderStyle.Dashed:
                context.Builder.AppendLine( FormattableString.Invariant( $"[{safeWidth * 3} {safeWidth * 2}] 0 d" ) );
                break;
            case PdfBorderStyle.Dotted:
                context.Builder.AppendLine( FormattableString.Invariant( $"[{safeWidth} {safeWidth * 2}] 0 d" ) );
                break;
            default:
                context.Builder.AppendLine( "[] 0 d" );
                break;
        }
    }

    private static void AppendRectangleFill( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        if ( !HasFill( element.Appearance?.BackgroundColor ) )
            return;

        double rectangleY = page.Height - y - element.Height;
        AppendColor( context, element.Appearance.BackgroundColor, stroke: false );
        context.Builder.AppendLine( FormattableString.Invariant( $"{x} {rectangleY} {element.Width} {element.Height} re f" ) );
    }

    private static void AppendRectangleStroke( PdfPageContentContext context, PdfPageDefinition page, PdfElementDefinition element, double x, double y )
    {
        PdfBorderDefinition border = element.Border ?? new();

        if ( !HasStroke( border ) )
            return;

        double rectangleY = page.Height - y - element.Height;
        AppendStroke( context, border );
        context.Builder.AppendLine( FormattableString.Invariant( $"{x} {rectangleY} {element.Width} {element.Height} re S" ) );
    }

    private static bool HasStroke( PdfBorderDefinition border )
    {
        return border?.Width > 0;
    }

    private static bool HasFill( string color )
    {
        return !string.IsNullOrWhiteSpace( color ) && !IsTransparentColor( color );
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

        if ( !TryParseColorComponent( components[0], 255d, out red ) || !TryParseColorComponent( components[1], 255d, out green ) || !TryParseColorComponent( components[2], 255d, out blue ) )
            return false;

        if ( components.Length > 3 && !TryParseColorComponent( components[3], 1d, out alpha ) )
            return false;

        return true;
    }

    private static bool TryParseColorComponent( string value, double divisor, out double component )
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

        component = Math.Clamp( percentage ? parsedValue / 100d : parsedValue / divisor, 0, 1 );

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

    private static bool TryCreateImageResource( PdfPageContentContext context, string source, out PdfImageResource imageResource )
    {
        imageResource = null;

        if ( !PdfImageDataReader.TryRead( source, out PdfImageData imageData ) )
            return false;

        string dictionary = FormattableString.Invariant( $"<< /Type /XObject /Subtype /Image /Width {imageData.Width} /Height {imageData.Height} /ColorSpace {imageData.ColorSpace} /BitsPerComponent {imageData.BitsPerComponent} /Filter {imageData.Filter} /Length {imageData.Data.Length} >>" );
        int objectId = AddObject( context.Objects, CreateStreamObject( dictionary, imageData.Data ) );

        imageResource = new()
        {
            Name = FormattableString.Invariant( $"Im{context.Images.Count + 1}" ),
            ObjectId = objectId,
            Width = imageData.Width,
            Height = imageData.Height,
        };

        context.Images.Add( imageResource );

        return true;
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
        List<long> offsets = [];

        WriteAsciiLine( stream, "%PDF-1.4" );

        for ( int i = 0; i < objects.Count; i++ )
        {
            offsets.Add( stream.Position );
            WriteAsciiLine( stream, FormattableString.Invariant( $"{( i + 1 )} 0 obj" ) );

            if ( objects[i].ContentBytes is not null )
                stream.Write( objects[i].ContentBytes, 0, objects[i].ContentBytes.Length );
            else
                WriteAscii( stream, objects[i].Content );

            WriteAsciiLine( stream, string.Empty );
            WriteAsciiLine( stream, "endobj" );
        }

        long xrefOffset = stream.Position;

        WriteAsciiLine( stream, "xref" );
        WriteAsciiLine( stream, FormattableString.Invariant( $"0 {objects.Count + 1}" ) );
        WriteAsciiLine( stream, "0000000000 65535 f " );

        foreach ( long offset in offsets )
        {
            WriteAsciiLine( stream, FormattableString.Invariant( $"{offset:0000000000} 00000 n " ) );
        }

        WriteAsciiLine( stream, "trailer" );
        WriteAsciiLine( stream, FormattableString.Invariant( $"<< /Size {objects.Count + 1} /Root 1 0 R >>" ) );
        WriteAsciiLine( stream, "startxref" );
        WriteAsciiLine( stream, xrefOffset.ToString( CultureInfo.InvariantCulture ) );
        WriteAscii( stream, "%%EOF" );

        return stream.ToArray();
    }

    private static void WriteAsciiLine( Stream stream, string value )
    {
        WriteAscii( stream, value );
        stream.WriteByte( (byte)'\n' );
    }

    private static void WriteAscii( Stream stream, string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return;

        byte[] bytes = Encoding.ASCII.GetBytes( value );
        stream.Write( bytes, 0, bytes.Length );
    }

    #endregion

    #region Classes

    private sealed class PdfPageContentContext
    {
        #region Members

        private readonly Dictionary<string, string> alphaStateNames = [];

        #endregion

        #region Constructors

        internal PdfPageContentContext( List<PdfObject> objects, PdfFontResources fontResources )
        {
            Objects = objects;
            FontResources = fontResources;
        }

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

        internal List<PdfObject> Objects { get; }

        internal PdfFontResources FontResources { get; }

        internal List<PdfAlphaState> AlphaStates { get; } = [];

        internal List<PdfImageResource> Images { get; } = [];

        #endregion
    }

    private sealed class PdfPageContent
    {
        internal string Content { get; set; }

        internal List<PdfAlphaState> AlphaStates { get; set; } = [];

        internal List<PdfImageResource> Images { get; set; } = [];
    }

    private sealed class PdfAlphaState
    {
        internal string Name { get; set; }

        internal double Alpha { get; set; }

        internal bool Stroke { get; set; }
    }

    private sealed class PdfImageResource
    {
        internal string Name { get; set; }

        internal int ObjectId { get; set; }

        internal int Width { get; set; }

        internal int Height { get; set; }
    }

    private readonly record struct PdfImagePlacement( double X, double Y, double Width, double Height );

    private enum PdfStandardFontFamily
    {
        Helvetica,

        Times,

        Courier,
    }

    private sealed class PdfFontResource
    {
        internal string Name { get; set; }

        internal int ObjectId { get; set; }
    }

    private sealed class PdfFontResources
    {
        private readonly Dictionary<PdfStandardFontFamily, PdfFontResource[]> families = [];

        internal List<PdfFontResource> All { get; } = [];

        internal void Add( PdfStandardFontFamily family, PdfFontResource[] resources )
        {
            families[family] = resources;
            All.AddRange( resources );
        }

        internal PdfFontResource[] GetFamily( PdfStandardFontFamily family )
        {
            return families.TryGetValue( family, out PdfFontResource[] resources )
                ? resources
                : families[PdfStandardFontFamily.Helvetica];
        }
    }

    private readonly record struct PdfColor( double Red, double Green, double Blue, double Alpha );

    private sealed class PdfObject
    {
        internal string Content { get; set; }

        internal byte[] ContentBytes { get; set; }
    }

    #endregion
}