using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;

namespace Blazorise.Docs.Compiler;

public class BlogMarkdown
{
    public bool Execute()
    {
        var success = true;

        try
        {
            var dirPath = Paths.DirPath();
            var markdownFiles = Directory.EnumerateFiles( dirPath, "*.md", SearchOption.AllDirectories );

            foreach ( var entry in markdownFiles.OrderBy( e => e.Replace( "\\", "/" ), StringComparer.Ordinal ) )
            {
                var blogDirectory = Path.GetDirectoryName( entry );
                var markdownFilename = Path.GetFileName( entry );
                var razorFilename = Path.Combine( blogDirectory, markdownFilename.Replace( ".md", ".razor" ) );

                if ( markdownFilename.Equals( "ReadMe.md", StringComparison.InvariantCultureIgnoreCase ) )
                    continue;

                var blogName = Path.GetFileName( blogDirectory ).Substring( "YYYY-MM-DD_".Length );
                var blogBuilder = new BlogBuilder( blogName, blogDirectory );

                var pageInfo = ParsePageInfo( blogBuilder, File.ReadAllText( entry, Encoding.UTF8 ) );
                var markdownDocument = Markdown.Parse( pageInfo.MarkdownText );

                var currentPageCode = string.Empty;
                var builtPageCode = string.Empty;

                blogBuilder.AddPageAndSeo( pageInfo.Permalink, pageInfo.Title, pageInfo.Description, pageInfo.ImageUrl, pageInfo.ImageTitle );

                foreach ( var block in markdownDocument )
                {
                    switch ( block )
                    {
                        case HeadingBlock headingBlock:
                            if ( headingBlock.Level == 1 )
                                blogBuilder.AddPageTitle( headingBlock );
                            else if ( headingBlock.Level == 2 )
                                blogBuilder.AddPageSubtitle( headingBlock );
                            else if ( headingBlock.Level > 2 )
                                blogBuilder.AddPageHeading( headingBlock );
                            break;
                        case ParagraphBlock paragraphBlock:
                            blogBuilder.AddPageParagraph( paragraphBlock );
                            break;
                        case QuoteBlock quoteBlock:
                            blogBuilder.AddPageQuote( quoteBlock );
                            break;
                        case ListBlock listBlock:
                            blogBuilder.AddPageList( listBlock );
                            break;
                        case FencedCodeBlock fencedCodeBlock:
                            blogBuilder.PersistCodeBlock( fencedCodeBlock, 0 );
                            break;
                    }
                }

                blogBuilder.AddPagePostInto( pageInfo.AuthorName, pageInfo.AuthorImage, pageInfo.PostedOn, pageInfo.ReadTime );

                if ( File.Exists( razorFilename ) )
                {
                    currentPageCode = File.ReadAllText( razorFilename );
                }

                builtPageCode = blogBuilder.ToString();

                if ( currentPageCode != builtPageCode )
                {
                    File.WriteAllText( razorFilename, blogBuilder.ToString() );
                }
            }
        }
        catch ( Exception e )
        {
            Console.WriteLine( $"Error generating blogs {Paths.SnippetsFilePath} : {e.Message}" );
            success = false;
        }

        return success;
    }

    /// <summary>
    /// Reads extra info about the page that should be at the beginning of the *.md file.
    /// </summary>
    /// <param name="blogBuilder"></param>
    /// <param name="markdownText"></param>
    /// <returns></returns>
    private PageInfo ParsePageInfo( BlogBuilder blogBuilder, string markdownText )
    {
        PageInfo pageInfo = new PageInfo()
        {
            MarkdownText = markdownText
        };

        if ( markdownText.StartsWith( "---" ) )
        {
            var subString = ( string line ) => line.Substring( line.Length + 1 ).Trim();

            var seoEnding = markdownText.IndexOf( "---", 3 );

            var pageInfoString = markdownText.Substring( 3, seoEnding - 3 ).Trim().Split( '\n' );

            foreach ( var line in pageInfoString )
            {
                if ( line.StartsWith( "title:" ) )
                    pageInfo.Title = line.Substring( "title:".Length + 1 ).Trim();
                else if ( line.StartsWith( "description:" ) )
                    pageInfo.Description = line.Substring( "description:".Length + 1 ).Trim();
                else if ( line.StartsWith( "permalink:" ) )
                    pageInfo.Permalink = line.Substring( "permalink:".Length + 1 ).Trim();
                else if ( line.StartsWith( "image-url:" ) )
                    pageInfo.ImageUrl = line.Substring( "image-url:".Length + 1 ).Trim();
                else if ( line.StartsWith( "image-title:" ) )
                    pageInfo.ImageTitle = line.Substring( "image-title:".Length + 1 ).Trim();
                else if ( line.StartsWith( "author-name:" ) )
                    pageInfo.AuthorName = line.Substring( "author-name:".Length + 1 ).Trim();
                else if ( line.StartsWith( "author-image:" ) )
                    pageInfo.AuthorImage = line.Substring( "author-image:".Length + 1 ).Trim();
                else if ( line.StartsWith( "posted-on:" ) )
                    pageInfo.PostedOn = line.Substring( "posted-on:".Length + 1 ).Trim();
                else if ( line.StartsWith( "read-time:" ) )
                    pageInfo.ReadTime = line.Substring( "read-time:".Length + 1 ).Trim();
            }

            pageInfo.MarkdownText = markdownText.Substring( seoEnding + 3 ).TrimStart();
        }

        return pageInfo;
    }

    class PageInfo
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Permalink { get; set; }

        public string ImageUrl { get; set; }

        public string ImageTitle { get; set; }

        public string AuthorName { get; set; }

        public string AuthorImage { get; set; }

        public string PostedOn { get; set; }

        public string ReadTime { get; set; }

        public string MarkdownText { get; set; }
    }
}