using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ColorCode;
using Markdig;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Blazorise.Docs.Compiler
{
    public class BlogBuilder
    {
        private readonly StringBuilder sb;
        private int indentLevel;

        public BlogBuilder()
        {
            sb = new StringBuilder();
            indentLevel = 0;
        }

        public void AddPageSeo( string url, string title, string description )
        {
            sb.Append( $"@page \"{url}\"" ).Append( '\n' ).Append( '\n' );

            sb.Append( $"<Seo Canonical=\"{url}\" Title=\"{title}\" Description=\"{description}\" />" ).Append( '\n' ).Append( '\n' );
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
                else
                    sb.Append( inline.ToString() );
            }

            sb.Append( '\n' );
        }

        public void AddPageTitle( HeadingBlock headingBlock )
        {
            sb.Append( "<BlogPageTitle>" ).Append( '\n' );

            sb.Append( "".PadLeft( 4, ' ' ) );

            if ( headingBlock.Inline != null )
                AddInlines( headingBlock.Inline );

            sb.Append( "</BlogPageTitle>" ).Append( '\n' ).Append( '\n' );
        }

        public void AddPageSubtitle( HeadingBlock headingBlock )
        {
            sb.Append( "<BlogPageSubtitle>" ).Append( '\n' );

            sb.Append( "".PadLeft( 4, ' ' ) );

            if ( headingBlock.Inline != null )
                AddInlines( headingBlock.Inline );

            sb.Append( "</BlogPageSubtitle>" ).Append( '\n' ).Append( '\n' );
        }

        public void AddPageParagraph( ParagraphBlock paragraphBlock )
        {
            sb.Append( "<BlogPageParagraph>" ).Append( '\n' );

            sb.Append( "".PadLeft( 4, ' ' ) );

            if ( paragraphBlock.Inline != null )
                AddInlines( paragraphBlock.Inline );

            sb.Append( "</BlogPageParagraph>" ).Append( '\n' ).Append( '\n' );
        }

        public void AddCodeBlock( FencedCodeBlock fencedCodeBlock )
        {
            var formatter = new HtmlClassFormatter();

            sb.Append( "<BlogPageSourceBlock Code=\"" );

            var parsedCodeBlock = ParseCodeBlock( fencedCodeBlock );
            var builtCodeBlock = new MarkupBuilder( formatter ).Build( parsedCodeBlock, false );

            sb.Append( "\"" );

            sb.Append( " />" ).Append( '\n' ).Append( '\n' );
        }

        string ParseCodeBlock( FencedCodeBlock fencedCodeBlock )
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
