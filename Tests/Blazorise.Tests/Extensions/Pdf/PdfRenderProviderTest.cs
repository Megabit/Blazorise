#region Using directives
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Pdf;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions.Pdf;

public class PdfRenderProviderTest
{
    [Fact]
    public async Task RenderAsync_Should_Write_Structure_Metadata_And_Escaped_Text()
    {
        const string Title = "PDF structure Ž";
        PdfDocumentDefinition document = CreateDocument();
        document.Title = Title;
        document.Pages[0].Elements.Add( CreateText( "A(B)\\C", 10, 10, 100, 20 ) );

        string pdf = await RenderAsync( document );

        Assert.StartsWith( "%PDF-1.4\n", pdf );
        Assert.EndsWith( "%%EOF", pdf );
        Assert.Contains( "/Type /Catalog", pdf );
        Assert.Contains( "/Type /Pages", pdf );
        Assert.Contains( "/Type /Page", pdf );
        Assert.Contains( "/Info ", pdf );
        Assert.Contains( $"/Title <FEFF{Convert.ToHexString( Encoding.BigEndianUnicode.GetBytes( Title ) )}>", pdf );
        Assert.Contains( @"(A\(B\)\\C) Tj", pdf );

        AssertCrossReferenceOffsets( pdf );
    }

    [Fact]
    public async Task RenderAsync_Should_Render_Standard_And_Embedded_Fonts_With_Unicode_Map()
    {
        PdfDocumentDefinition document = CreateDocument();
        document.AddFont( "OpenSansTest", FontSource.FromBytes( ReadAsset( "OpenSans-Regular.ttf" ) ) );
        document.Pages[0].Elements.Add( CreateText( "Standard", 10, 10, 100, 20 ) );

        PdfElementDefinition embeddedText = CreateText( "Ž", 10, 40, 100, 20 );
        embeddedText.Font.Family = "OpenSansTest";
        document.Pages[0].Elements.Add( embeddedText );

        string pdf = await RenderAsync( document );

        Assert.Contains( "/Subtype /Type1 /BaseFont /Helvetica", pdf );
        Assert.Contains( "/Subtype /Type0", pdf );
        Assert.Contains( "/FontFile2", pdf );
        Assert.Contains( "/ToUnicode", pdf );

        System.Text.RegularExpressions.Match unicodeMapping = Regex.Match( pdf, @"<(?<glyph>[0-9A-F]{4})> <017D>" );
        Assert.True( unicodeMapping.Success );
        Assert.Contains( $"<{unicodeMapping.Groups["glyph"].Value}> Tj", pdf );
    }

    [Fact]
    public async Task RenderAsync_Should_Embed_Jpeg_And_Transparent_Png_Images()
    {
        PdfDocumentDefinition document = CreateDocument();
        document.Pages[0].Elements.Add( CreateImage( CreateDataUri( "image/jpeg", ReadAsset( "image.jpg" ) ), 10, 10 ) );
        document.Pages[0].Elements.Add( CreateImage( CreateDataUri( "image/png", ReadAsset( "image.png" ) ), 70, 10 ) );

        string pdf = await RenderAsync( document );

        Assert.Contains( "/Filter /DCTDecode", pdf );
        Assert.Contains( "/Filter /FlateDecode", pdf );
        Assert.Contains( "/SMask", pdf );
        Assert.True( CountOccurrences( pdf, "/Subtype /Image" ) >= 3 );
        Assert.Contains( "/Im1 Do", pdf );
        Assert.Contains( "/Im2 Do", pdf );
    }

    [Fact]
    public async Task RenderAsync_Should_Wrap_And_Clip_Text_To_Its_Bounds()
    {
        PdfDocumentDefinition document = CreateDocument();
        PdfElementDefinition clippedText = CreateText( "one two three four", 10, 10, 25, 50 );
        PdfElementDefinition overflowingText = CreateText( "overflow", 50, 10, 25, 20 );
        overflowingText.ClipContent = false;
        document.Pages[0].Elements.Add( clippedText );
        document.Pages[0].Elements.Add( overflowingText );

        string pdf = await RenderAsync( document );

        Assert.Contains( "10 240 25 50 re W n", pdf );
        Assert.DoesNotContain( "50 270 25 20 re W n", pdf );
        Assert.True( CountOccurrences( pdf, " Tj ET" ) >= 4 );
    }

    [Fact]
    public async Task RenderAsync_Should_Apply_Table_And_Nested_Element_Offsets()
    {
        PdfDocumentDefinition document = CreateDocument();
        PdfElementDefinition nestedText = CreateText( "Nested", 5, 6, 30, 10 );
        nestedText.Wrap = false;
        PdfElementDefinition table = new()
        {
            Type = PdfElementType.Table,
            X = 10,
            Y = 20,
            Width = 100,
            Height = 50,
            Border = new()
            {
                Width = 1,
            },
            Rows =
            [
                new()
                {
                    Height = 30,
                    Cells =
                    [
                        new()
                        {
                            Width = 40,
                            Elements = [nestedText],
                        },
                    ],
                },
            ],
        };
        document.Pages[0].Elements.Add( table );

        string pdf = await RenderAsync( document );

        Assert.Contains( "10 230 100 50 re W n", pdf );
        Assert.Contains( "10 250 40 30 re S", pdf );
        Assert.Contains( "15 264 30 10 re W n", pdf );
        Assert.Contains( "15 262 Td (Nested) Tj", pdf );
    }

    [Fact]
    public async Task RenderAsync_Should_Normalize_Invalid_Definition_Values()
    {
        PdfElementDefinition element = CreateText( "Fallback", double.PositiveInfinity, double.NaN, -10, double.NaN );
        element.Type = (PdfElementType)999;
        element.Font.Family = null;
        element.Font.Size = double.NaN;
        element.Font.Color = "invalid";
        element.Border.Color = "invalid";
        element.Appearance.BackgroundColor = "invalid";

        PdfDocumentDefinition document = new()
        {
            PageSize = (PdfPageSize)999,
            Orientation = (PdfOrientation)999,
            PageWidth = double.NaN,
            PageHeight = -10,
            Fonts = [null],
            Pages =
            [
                null,
                new()
                {
                    Size = (PdfPageSize)999,
                    Orientation = (PdfOrientation)999,
                    Width = double.NaN,
                    Height = -10,
                    Elements = [null, element],
                },
            ],
        };

        string pdf = await RenderAsync( document );

        Assert.StartsWith( "%PDF-1.4", pdf );
        Assert.Single( document.Pages );
        Assert.Empty( document.Fonts );
        Assert.Single( document.Pages[0].Elements );
        Assert.Equal( PdfElementType.Text, element.Type );
        Assert.Equal( 0d, element.X );
        Assert.Equal( 0d, element.Y );
        Assert.Equal( 0d, element.Width );
        Assert.Equal( 0d, element.Height );
        Assert.Equal( "Helvetica", element.Font.Family );
        Assert.Equal( 12d, element.Font.Size );
        Assert.Equal( "#000000", element.Font.Color );
        Assert.Equal( "#000000", element.Border.Color );
        Assert.Null( element.Appearance.BackgroundColor );
        Assert.True( document.Pages[0].Width > 0 );
        Assert.True( document.Pages[0].Height > 0 );
    }

    [Fact]
    public async Task RenderAsync_Should_Reject_Documents_Above_Configured_Limits()
    {
        PdfDocumentDefinition document = CreateDocument();
        document.Pages.Add( CreatePage() );
        SimplePdfRenderProvider provider = new();

        await Assert.ThrowsAsync<InvalidDataException>( () => provider.RenderAsync( document, new()
        {
            MaxPages = 1,
        } ) );
    }

    [Fact]
    public async Task RenderToStreamAsync_Should_Observe_Cancellation_During_Output()
    {
        PdfDocumentDefinition document = CreateDocument();
        document.Pages[0].Elements.Add( CreateText( new string( 'A', 10000 ), 10, 10, 100, 200 ) );
        SimplePdfRenderProvider provider = new();
        using CancellationTokenSource cancellationTokenSource = new();
        using CancelAfterWriteStream stream = new( cancellationTokenSource );

        await Assert.ThrowsAnyAsync<OperationCanceledException>( () => provider.RenderToStreamAsync( document, stream, new(), cancellationTokenSource.Token ) );

        Assert.True( stream.Length > 0 );
        Assert.False( Encoding.Latin1.GetString( stream.ToArray() ).EndsWith( "%%EOF", StringComparison.Ordinal ) );
    }

    private static async Task<string> RenderAsync( PdfDocumentDefinition document )
    {
        SimplePdfRenderProvider provider = new();
        PdfGenerationResult result = await provider.RenderAsync( document, new() );

        return Encoding.Latin1.GetString( result.Content );
    }

    private static PdfDocumentDefinition CreateDocument()
    {
        return new()
        {
            PageSize = PdfPageSize.Custom,
            PageWidth = 200,
            PageHeight = 300,
            Pages = [CreatePage()],
        };
    }

    private static PdfPageDefinition CreatePage()
    {
        return new()
        {
            Size = PdfPageSize.Custom,
            Width = 200,
            Height = 300,
        };
    }

    private static PdfElementDefinition CreateText( string text, double x, double y, double width, double height )
    {
        return new()
        {
            Type = PdfElementType.Text,
            Text = text,
            X = x,
            Y = y,
            Width = width,
            Height = height,
        };
    }

    private static PdfElementDefinition CreateImage( string source, double x, double y )
    {
        return new()
        {
            Type = PdfElementType.Image,
            Source = source,
            X = x,
            Y = y,
            Width = 50,
            Height = 50,
        };
    }

    private static string CreateDataUri( string mediaType, byte[] data )
    {
        return $"data:{mediaType};base64,{Convert.ToBase64String( data )}";
    }

    private static byte[] ReadAsset( string fileName )
    {
        return File.ReadAllBytes( Path.Combine( AppContext.BaseDirectory, "Assets", "Pdf", fileName ) );
    }

    private static int CountOccurrences( string value, string searchValue )
    {
        int count = 0;
        int index = 0;

        while ( ( index = value.IndexOf( searchValue, index, StringComparison.Ordinal ) ) >= 0 )
        {
            count++;
            index += searchValue.Length;
        }

        return count;
    }

    private static void AssertCrossReferenceOffsets( string pdf )
    {
        int startCrossReferenceIndex = pdf.LastIndexOf( "startxref\n", StringComparison.Ordinal );
        Assert.True( startCrossReferenceIndex >= 0 );

        int offsetStart = startCrossReferenceIndex + "startxref\n".Length;
        int offsetEnd = pdf.IndexOf( '\n', offsetStart );
        int crossReferenceOffset = int.Parse( pdf[offsetStart..offsetEnd], CultureInfo.InvariantCulture );
        Assert.StartsWith( "xref\n", pdf[crossReferenceOffset..] );

        string[] lines = pdf[crossReferenceOffset..].Split( '\n' );
        string[] header = lines[1].Split( ' ', StringSplitOptions.RemoveEmptyEntries );
        int objectCount = int.Parse( header[1], CultureInfo.InvariantCulture );

        for ( int objectIndex = 1; objectIndex < objectCount; objectIndex++ )
        {
            int objectOffset = int.Parse( lines[objectIndex + 2][..10], CultureInfo.InvariantCulture );
            Assert.StartsWith( $"{objectIndex} 0 obj", pdf[objectOffset..] );
        }
    }

    private sealed class CancelAfterWriteStream : MemoryStream
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        internal CancelAfterWriteStream( CancellationTokenSource cancellationTokenSource )
        {
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public override async ValueTask WriteAsync( ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default )
        {
            await base.WriteAsync( buffer, cancellationToken );
            cancellationTokenSource.Cancel();
        }
    }
}