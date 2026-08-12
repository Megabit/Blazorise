using System.Collections.Generic;
using Blazorise.Docs.BlogRuntime;
using Markdig.Syntax;
using Xunit;

namespace Blazorise.Tests.BlogRuntime;

public class BlogAstWalkerTest
{
    private const string VideoId = "GC5E8ie2pdM";

    [Fact]
    public void YouTubePlaceholderUsesSuppliedVideoId()
    {
        RecordingBlogSink sink = Build( $"{{% youtube {VideoId} %}}" );

        Assert.Equal( new[] { VideoId }, sink.VideoIds );
        Assert.Empty( sink.Paragraphs );
    }

    [Theory]
    [InlineData( "{% youtube %}" )]
    [InlineData( "{% youtube invalid %}" )]
    [InlineData( "{% youtube abc/defghij %}" )]
    public void MissingOrInvalidVideoIdDoesNotRenderVideo( string placeholder )
    {
        RecordingBlogSink sink = Build( placeholder );

        Assert.Empty( sink.VideoIds );
        Assert.Empty( sink.Paragraphs );
    }

    [Fact]
    public void PlaceholderMustBeAStandaloneMarkdownBlock()
    {
        RecordingBlogSink sink = Build( $"Watch this: {{% youtube {VideoId} %}}" );

        Assert.Empty( sink.VideoIds );
        Assert.Single( sink.Paragraphs );
    }

    [Fact]
    public void PlaceholderInsideFencedCodeIsNotProcessed()
    {
        RecordingBlogSink sink = Build( $$"""
            ```liquid
            {% youtube {{VideoId}} %}
            ```
            """ );

        Assert.Empty( sink.VideoIds );
        Assert.Equal( 1, sink.CodeBlockCount );
    }

    [Theory]
    [InlineData( VideoId, "https://www.youtube-nocookie.com/embed/GC5E8ie2pdM" )]
    [InlineData( "invalid", null )]
    [InlineData( "abcdefghij~", null )]
    [InlineData( null, null )]
    public void EmbedUrlIsCreatedOnlyForValidVideoIds( string videoId, string expected )
    {
        Assert.Equal( expected, YouTubeVideoPlaceholder.GetEmbedUrl( videoId ) );
    }

    private static RecordingBlogSink Build( string markdown )
    {
        RecordingBlogSink sink = new();
        BlogAstWalker.Build( sink, markdown, url => url, out _ );
        return sink;
    }

    private sealed class RecordingBlogSink : IBlogSink<RecordingBlogSink>
    {
        public List<string> VideoIds { get; } = new();

        public List<string> Paragraphs { get; } = new();

        public int CodeBlockCount { get; private set; }

        public void AddPageAndSeo( string url, string title, string desc, string imageUrl, string imageTitle )
        {
        }

        public void AddPageVideo( string videoId )
        {
            if ( videoId is not null )
                VideoIds.Add( videoId );
        }

        public void AddPagePostInfo( string authorName, string authorImage, string postedOn, string readTime )
        {
        }

        public void AddPageTitle( HeadingBlock h1 )
        {
        }

        public void AddPageSubtitle( HeadingBlock h2 )
        {
        }

        public void AddPageHeading( HeadingBlock hN )
        {
        }

        public void AddPageLead( ParagraphBlock p )
        {
        }

        public void AddPageParagraph( ParagraphBlock p )
            => Paragraphs.Add( p.Inline?.ToString() ?? string.Empty );

        public void AddPageQuote( QuoteBlock q )
        {
        }

        public void AddPageList( ListBlock list )
        {
        }

        public void PersistCodeBlock( FencedCodeBlock code, int indentLevel )
            => CodeBlockCount++;

        public void AddPageTable( Markdig.Extensions.Tables.Table table )
        {
        }

        public void AddPageDivider()
        {
        }

        public RecordingBlogSink Build() => this;
    }
}