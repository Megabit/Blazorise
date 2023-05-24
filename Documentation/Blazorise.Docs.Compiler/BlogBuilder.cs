using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using ColorCode;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Blazorise.Docs.Compiler;

public class BlogBuilder
{
    private readonly string blogName;
    private readonly string blogDirectory;
    private readonly StringBuilder sb;
    private const int IndentSize = 4;
    private string NewLine = "\r\n";
    private int codeIndex = 0;

    public BlogBuilder( string blogName, string blogDirectory )
    {
        this.blogName = blogName;
        this.blogDirectory = blogDirectory;

        sb = new StringBuilder();
    }

    public void AddPageAndSeo( string url, string title, string description, string imageUrl, string imageTitle )
    {
        sb.Append( $"@page \"{url}\"" ).Append( NewLine ).Append( NewLine );

        sb.Append( $"<Seo Canonical=\"{url}\" Title=\"{title}\" Description=\"{description}\" ImageUrl=\"{imageUrl}\" />" ).Append( NewLine ).Append( NewLine );

        sb.Append( $"<BlogPageImage Source=\"{imageUrl}\" Text=\"{imageTitle}\" />" ).Append( NewLine ).Append( NewLine );
    }

    public void AddPagePostInto( string authorName, string authorImage, string postedOn, string readTime )
    {
        sb.Append( $"<BlogPagePostInto UserName=\"{authorName}\" ImageName=\"{authorImage}\" PostedOn=\"{postedOn}\" Read=\"{readTime}\" />" ).Append( NewLine );
    }

    private void AddInlines( ContainerInline containerInline )
    {
        foreach ( var inline in containerInline )
        {
            if ( inline is EmphasisInline emphasisInline )
            {
                if ( emphasisInline.DelimiterCount == 2 )
                    sb.Append( "<Strong>" ).Append( string.Join( "", emphasisInline ) ).Append( "</Strong>" );
                else if ( emphasisInline.DelimiterCount == 1 )
                    sb.Append( "<Text Italic>" ).Append( string.Join( "", emphasisInline ) ).Append( "</Text>" );
            }
            else if ( inline is LinkInline linkInline )
            {
                var title = string.IsNullOrEmpty( linkInline.Title ) ? linkInline.FirstChild?.ToString() : linkInline.Title;

                if ( linkInline.IsImage )
                    sb.Append( $"<BlogPageImageModal ImageSource=\"{linkInline.Url}\" ImageTitle=\"{title}\" />" );
                //sb.Append( $"<Image Source=\"{linkInline.Url}\" Text=\"{title}\">" ).Append( linkInline.FirstChild?.ToString() ).Append( "</Image>" );
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
                {
                    content = content.Replace( "<", "&lt;" );
                    content = content.Replace( ">", "&gt;" );

                    sb.Append( $"<Code>" ).Append( content ).Append( "</Code>" );
                }
            }
            else
                sb.Append( inline.ToString() );
        }

        sb.Append( NewLine );
    }

    public void AddPageTitle( HeadingBlock headingBlock )
    {
        sb.Append( "<BlogPageTitle>" ).Append( NewLine );

        sb.Append( "".PadLeft( IndentSize, ' ' ) );

        if ( headingBlock.Inline != null )
            AddInlines( headingBlock.Inline );

        sb.Append( "</BlogPageTitle>" ).Append( NewLine ).Append( NewLine );
    }

    public void AddPageSubtitle( HeadingBlock headingBlock )
    {
        sb.Append( "<BlogPageSubtitle>" ).Append( NewLine );

        sb.Append( "".PadLeft( IndentSize, ' ' ) );

        if ( headingBlock.Inline != null )
            AddInlines( headingBlock.Inline );

        sb.Append( "</BlogPageSubtitle>" ).Append( NewLine ).Append( NewLine );
    }

    public void AddPageHeading( HeadingBlock headingBlock )
    {
        sb.Append( $"<Heading Size=\"HeadingSize.Is{headingBlock.Level}\">" ).Append( NewLine );

        sb.Append( "".PadLeft( IndentSize, ' ' ) );

        if ( headingBlock.Inline != null )
            AddInlines( headingBlock.Inline );

        sb.Append( "</Heading>" ).Append( NewLine ).Append( NewLine );
    }

    public void AddPageParagraph( ParagraphBlock paragraphBlock )
    {
        if ( paragraphBlock.Inline == null )
            return;

        if ( paragraphBlock.Inline.FirstChild is LinkInline linkInline && linkInline.IsImage )
        {
            var title = string.IsNullOrEmpty( linkInline.Title ) ? linkInline.FirstChild?.ToString() : linkInline.Title;

            sb.Append( $"<BlogPageImageModal ImageSource=\"{linkInline.Url}\" ImageTitle=\"{title}\" />" ).Append( NewLine ).Append( NewLine );
        }
        else
        {
            sb.Append( "<BlogPageParagraph>" ).Append( NewLine );

            sb.Append( "".PadLeft( IndentSize, ' ' ) );

            if ( paragraphBlock.Inline != null )
                AddInlines( paragraphBlock.Inline );

            sb.Append( "</BlogPageParagraph>" ).Append( NewLine ).Append( NewLine );
        }
    }

    public void AddPageQuote( QuoteBlock quoteBlock )
    {
        foreach ( var block in quoteBlock )
        {
            if ( block is ParagraphBlock paragraphBlock )
            {
                sb.Append( "<BlogPageParagraph>" ).Append( NewLine );

                sb.Append( "".PadLeft( IndentSize, ' ' ) );

                sb.Append( "<Blockquote>" ).Append( NewLine );

                sb.Append( "".PadLeft( IndentSize * 2, ' ' ) );

                if ( paragraphBlock.Inline != null )
                    AddInlines( paragraphBlock.Inline );

                sb.Append( "".PadLeft( IndentSize, ' ' ) );

                sb.Append( "</Blockquote>" ).Append( NewLine );

                sb.Append( "</BlogPageParagraph>" ).Append( NewLine ).Append( NewLine );
            }
        }
    }

    public void AddPageList( ListBlock listBlock )
    {
        sb.Append( $"<BlogPageList" );

        if ( listBlock.IsOrdered )
            sb.Append( " Ordered" );

        sb.Append( ">" ).Append( NewLine );

        foreach ( ListItemBlock listItem in listBlock )
        {
            sb.Append( "".PadLeft( IndentSize, ' ' ) );
            sb.Append( "<BlogPageListItem>" ).Append( NewLine );

            sb.Append( "".PadLeft( IndentSize * 2, ' ' ) );

            foreach ( var block in listItem )
            {
                if ( block is ParagraphBlock paragraphBlock )
                {
                    AddInlines( paragraphBlock.Inline );
                }
                else if ( block is FencedCodeBlock fencedCodeBlock )
                {
                    PersistCodeBlock( fencedCodeBlock, 2 );
                }
            }

            sb.Append( "".PadLeft( IndentSize, ' ' ) );
            sb.Append( "</BlogPageListItem>" ).Append( NewLine );
        }

        sb.Append( "</BlogPageList>" ).Append( NewLine ).Append( NewLine );
    }

    public (string builtCodeBlock, string parsedCodeBlock) AddCodeBlock( FencedCodeBlock fencedCodeBlock, string codeBlockName, int indentLevel )
    {
        var formatter = new HtmlClassFormatter();

        sb.Append( "".PadLeft( IndentSize * indentLevel, ' ' ) );

        sb.Append( $"<BlogPageSourceBlock Code=\"{codeBlockName}" );

        var parsedCodeBlock = ParseCodeBlock( fencedCodeBlock );

        var builtCodeBlock = new MarkupBuilder( formatter )
            .Build( parsedCodeBlock, ParseLanguage( fencedCodeBlock.Info ) );

        sb.Append( "\"" );

        sb.Append( " />" ).Append( NewLine );

        if ( indentLevel == 0 )
            sb.Append( NewLine );

        return (builtCodeBlock, parsedCodeBlock);
    }

    private static string ParseLanguage( string info )
    {
        if ( string.IsNullOrEmpty( info ) )
            return null;

        if ( info.StartsWith( "csharp" ) || info.StartsWith( "cs" ) )
            return "cs";
        else if ( info.StartsWith( "bash" ) )
            return "powershell";
        else if ( info.StartsWith( "html" ) )
            return "html";
        else if ( info.StartsWith( "css" ) )
            return "css";

        return null;
    }

    public void PersistCodeBlock( FencedCodeBlock fencedCodeBlock, int indentLevel )
    {
        var codeBlockName = ( fencedCodeBlock.Info != null && fencedCodeBlock.Info.IndexOf( '|' ) > 0
            ? $"{blogName}_{fencedCodeBlock.Info.Substring( fencedCodeBlock.Info.IndexOf( '|' ) + 1 )}"
            : $"{blogName}{( ++codeIndex )}" );

        var hasRazorFileExtension = codeBlockName.EndsWith( ".razor" );

        if ( hasRazorFileExtension )
            codeBlockName = codeBlockName.Replace( ".razor", "" );

        var codeBlockFileName = Path.Combine( blogDirectory, "Code", $"{codeBlockName}Code.html" );
        var codeBlockExampleFileName = Path.Combine( blogDirectory, "Examples", hasRazorFileExtension ? $"{codeBlockName}.razor" : $"{codeBlockName}.snippet" );
        var codeBlockDirectory = Path.GetDirectoryName( codeBlockFileName );
        var codeBlockExamplesDirectory = Path.GetDirectoryName( codeBlockExampleFileName );
        var currentCodeBlock = string.Empty;
        var currentCodeBlockExample = string.Empty;

        if ( !Directory.Exists( codeBlockDirectory ) )
        {
            Directory.CreateDirectory( codeBlockDirectory );
        }

        if ( !Directory.Exists( codeBlockExamplesDirectory ) )
        {
            Directory.CreateDirectory( codeBlockExamplesDirectory );
        }

        var (builtCodeBlock, parsedCodeBlock) = AddCodeBlock( fencedCodeBlock, codeBlockName, indentLevel );

        if ( File.Exists( codeBlockFileName ) )
        {
            currentCodeBlock = File.ReadAllText( codeBlockFileName );
        }

        if ( File.Exists( codeBlockExampleFileName ) )
        {
            currentCodeBlockExample = File.ReadAllText( codeBlockExampleFileName );
        }

        if ( currentCodeBlock != builtCodeBlock )
        {
            File.WriteAllText( codeBlockFileName, builtCodeBlock );
        }

        if ( currentCodeBlockExample != parsedCodeBlock )
        {
            File.WriteAllText( codeBlockExampleFileName, parsedCodeBlock );
        }
    }

    static string ParseCodeBlock( FencedCodeBlock fencedCodeBlock )
    {
        return string.Join( "\r\n", fencedCodeBlock.Lines );
    }

    public override string ToString()
    {
        return sb.ToString();
    }
}