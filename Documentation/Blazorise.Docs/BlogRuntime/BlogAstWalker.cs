using System;
using Markdig.Syntax;

namespace Blazorise.Docs.BlogRuntime;

internal static class BlogAstWalker
{
    public static TOut Build<TOut>(
        IBlogSink<TOut> sink,
        string markdown,
        Func<string, string> imageRewriter,
        out FrontMatter info )
    {
        info = MarkdownFrontMatter.Parse( markdown );

        // Resolve front-matter image fields here
        var resolvedCover = string.IsNullOrWhiteSpace( info.ImageUrl ) ? info.ImageUrl : imageRewriter( info.ImageUrl );
        var resolvedAuthor = string.IsNullOrWhiteSpace( info.AuthorImage ) ? info.AuthorImage : imageRewriter( info.AuthorImage );

        var doc = Markdig.Markdown.Parse( info.MarkdownText );

        sink.AddPageAndSeo( info.Permalink, info.Title, info.Description, resolvedCover ?? "", info.ImageTitle );

        foreach ( var block in doc )
        {
            switch ( block )
            {
                case HeadingBlock h when h.Level == 1:
                    sink.AddPageTitle( h );
                    break;
                case HeadingBlock h when h.Level == 2:
                    sink.AddPageSubtitle( h );
                    break;
                case HeadingBlock h when h.Level > 2:
                    sink.AddPageHeading( h );
                    break;
                case ParagraphBlock p:
                    sink.AddPageParagraph( p );
                    break;
                case QuoteBlock q:
                    sink.AddPageQuote( q );
                    break;
                case ListBlock l:
                    sink.AddPageList( l );
                    break;
                case FencedCodeBlock c:
                    sink.PersistCodeBlock( c, 0 );
                    break;
            }
        }

        sink.AddPagePostInfo( info.AuthorName, resolvedAuthor ?? "", info.PostedOn, info.ReadTime );

        return sink.Build();
    }
}