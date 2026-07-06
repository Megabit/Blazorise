#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Pdf.Internal;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Built-in lightweight PDF render provider for basic document generation.
/// </summary>
public sealed class SimplePdfRenderProvider : IPdfRenderProvider
{
    #region Members

    private const double LineHeightRatio = 1.2;

    private readonly IFontProvider fontProvider;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the simple PDF render provider.
    /// </summary>
    /// <param name="fontProvider">Blazorise font provider.</param>
    public SimplePdfRenderProvider( IFontProvider fontProvider = null )
    {
        this.fontProvider = fontProvider;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public Task<PdfRenderResult> RenderAsync( PdfDocumentDefinition document, PdfGenerateOptions options, CancellationToken cancellationToken = default )
    {
        if ( document is null )
            throw new ArgumentNullException( nameof( document ) );

        options ??= new();

        byte[] content = GeneratePdf( document, fontProvider );

        return Task.FromResult( new PdfRenderResult
        {
            Content = content,
            FileName = options.FileName,
        } );
    }

    private static byte[] GeneratePdf( PdfDocumentDefinition document, IFontProvider fontProvider )
    {
        List<PdfObject> objects = [];
        List<int> pageObjectIds = [];
        int catalogId = ReserveObject( objects );
        int pagesId = ReserveObject( objects );
        List<PdfPageDefinition> pages = document.Pages.Count > 0 ? document.Pages : [CreateDefaultPage( document )];
        PdfFontResources fontResources = AddFontResources( objects, pages, fontProvider );

        foreach ( PdfPageDefinition page in pages )
        {
            PdfPageContent pageContent = BuildPageContent( page, objects, fontResources, fontProvider );
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

    private static PdfFontResources AddFontResources( List<PdfObject> objects, IReadOnlyList<PdfPageDefinition> pages, IFontProvider fontProvider )
    {
        PdfFontResources fontResources = new();
        bool centralEuropeanFallbackRequired = RequiresType1CentralEuropeanFallback( pages );

        foreach ( string family in CollectFontFamilies( pages, fontProvider ) )
        {
            FontFamily customFont = ResolveCustomFontFamily( family, fontProvider );
            string[] baseFonts = GetBaseFontNames( customFont is null ? family : Fonts.Helvetica );
            PdfFontResource[] resources = new PdfFontResource[baseFonts.Length];

            for ( int i = 0; i < baseFonts.Length; i++ )
            {
                PdfEmbeddedFont embeddedFont = customFont is not null
                    ? TryCreateEmbeddedFontResource( objects, customFont, i, fontResources.All.Count + i + 1 )
                    : null;

                resources[i] = new()
                {
                    Name = FormattableString.Invariant( $"F{fontResources.All.Count + i + 1}" ),
                    ObjectId = embeddedFont?.ObjectId ?? AddObject( objects, BuildType1FontObject( baseFonts[i], centralEuropeanFallbackRequired ) ),
                    EmbeddedFont = embeddedFont,
                    Metrics = embeddedFont is not null ? embeddedFont.Metrics : new PdfStandardFontMetrics( baseFonts[i] ),
                };
            }

            fontResources.Add( family, resources );
        }

        return fontResources;
    }

    private static IReadOnlyList<string> CollectFontFamilies( IReadOnlyList<PdfPageDefinition> pages, IFontProvider fontProvider )
    {
        List<string> families = [Fonts.Helvetica];

        foreach ( PdfPageDefinition page in pages )
        {
            foreach ( PdfElementDefinition element in page.Elements )
            {
                CollectFontFamilies( families, element, fontProvider );
            }
        }

        return families;
    }

    private static void CollectFontFamilies( List<string> families, PdfElementDefinition element, IFontProvider fontProvider )
    {
        if ( element is null )
            return;

        if ( element.Type == PdfElementType.Text )
        {
            string family = ResolveFontFamilyKey( element.Font?.Family, fontProvider );

            if ( !families.Contains( family, StringComparer.OrdinalIgnoreCase ) )
                families.Add( family );
        }

        foreach ( PdfTableRowDefinition row in element.Rows ?? [] )
        {
            foreach ( PdfTableCellDefinition cell in row.Cells ?? [] )
            {
                foreach ( PdfElementDefinition child in cell.Elements ?? [] )
                {
                    CollectFontFamilies( families, child, fontProvider );
                }
            }
        }
    }

    private static string ResolveFontFamilyKey( string family, IFontProvider fontProvider )
    {
        FontFamily registeredFont = ResolveCustomFontFamily( family, fontProvider );

        if ( registeredFont is not null )
            return registeredFont.Name;

        return ResolveStandardFontFamilyName( family );
    }

    private static string ResolveStandardFontFamilyName( string family )
    {
        if ( string.IsNullOrWhiteSpace( family ) )
            return Fonts.Helvetica;

        if ( ContainsFamilyName( family, "courier", "consolas", "mono" ) )
            return Fonts.Courier;

        if ( ContainsFamilyName( family, "sans" ) )
            return Fonts.Helvetica;

        if ( ContainsFamilyName( family, "times", "georgia", "garamond", "serif" ) )
            return Fonts.Times;

        return Fonts.Helvetica;
    }

    private static FontFamily ResolveCustomFontFamily( string family, IFontProvider fontProvider )
    {
        FontFamily registeredFont = fontProvider?.Resolve( family );

        return registeredFont is not null && HasFontSource( registeredFont ) && !IsStandardFontFamily( registeredFont.Name )
            ? registeredFont
            : null;
    }

    private static bool HasFontSource( FontFamily font )
    {
        return font?.Regular is not null || font?.Bold is not null || font?.Italic is not null || font?.BoldItalic is not null;
    }

    private static bool IsStandardFontFamily( string family )
    {
        return IsFontFamily( family, Fonts.Helvetica )
               || IsFontFamily( family, Fonts.Times )
               || IsFontFamily( family, Fonts.Courier );
    }

    private static bool IsFontFamily( string family, string expectedFamily )
    {
        return string.Equals( family, expectedFamily, StringComparison.OrdinalIgnoreCase );
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

    private static string[] GetBaseFontNames( string family )
    {
        if ( IsFontFamily( family, Fonts.Times ) )
            return ["Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic"];

        if ( IsFontFamily( family, Fonts.Courier ) )
            return ["Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique"];

        return ["Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique"];
    }

    private static string BuildType1FontObject( string baseFontName, bool centralEuropeanFallbackRequired )
    {
        if ( !centralEuropeanFallbackRequired )
            return FormattableString.Invariant( $"<< /Type /Font /Subtype /Type1 /BaseFont /{baseFontName} /Encoding /WinAnsiEncoding >>" );

        return FormattableString.Invariant( $"<< /Type /Font /Subtype /Type1 /BaseFont /{baseFontName} /Encoding << /Type /Encoding /BaseEncoding /WinAnsiEncoding /Differences [ 129 /ccaron 131 /Dcroat 141 /cacute 143 /dcroat 144 /Ccaron 157 /Cacute ] >> >>" );
    }

    private static bool RequiresType1CentralEuropeanFallback( IReadOnlyList<PdfPageDefinition> pages )
    {
        foreach ( PdfPageDefinition page in pages )
        {
            foreach ( PdfElementDefinition element in page.Elements )
            {
                if ( RequiresType1CentralEuropeanFallback( element ) )
                    return true;
            }
        }

        return false;
    }

    private static bool RequiresType1CentralEuropeanFallback( PdfElementDefinition element )
    {
        if ( element is null )
            return false;

        if ( element.Type == PdfElementType.Text && RequiresType1CentralEuropeanFallback( element.Text ) )
            return true;

        foreach ( PdfTableRowDefinition row in element.Rows ?? [] )
        {
            foreach ( PdfTableCellDefinition cell in row.Cells ?? [] )
            {
                foreach ( PdfElementDefinition child in cell.Elements ?? [] )
                {
                    if ( RequiresType1CentralEuropeanFallback( child ) )
                        return true;
                }
            }
        }

        return false;
    }

    private static bool RequiresType1CentralEuropeanFallback( string text )
    {
        if ( string.IsNullOrEmpty( text ) )
            return false;

        foreach ( char character in text )
        {
            if ( IsType1CentralEuropeanFallbackCharacter( character ) )
                return true;
        }

        return false;
    }

    private static bool IsType1CentralEuropeanFallbackCharacter( char character )
    {
        return character is '\u010D' or '\u010C' or '\u0107' or '\u0106' or '\u0111' or '\u0110';
    }

    private static PdfEmbeddedFont TryCreateEmbeddedFontResource( List<PdfObject> objects, FontFamily family, int variantIndex, int resourceIndex )
    {
        if ( !PdfEmbeddedFont.TryCreate( family, variantIndex, resourceIndex, out PdfEmbeddedFont embeddedFont ) )
            return null;

        return AddEmbeddedFontResource( objects, embeddedFont );
    }

    private static PdfEmbeddedFont AddEmbeddedFontResource( List<PdfObject> objects, PdfEmbeddedFont embeddedFont )
    {
        int fontFileId = AddObject( objects, CreateStreamObject( FormattableString.Invariant( $"<< /Length {embeddedFont.FontBytes.Length} /Length1 {embeddedFont.FontBytes.Length} >>" ), embeddedFont.FontBytes ) );
        int fontDescriptorId = AddObject( objects, BuildEmbeddedFontDescriptorObject( embeddedFont, fontFileId ) );
        int toUnicodeId = AddObject( objects, CreateStreamObject( BuildToUnicodeMap( embeddedFont ) ) );
        int cidFontId = AddObject( objects, BuildEmbeddedCidFontObject( embeddedFont, fontDescriptorId ) );
        embeddedFont.ObjectId = AddObject( objects, BuildEmbeddedType0FontObject( embeddedFont, cidFontId, toUnicodeId ) );

        return embeddedFont;
    }

    private static string BuildEmbeddedType0FontObject( PdfEmbeddedFont embeddedFont, int cidFontId, int toUnicodeId )
    {
        return FormattableString.Invariant( $"<< /Type /Font /Subtype /Type0 /BaseFont /{embeddedFont.FontName} /Encoding /Identity-H /DescendantFonts [ {cidFontId} 0 R ] /ToUnicode {toUnicodeId} 0 R >>" );
    }

    private static string BuildEmbeddedCidFontObject( PdfEmbeddedFont embeddedFont, int fontDescriptorId )
    {
        return FormattableString.Invariant( $"<< /Type /Font /Subtype /CIDFontType2 /BaseFont /{embeddedFont.FontName} /CIDSystemInfo << /Registry (Adobe) /Ordering (Identity) /Supplement 0 >> /FontDescriptor {fontDescriptorId} 0 R /CIDToGIDMap /Identity /DW {PdfTrueTypeFontMetrics.DefaultGlyphWidth} /W {embeddedFont.BuildWidthArray()} >>" );
    }

    private static string BuildEmbeddedFontDescriptorObject( PdfEmbeddedFont embeddedFont, int fontFileId )
    {
        return FormattableString.Invariant( $"<< /Type /FontDescriptor /FontName /{embeddedFont.FontName} /Flags {embeddedFont.Flags} /FontBBox [ {embeddedFont.MinX} {embeddedFont.MinY} {embeddedFont.MaxX} {embeddedFont.MaxY} ] /ItalicAngle {embeddedFont.ItalicAngle} /Ascent {embeddedFont.Ascent} /Descent {embeddedFont.Descent} /CapHeight {embeddedFont.CapHeight} /StemV 80 /FontFile2 {fontFileId} 0 R >>" );
    }

    private static string BuildToUnicodeMap( PdfEmbeddedFont embeddedFont )
    {
        StringBuilder builder = new();
        builder.AppendLine( "/CIDInit /ProcSet findresource begin" );
        builder.AppendLine( "12 dict begin" );
        builder.AppendLine( "begincmap" );
        builder.AppendLine( "/CIDSystemInfo << /Registry (Adobe) /Ordering (UCS) /Supplement 0 >> def" );
        builder.AppendLine( "/CMapName /Adobe-Identity-UCS def" );
        builder.AppendLine( "/CMapType 2 def" );
        builder.AppendLine( "1 begincodespacerange" );
        builder.AppendLine( "<0000> <FFFF>" );
        builder.AppendLine( "endcodespacerange" );

        List<KeyValuePair<int, int>> mappings = embeddedFont.CreateGlyphUnicodeMappings();

        for ( int i = 0; i < mappings.Count; i += 100 )
        {
            int count = Math.Min( 100, mappings.Count - i );
            builder.AppendLine( FormattableString.Invariant( $"{count} beginbfchar" ) );

            for ( int j = 0; j < count; j++ )
            {
                KeyValuePair<int, int> mapping = mappings[i + j];
                builder.AppendLine( FormattableString.Invariant( $"<{mapping.Key:X4}> <{ToUnicodeHex( mapping.Value )}>" ) );
            }

            builder.AppendLine( "endbfchar" );
        }

        builder.AppendLine( "endcmap" );
        builder.AppendLine( "CMapName currentdict /CMap defineresource pop" );
        builder.AppendLine( "end" );
        builder.AppendLine( "end" );

        return builder.ToString();
    }

    private static string ToUnicodeHex( int codePoint )
    {
        if ( codePoint <= 0xFFFF )
            return codePoint.ToString( "X4", CultureInfo.InvariantCulture );

        string text = char.ConvertFromUtf32( codePoint );
        StringBuilder builder = new();

        foreach ( char character in text )
        {
            builder.Append( ( (int)character ).ToString( "X4", CultureInfo.InvariantCulture ) );
        }

        return builder.ToString();
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

    private static PdfPageContent BuildPageContent( PdfPageDefinition page, List<PdfObject> objects, PdfFontResources fontResources, IFontProvider fontProvider )
    {
        PdfPageContentContext context = new( objects, fontResources, fontProvider );

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
        PdfFontResource fontResource = ResolveFontResource( context.FontResources, context.FontProvider, font );
        IReadOnlyList<string> lines = WrapText( element.Text, fontResource, fontSize, element.Width );
        double lineHeight = ResolveLineHeight( fontSize );
        double textY = ResolveTextY( page, element, y, fontSize, lines.Count, lineHeight );

        AppendColor( context, font.Color, stroke: false );

        for ( int i = 0; i < lines.Count; i++ )
        {
            string line = lines[i];
            double textX = ResolveTextX( element, x, line, fontResource, fontSize );
            double lineY = textY - ( i * lineHeight );

            string textOperand = fontResource.EmbeddedFont is null
                ? $"({EscapeText( line )})"
                : EncodeEmbeddedText( line, fontResource.EmbeddedFont );

            context.Builder.AppendLine( FormattableString.Invariant( $"BT /{fontResource.Name} {fontSize} Tf {textX} {lineY} Td {textOperand} Tj ET" ) );
        }
    }

    private static PdfFontResource ResolveFontResource( PdfFontResources fontResources, IFontProvider fontProvider, PdfFontDefinition font )
    {
        PdfFontResource[] variants = fontResources.GetFamily( ResolveFontFamilyKey( font.Family, fontProvider ) );

        if ( font.Bold && font.Italic )
            return variants[3];

        if ( font.Bold )
            return variants[1];

        if ( font.Italic )
            return variants[2];

        return variants[0];
    }

    private static double ResolveTextX( PdfElementDefinition element, double x, string text, PdfFontResource fontResource, double fontSize )
    {
        PdfFontDefinition font = element.Font ?? new();
        double textWidth = MeasureTextWidth( text, fontResource, fontSize );

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

    private static double MeasureTextWidth( string text, PdfFontResource fontResource, double fontSize )
    {
        return fontResource?.MeasureTextWidth( text, fontSize ) ?? 0;
    }

    private static IReadOnlyList<string> WrapText( string text, PdfFontResource fontResource, double fontSize, double maxWidth )
    {
        if ( string.IsNullOrEmpty( text ) )
            return [string.Empty];

        if ( maxWidth <= 0 )
            return [text];

        List<string> lines = [];
        string[] paragraphs = text.Replace( "\r\n", "\n", StringComparison.Ordinal ).Replace( '\r', '\n' ).Split( '\n' );

        foreach ( string paragraph in paragraphs )
        {
            AppendWrappedParagraph( lines, paragraph, fontResource, fontSize, maxWidth );
        }

        return lines;
    }

    private static void AppendWrappedParagraph( List<string> lines, string paragraph, PdfFontResource fontResource, double fontSize, double maxWidth )
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
                AppendWordToLines( lines, ref currentLine, word, fontResource, fontSize, maxWidth );
                continue;
            }

            if ( MeasureTextWidth( $"{currentLine} {word}", fontResource, fontSize ) <= maxWidth )
            {
                currentLine = $"{currentLine} {word}";
                continue;
            }

            lines.Add( currentLine );
            currentLine = null;
            AppendWordToLines( lines, ref currentLine, word, fontResource, fontSize, maxWidth );
        }

        if ( currentLine is not null )
            lines.Add( currentLine );
    }

    private static void AppendWordToLines( List<string> lines, ref string currentLine, string word, PdfFontResource fontResource, double fontSize, double maxWidth )
    {
        if ( MeasureTextWidth( word, fontResource, fontSize ) <= maxWidth )
        {
            currentLine = word;
            return;
        }

        foreach ( string segment in SplitWord( word, fontResource, fontSize, maxWidth ) )
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

    private static IEnumerable<string> SplitWord( string word, PdfFontResource fontResource, double fontSize, double maxWidth )
    {
        StringBuilder segment = new();

        foreach ( char character in word )
        {
            if ( segment.Length > 0 && MeasureTextWidth( $"{segment}{character}", fontResource, fontSize ) > maxWidth )
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
            if ( TryGetType1TextByte( character, out byte value ) )
            {
                AppendPdfStringByte( builder, value );
            }
            else
            {
                builder.Append( '?' );
            }
        }

        return builder.ToString();
    }

    private static string EncodeEmbeddedText( string text, PdfEmbeddedFont embeddedFont )
    {
        if ( string.IsNullOrEmpty( text ) )
            return "<>";

        StringBuilder builder = new();
        builder.Append( '<' );

        foreach ( int codePoint in EnumerateCodePoints( text ) )
        {
            int glyphId = embeddedFont.GetGlyphId( codePoint );
            builder.Append( glyphId.ToString( "X4", CultureInfo.InvariantCulture ) );
        }

        builder.Append( '>' );

        return builder.ToString();
    }

    private static IEnumerable<int> EnumerateCodePoints( string text )
    {
        for ( int i = 0; i < text.Length; i++ )
        {
            char character = text[i];

            if ( char.IsHighSurrogate( character ) && i + 1 < text.Length && char.IsLowSurrogate( text[i + 1] ) )
            {
                yield return char.ConvertToUtf32( character, text[++i] );
                continue;
            }

            yield return character;
        }
    }

    private static void AppendPdfStringByte( StringBuilder builder, byte value )
    {
        switch ( value )
        {
            case (byte)'(':
                builder.Append( "\\(" );
                break;
            case (byte)')':
                builder.Append( "\\)" );
                break;
            case (byte)'\\':
                builder.Append( "\\\\" );
                break;
            case >= 32 and <= 126:
                builder.Append( (char)value );
                break;
            default:
                builder.Append( '\\' );
                builder.Append( Convert.ToString( value, 8 ).PadLeft( 3, '0' ) );
                break;
        }
    }

    private static bool TryGetType1TextByte( char character, out byte value )
    {
        if ( character <= 0x7F )
        {
            value = (byte)character;
            return true;
        }

        if ( character >= 0xA0 && character <= 0xFF )
        {
            value = (byte)character;
            return true;
        }

        value = character switch
        {
            '\u20AC' => 0x80,
            '\u201A' => 0x82,
            '\u0192' => 0x83,
            '\u201E' => 0x84,
            '\u2026' => 0x85,
            '\u2020' => 0x86,
            '\u2021' => 0x87,
            '\u02C6' => 0x88,
            '\u2030' => 0x89,
            '\u0160' => 0x8A,
            '\u2039' => 0x8B,
            '\u0152' => 0x8C,
            '\u017D' => 0x8E,
            '\u010D' => 0x81,
            '\u0110' => 0x83,
            '\u0107' => 0x8D,
            '\u0111' => 0x8F,
            '\u010C' => 0x90,
            '\u2018' => 0x91,
            '\u2019' => 0x92,
            '\u201C' => 0x93,
            '\u201D' => 0x94,
            '\u2022' => 0x95,
            '\u2013' => 0x96,
            '\u2014' => 0x97,
            '\u02DC' => 0x98,
            '\u2122' => 0x99,
            '\u0161' => 0x9A,
            '\u203A' => 0x9B,
            '\u0153' => 0x9C,
            '\u0106' => 0x9D,
            '\u017E' => 0x9E,
            '\u0178' => 0x9F,
            _ => 0,
        };

        return value != 0;
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

        internal PdfPageContentContext( List<PdfObject> objects, PdfFontResources fontResources, IFontProvider fontProvider )
        {
            Objects = objects;
            FontResources = fontResources;
            FontProvider = fontProvider;
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

        internal IFontProvider FontProvider { get; }

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

    private sealed class PdfEmbeddedFont
    {
        private readonly Dictionary<int, int> glyphsByCodePoint;
        private readonly Dictionary<int, int> glyphWidths;

        private PdfEmbeddedFont( string fontName, byte[] fontBytes, Dictionary<int, int> glyphsByCodePoint, Dictionary<int, int> glyphWidths, int minX, int minY, int maxX, int maxY, int ascent, int descent, int flags, int italicAngle )
        {
            FontName = fontName;
            FontBytes = fontBytes;
            this.glyphsByCodePoint = glyphsByCodePoint;
            this.glyphWidths = glyphWidths;
            Metrics = new( glyphsByCodePoint, glyphWidths );
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            Ascent = ascent;
            Descent = descent;
            CapHeight = ascent;
            Flags = flags;
            ItalicAngle = italicAngle;
        }

        internal int ObjectId { get; set; }

        internal string FontName { get; }

        internal byte[] FontBytes { get; }

        internal int MinX { get; }

        internal int MinY { get; }

        internal int MaxX { get; }

        internal int MaxY { get; }

        internal int Ascent { get; }

        internal int Descent { get; }

        internal int CapHeight { get; }

        internal int Flags { get; }

        internal int ItalicAngle { get; }

        internal PdfTrueTypeFontMetrics Metrics { get; }

        internal int GetGlyphId( int codePoint )
        {
            return glyphsByCodePoint.TryGetValue( codePoint, out int glyphId )
                ? glyphId
                : 0;
        }

        internal string BuildWidthArray()
        {
            if ( glyphWidths.Count == 0 )
                return "[]";

            StringBuilder builder = new();
            builder.Append( "[ " );

            foreach ( KeyValuePair<int, int> pair in glyphWidths.OrderBy( x => x.Key ) )
            {
                if ( pair.Value == PdfTrueTypeFontMetrics.DefaultGlyphWidth )
                    continue;

                builder.Append( pair.Key.ToString( CultureInfo.InvariantCulture ) );
                builder.Append( ' ' );
                builder.Append( pair.Key.ToString( CultureInfo.InvariantCulture ) );
                builder.Append( ' ' );
                builder.Append( pair.Value.ToString( CultureInfo.InvariantCulture ) );
                builder.Append( ' ' );
            }

            builder.Append( ']' );

            return builder.ToString();
        }

        internal List<KeyValuePair<int, int>> CreateGlyphUnicodeMappings()
        {
            Dictionary<int, int> mappings = [];

            foreach ( KeyValuePair<int, int> pair in glyphsByCodePoint )
            {
                if ( pair.Value <= 0 )
                    continue;

                if ( !mappings.ContainsKey( pair.Value ) )
                    mappings.Add( pair.Value, pair.Key );
            }

            return mappings
                .OrderBy( x => x.Key )
                .ToList();
        }

        internal static bool TryCreate( FontFamily family, int variantIndex, int resourceIndex, out PdfEmbeddedFont embeddedFont )
        {
            embeddedFont = null;
            bool bold = variantIndex is 1 or 3;
            bool italic = variantIndex is 2 or 3;
            FontSource source = family?.ResolveSource( bold, italic );

            if ( !TryReadFontSource( source, out byte[] fontBytes ) )
                return false;

            return TryReadTrueTypeFont( fontBytes, family?.Name ?? Fonts.Helvetica, variantIndex, resourceIndex, out embeddedFont );
        }

        private static bool TryReadFontSource( FontSource source, out byte[] fontBytes )
        {
            fontBytes = null;

            if ( source is null )
                return false;

            if ( source.Format is not FontFormat.TrueType and not FontFormat.OpenType )
                return false;

            if ( source.Data is { Length: > 0 } )
            {
                fontBytes = source.Data;
                return true;
            }

            if ( !string.IsNullOrWhiteSpace( source.FileName ) && File.Exists( source.FileName ) )
            {
                fontBytes = File.ReadAllBytes( source.FileName );
                return true;
            }

            return false;
        }

        private static bool TryReadTrueTypeFont( byte[] fontBytes, string family, int variantIndex, int resourceIndex, out PdfEmbeddedFont embeddedFont )
        {
            embeddedFont = null;

            if ( !TryReadTableDirectory( fontBytes, out Dictionary<string, TrueTypeTable> tables ) )
                return false;

            if ( !TryReadCMap( fontBytes, tables, out Dictionary<int, int> glyphsByCodePoint ) || glyphsByCodePoint.Count == 0 )
                return false;

            int unitsPerEm = 1000;
            int minX = 0;
            int minY = -250;
            int maxX = 1000;
            int maxY = 1000;
            int ascent = 800;
            int descent = -200;

            if ( TryGetTable( tables, "head", out TrueTypeTable head ) && head.Offset + 54 <= fontBytes.Length )
            {
                unitsPerEm = Math.Max( 1, ReadUInt16( fontBytes, head.Offset + 18 ) );
                minX = ScaleMetric( ReadInt16( fontBytes, head.Offset + 36 ), unitsPerEm );
                minY = ScaleMetric( ReadInt16( fontBytes, head.Offset + 38 ), unitsPerEm );
                maxX = ScaleMetric( ReadInt16( fontBytes, head.Offset + 40 ), unitsPerEm );
                maxY = ScaleMetric( ReadInt16( fontBytes, head.Offset + 42 ), unitsPerEm );
            }

            if ( TryGetTable( tables, "hhea", out TrueTypeTable hhea ) && hhea.Offset + 8 <= fontBytes.Length )
            {
                ascent = ScaleMetric( ReadInt16( fontBytes, hhea.Offset + 4 ), unitsPerEm );
                descent = ScaleMetric( ReadInt16( fontBytes, hhea.Offset + 6 ), unitsPerEm );
            }

            Dictionary<int, int> glyphWidths = TryReadGlyphWidths( fontBytes, tables, unitsPerEm, out Dictionary<int, int> parsedGlyphWidths )
                ? parsedGlyphWidths
                : [];

            bool italic = variantIndex is 2 or 3;
            int flags = 32;

            if ( IsFontFamily( family, Fonts.Courier ) )
                flags |= 1;

            if ( IsFontFamily( family, Fonts.Times ) )
                flags |= 2;

            if ( italic )
                flags |= 64;

            embeddedFont = new(
                FormattableString.Invariant( $"BlazoriseEmbeddedFont{resourceIndex}" ),
                fontBytes,
                glyphsByCodePoint,
                glyphWidths,
                minX,
                minY,
                maxX,
                maxY,
                ascent,
                descent,
                flags,
                italic ? -12 : 0 );

            return true;
        }

        private static int ScaleMetric( int value, int unitsPerEm )
        {
            return (int)Math.Round( value * 1000d / unitsPerEm );
        }

        private static bool TryReadGlyphWidths( byte[] fontBytes, Dictionary<string, TrueTypeTable> tables, int unitsPerEm, out Dictionary<int, int> glyphWidths )
        {
            glyphWidths = [];

            if ( !TryGetTable( tables, "hhea", out TrueTypeTable hhea )
                 || !TryGetTable( tables, "maxp", out TrueTypeTable maxp )
                 || !TryGetTable( tables, "hmtx", out TrueTypeTable hmtx ) )
                return false;

            if ( hhea.Offset + 36 > fontBytes.Length || maxp.Offset + 6 > fontBytes.Length )
                return false;

            int horizontalMetricCount = ReadUInt16( fontBytes, hhea.Offset + 34 );
            int glyphCount = ReadUInt16( fontBytes, maxp.Offset + 4 );

            if ( horizontalMetricCount <= 0 || glyphCount <= 0 )
                return false;

            int readableMetricCount = Math.Min( horizontalMetricCount, glyphCount );
            int hmtxEndOffset = Math.Min( fontBytes.Length, hmtx.Offset + hmtx.Length );

            if ( hmtx.Offset + ( readableMetricCount * 4 ) > hmtxEndOffset )
                return false;

            int lastAdvanceWidth = PdfTrueTypeFontMetrics.DefaultGlyphWidth;

            for ( int glyphId = 0; glyphId < glyphCount; glyphId++ )
            {
                int advanceWidth;

                if ( glyphId < readableMetricCount )
                {
                    advanceWidth = ReadUInt16( fontBytes, hmtx.Offset + ( glyphId * 4 ) );
                    lastAdvanceWidth = advanceWidth;
                }
                else
                {
                    advanceWidth = lastAdvanceWidth;
                }

                glyphWidths[glyphId] = ScaleMetric( advanceWidth, unitsPerEm );
            }

            return glyphWidths.Count > 0;
        }

        private static bool TryReadTableDirectory( byte[] fontBytes, out Dictionary<string, TrueTypeTable> tables )
        {
            tables = [];

            if ( fontBytes.Length < 12 )
                return false;

            int tableCount = ReadUInt16( fontBytes, 4 );
            int tableOffset = 12;

            for ( int i = 0; i < tableCount; i++ )
            {
                int recordOffset = tableOffset + ( i * 16 );

                if ( recordOffset + 16 > fontBytes.Length )
                    return false;

                string tag = Encoding.ASCII.GetString( fontBytes, recordOffset, 4 );
                int offset = (int)ReadUInt32( fontBytes, recordOffset + 8 );
                int length = (int)ReadUInt32( fontBytes, recordOffset + 12 );

                if ( offset < 0 || length < 0 || offset + length > fontBytes.Length )
                    continue;

                tables[tag] = new( offset, length );
            }

            return tables.Count > 0;
        }

        private static bool TryReadCMap( byte[] fontBytes, Dictionary<string, TrueTypeTable> tables, out Dictionary<int, int> glyphsByCodePoint )
        {
            glyphsByCodePoint = [];

            if ( !TryGetTable( tables, "cmap", out TrueTypeTable cmap ) || cmap.Offset + 4 > fontBytes.Length )
                return false;

            int recordCount = ReadUInt16( fontBytes, cmap.Offset + 2 );
            int bestFormat12Offset = -1;
            int bestFormat4Offset = -1;

            for ( int i = 0; i < recordCount; i++ )
            {
                int recordOffset = cmap.Offset + 4 + ( i * 8 );

                if ( recordOffset + 8 > fontBytes.Length )
                    break;

                int platformId = ReadUInt16( fontBytes, recordOffset );
                int encodingId = ReadUInt16( fontBytes, recordOffset + 2 );
                int subtableOffset = cmap.Offset + (int)ReadUInt32( fontBytes, recordOffset + 4 );

                if ( subtableOffset + 2 > fontBytes.Length )
                    continue;

                int format = ReadUInt16( fontBytes, subtableOffset );

                if ( format == 12 && ( platformId == 3 && encodingId == 10 || bestFormat12Offset < 0 ) )
                    bestFormat12Offset = subtableOffset;
                else if ( format == 4 && ( platformId == 3 || bestFormat4Offset < 0 ) )
                    bestFormat4Offset = subtableOffset;
            }

            if ( bestFormat12Offset >= 0 && TryReadFormat12CMap( fontBytes, bestFormat12Offset, glyphsByCodePoint ) )
                return true;

            return bestFormat4Offset >= 0 && TryReadFormat4CMap( fontBytes, bestFormat4Offset, glyphsByCodePoint );
        }

        private static bool TryReadFormat12CMap( byte[] fontBytes, int offset, Dictionary<int, int> glyphsByCodePoint )
        {
            if ( offset + 16 > fontBytes.Length )
                return false;

            int groupCount = (int)ReadUInt32( fontBytes, offset + 12 );

            for ( int i = 0; i < groupCount; i++ )
            {
                int groupOffset = offset + 16 + ( i * 12 );

                if ( groupOffset + 12 > fontBytes.Length )
                    break;

                int startCodePoint = (int)ReadUInt32( fontBytes, groupOffset );
                int endCodePoint = (int)ReadUInt32( fontBytes, groupOffset + 4 );
                int startGlyphId = (int)ReadUInt32( fontBytes, groupOffset + 8 );

                for ( int codePoint = startCodePoint; codePoint <= endCodePoint && codePoint <= 0x10FFFF; codePoint++ )
                {
                    glyphsByCodePoint[codePoint] = startGlyphId + codePoint - startCodePoint;
                }
            }

            return glyphsByCodePoint.Count > 0;
        }

        private static bool TryReadFormat4CMap( byte[] fontBytes, int offset, Dictionary<int, int> glyphsByCodePoint )
        {
            if ( offset + 16 > fontBytes.Length )
                return false;

            int length = ReadUInt16( fontBytes, offset + 2 );
            int endOffset = Math.Min( fontBytes.Length, offset + length );
            int segmentCount = ReadUInt16( fontBytes, offset + 6 ) / 2;
            int endCodeOffset = offset + 14;
            int startCodeOffset = endCodeOffset + ( segmentCount * 2 ) + 2;
            int idDeltaOffset = startCodeOffset + ( segmentCount * 2 );
            int idRangeOffsetOffset = idDeltaOffset + ( segmentCount * 2 );

            if ( idRangeOffsetOffset + ( segmentCount * 2 ) > endOffset )
                return false;

            for ( int i = 0; i < segmentCount; i++ )
            {
                int endCode = ReadUInt16( fontBytes, endCodeOffset + ( i * 2 ) );
                int startCode = ReadUInt16( fontBytes, startCodeOffset + ( i * 2 ) );
                int idDelta = ReadInt16( fontBytes, idDeltaOffset + ( i * 2 ) );
                int idRangeOffset = ReadUInt16( fontBytes, idRangeOffsetOffset + ( i * 2 ) );

                if ( startCode == 0xFFFF && endCode == 0xFFFF )
                    continue;

                for ( int codePoint = startCode; codePoint <= endCode && codePoint < 0xFFFF; codePoint++ )
                {
                    int glyphId;

                    if ( idRangeOffset == 0 )
                    {
                        glyphId = ( codePoint + idDelta ) & 0xFFFF;
                    }
                    else
                    {
                        int glyphIndexOffset = idRangeOffsetOffset + ( i * 2 ) + idRangeOffset + ( ( codePoint - startCode ) * 2 );

                        if ( glyphIndexOffset + 2 > endOffset )
                            continue;

                        glyphId = ReadUInt16( fontBytes, glyphIndexOffset );

                        if ( glyphId != 0 )
                            glyphId = ( glyphId + idDelta ) & 0xFFFF;
                    }

                    if ( glyphId != 0 )
                        glyphsByCodePoint[codePoint] = glyphId;
                }
            }

            return glyphsByCodePoint.Count > 0;
        }

        private static bool TryGetTable( Dictionary<string, TrueTypeTable> tables, string tag, out TrueTypeTable table )
        {
            return tables.TryGetValue( tag, out table );
        }

        private static int ReadUInt16( byte[] data, int offset )
        {
            return ( data[offset] << 8 ) | data[offset + 1];
        }

        private static int ReadInt16( byte[] data, int offset )
        {
            int value = ReadUInt16( data, offset );

            return value >= 0x8000 ? value - 0x10000 : value;
        }

        private static uint ReadUInt32( byte[] data, int offset )
        {
            return ( (uint)data[offset] << 24 ) | ( (uint)data[offset + 1] << 16 ) | ( (uint)data[offset + 2] << 8 ) | data[offset + 3];
        }
    }

    private readonly record struct TrueTypeTable( int Offset, int Length );

    private sealed class PdfFontResource
    {
        internal double MeasureTextWidth( string text, double fontSize )
        {
            return Metrics?.MeasureTextWidth( text, fontSize ) ?? 0;
        }

        internal string Name { get; set; }

        internal int ObjectId { get; set; }

        internal PdfEmbeddedFont EmbeddedFont { get; set; }

        internal IPdfFontMetrics Metrics { get; set; }
    }

    private sealed class PdfFontResources
    {
        private readonly Dictionary<string, PdfFontResource[]> families = new( StringComparer.OrdinalIgnoreCase );

        internal List<PdfFontResource> All { get; } = [];

        internal void Add( string family, PdfFontResource[] resources )
        {
            families[family] = resources;
            All.AddRange( resources );
        }

        internal PdfFontResource[] GetFamily( string family )
        {
            return families.TryGetValue( family, out PdfFontResource[] resources )
                ? resources
                : families[Fonts.Helvetica];
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