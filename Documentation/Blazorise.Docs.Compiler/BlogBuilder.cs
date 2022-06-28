using System.IO;
using System.Text;
using ColorCode;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Blazorise.Docs.Compiler
{
    public class BlogBuilder
    {
        private readonly StringBuilder sb;
        private const int IndentSize = 4;

        public BlogBuilder()
        {
            sb = new StringBuilder();
        }

        public void AddPageAndSeo( string url, string title, string description, string imageUrl, string imageTitle )
        {
            sb.Append( $"@page \"{url}\"" ).Append( '\n' ).Append( '\n' );

            sb.Append( $"<Seo Canonical=\"{url}\" Title=\"{title}\" Description=\"{description}\" />" ).Append( '\n' ).Append( '\n' );

            sb.Append( $"<BlogPageImage Source=\"{imageUrl}\" Text=\"{imageTitle}\" />" ).Append( '\n' ).Append( '\n' );
        }

        private void AddInlines( ContainerInline containerInline )
        {
            foreach ( var inline in containerInline )
            {
                if ( inline is EmphasisInline emphasisInline )
                {
                    if ( emphasisInline.DelimiterCount == 2 )
                        sb.Append( "<Strong>" ).Append( emphasisInline.FirstChild.ToString() ).Append( "</Strong>" );
                    else if ( emphasisInline.DelimiterCount == 1 )
                        sb.Append( "<Text Italic>" ).Append( emphasisInline.FirstChild.ToString() ).Append( "</Text>" );
                }
                else if ( inline is LinkInline linkInline )
                {
                    var title = string.IsNullOrEmpty( linkInline.Title ) ? linkInline.FirstChild?.ToString() : linkInline.Title;

                    if ( linkInline.IsImage )
                        sb.Append( $"<Image Source=\"{linkInline.Url}\" Text=\"{title}\">" ).Append( linkInline.FirstChild?.ToString() ).Append( "</Image>" );
                    else
                        sb.Append( $"<Anchor To=\"{linkInline.Url}\" Title=\"Link to {title}\">" ).Append( linkInline.FirstChild?.ToString() ).Append( "</Anchor>" );
                }
                else if ( inline is CodeInline codeInline )
                {
                    var content = codeInline.Content;

                    if ( content.StartsWith( '<' ) && content.EndsWith( '>' ) )
                    {
                        content = content.Trim( '<', '>' );

                        sb.Append( $"<Code Tag>" ).Append( content ).Append( "</Code>" );
                    }
                    else
                        sb.Append( $"<Code>" ).Append( content ).Append( "</Code>" );
                }
                else
                    sb.Append( inline.ToString() );
            }

            sb.Append( '\n' );
        }

        public void AddPageTitle( HeadingBlock headingBlock )
        {
            sb.Append( "<BlogPageTitle>" ).Append( '\n' );

            sb.Append( "".PadLeft( IndentSize, ' ' ) );

            if ( headingBlock.Inline != null )
                AddInlines( headingBlock.Inline );

            sb.Append( "</BlogPageTitle>" ).Append( '\n' ).Append( '\n' );
        }

        public void AddPageSubtitle( HeadingBlock headingBlock )
        {
            sb.Append( "<BlogPageSubtitle>" ).Append( '\n' );

            sb.Append( "".PadLeft( IndentSize, ' ' ) );

            if ( headingBlock.Inline != null )
                AddInlines( headingBlock.Inline );

            sb.Append( "</BlogPageSubtitle>" ).Append( '\n' ).Append( '\n' );
        }

        public void AddPageParagraph( ParagraphBlock paragraphBlock )
        {
            if ( paragraphBlock.Inline == null )
                return;

            if ( paragraphBlock.Inline.FirstChild is LinkInline linkInline && linkInline.IsImage )
            {
                var title = string.IsNullOrEmpty( linkInline.Title ) ? linkInline.FirstChild?.ToString() : linkInline.Title;

                sb.Append( $"<BlogPageImageModal ImageSource=\"{linkInline.Url}\" ImageTitle=\"{title}\" />" ).Append( '\n' ).Append( '\n' );
            }
            else
            {
                sb.Append( "<BlogPageParagraph>" ).Append( '\n' );

                sb.Append( "".PadLeft( IndentSize, ' ' ) );

                if ( paragraphBlock.Inline != null )
                    AddInlines( paragraphBlock.Inline );

                sb.Append( "</BlogPageParagraph>" ).Append( '\n' ).Append( '\n' );
            }
        }

        public void AddPageQuote( QuoteBlock quoteBlock )
        {
            foreach ( var block in quoteBlock )
            {
                if ( block is ParagraphBlock paragraphBlock )
                {
                    sb.Append( "<BlogPageParagraph>" ).Append( '\n' );

                    sb.Append( "".PadLeft( IndentSize, ' ' ) );

                    sb.Append( "<Blockquote>" ).Append( '\n' );

                    sb.Append( "".PadLeft( IndentSize * 2, ' ' ) );

                    if ( paragraphBlock.Inline != null )
                        AddInlines( paragraphBlock.Inline );

                    sb.Append( "".PadLeft( IndentSize, ' ' ) );

                    sb.Append( "</Blockquote>" ).Append( '\n' );

                    sb.Append( "</BlogPageParagraph>" ).Append( '\n' ).Append( '\n' );
                }
            }
        }

        public void AddPageList( ListBlock listBlock )
        {
            sb.Append( $"<BlogPageList Ordered=\"{listBlock.IsOrdered.ToString().ToLowerInvariant()}\">" ).Append( '\n' );

            foreach ( ListItemBlock listItem in listBlock )
            {
                sb.Append( "".PadLeft( IndentSize, ' ' ) );
                sb.Append( "<BlogPageListItem>" ).Append( '\n' );

                sb.Append( "".PadLeft( IndentSize * 2, ' ' ) );

                if ( listItem.Count > 0 && listItem[0] is ParagraphBlock paragraphBlock )
                {
                    AddInlines( paragraphBlock.Inline );
                }

                sb.Append( "".PadLeft( IndentSize, ' ' ) );
                sb.Append( "</BlogPageListItem>" ).Append( '\n' );
            }


            sb.Append( "</BlogPageList>" ).Append( '\n' ).Append( '\n' );
        }

        public string AddCodeBlock( FencedCodeBlock fencedCodeBlock, string codeBlockName )
        {
            var formatter = new HtmlClassFormatter();

            sb.Append( $"<BlogPageSourceBlock Code=\"{codeBlockName}" );

            var parsedCodeBlock = ParseCodeBlock( fencedCodeBlock );

            var builtCodeBlock = new MarkupBuilder( formatter )
                .Build( parsedCodeBlock, fencedCodeBlock.Info == "cs" || fencedCodeBlock.Info == "csharp" );

            sb.Append( "\"" );

            sb.Append( " />" ).Append( '\n' ).Append( '\n' );

            return builtCodeBlock;
        }

        static string ParseCodeBlock( FencedCodeBlock fencedCodeBlock )
        {
            var sb = new StringBuilder();

            foreach ( StringLine line in fencedCodeBlock.Lines )
            {
                sb.AppendLine( line.ToString() );
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
