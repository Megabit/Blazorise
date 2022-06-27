using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;

namespace Blazorise.Docs.Compiler
{
    public class BlogMarkdown
    {
        public bool Execute()
        {
            var paths = new Paths();
            var success = true;

            try
            {
                var dirPath = paths.DirPath();
                var markdownFiles = Directory.EnumerateFiles( dirPath, "*.md", SearchOption.AllDirectories );

                foreach ( var entry in markdownFiles.OrderBy( e => e.Replace( "\\", "/" ), StringComparer.Ordinal ) )
                {
                    var bb = new BlogBuilder();

                    var directory = Path.GetDirectoryName( entry );
                    var markdownFilename = Path.GetFileName( entry );
                    var razorFilename = Path.Combine( directory, markdownFilename.Replace( ".md", ".razor" ) );

                    var markdownText = ParsePageInfo( bb, File.ReadAllText( entry, Encoding.UTF8 ) );
                    var markdownDocument = Markdown.Parse( markdownText );

                    var currentPageCode = string.Empty;
                    var builtPageCode = string.Empty;



                    foreach ( var block in markdownDocument )
                    {
                        if ( block is HeadingBlock headingBlock )
                        {
                            if ( headingBlock.Level == 1 )
                                bb.AddPageTitle( headingBlock );
                            else if ( headingBlock.Level == 2 )
                                bb.AddPageSubtitle( headingBlock );
                        }
                        else if ( block is ParagraphBlock paragraphBlock )
                        {
                            bb.AddPageParagraph( paragraphBlock );
                        }
                        else if ( block is ListBlock listBlock )
                        {
                            bb.AddPageList( listBlock );
                        }
                        else if ( block is FencedCodeBlock fencedCodeBlock )
                        {
                            var codeBlockFileName = Path.Combine( directory, "Code", "TestCode.html" );
                            var codeBlockDirectory = Path.GetDirectoryName( codeBlockFileName );

                            if ( !Directory.Exists( codeBlockDirectory ) )
                            {
                                Directory.CreateDirectory( codeBlockDirectory );
                            }

                            bb.AddCodeBlock( fencedCodeBlock, codeBlockFileName );
                        }
                    }

                    if ( File.Exists( razorFilename ) )
                    {
                        currentPageCode = File.ReadAllText( razorFilename );
                    }

                    builtPageCode = bb.ToString();

                    if ( currentPageCode != builtPageCode )
                    {
                        File.WriteAllText( razorFilename, bb.ToString() );
                    }
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( $"Error generating blogs {paths.SnippetsFilePath} : {e.Message}" );
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
        private string ParsePageInfo( BlogBuilder blogBuilder, string markdownText )
        {
            if ( markdownText.StartsWith( "---" ) )
            {
                var seoEnding = markdownText.IndexOf( "---", 3 );

                var pageInfo = markdownText.Substring( 3, seoEnding - 3 ).Trim().Split( '\n' );

                string title = null;
                string description = null;
                string permalink = null;

                foreach ( var line in pageInfo )
                {
                    if ( line.StartsWith( "title:" ) )
                        title = line.Substring( "title:".Length + 1 ).Trim();
                    else if ( line.StartsWith( "description:" ) )
                        description = line.Substring( "description:".Length + 1 ).Trim();
                    else if ( line.StartsWith( "permalink:" ) )
                        permalink = line.Substring( "permalink:".Length + 1 ).Trim();
                }

                blogBuilder.AddPageAndSeo( permalink, title, description );

                return markdownText.Substring( seoEnding + 3 ).TrimStart();
            }

            return markdownText;
        }
    }
}
