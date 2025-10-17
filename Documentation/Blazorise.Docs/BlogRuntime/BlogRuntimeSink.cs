using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Docs.Components;
using ColorCode;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Newtonsoft.Json.Linq;

namespace Blazorise.Docs.BlogRuntime;

// Same surface as your builder but returning a RenderFragment instead of text
internal interface IBlogSink<TOut>
{
    void AddPageAndSeo( string url, string title, string desc, string imageUrl, string imageTitle );
    void AddPagePostInfo( string authorName, string authorImage, string postedOn, string readTime );
    void AddPageTitle( HeadingBlock h1 );
    void AddPageSubtitle( HeadingBlock h2 );
    void AddPageHeading( HeadingBlock hN );
    void AddPageLead( ParagraphBlock p );
    void AddPageParagraph( ParagraphBlock p );
    void AddPageQuote( QuoteBlock q );
    void AddPageList( ListBlock list );
    void PersistCodeBlock( FencedCodeBlock code, int indentLevel );
    TOut Build();
}

internal sealed class BlogRuntimeSink : IBlogSink<RenderFragment>
{
    private readonly List<Action<RenderTreeBuilder>> ops = new();
    private int codeIndex = 0;
    private readonly string blogName;
    private readonly Func<string, string> rewriteImageUrl;
    private static readonly HtmlClassFormatter htmlClassFormatter = new();
    private static readonly MarkupBuilder markupBuilder = new( htmlClassFormatter );

    public BlogRuntimeSink( string blogName, Func<string, string> rewriteImageUrl )
    {
        this.blogName = blogName;
        this.rewriteImageUrl = rewriteImageUrl;
    }

    public void AddPageAndSeo( string url, string title, string description, string imageUrl, string imageTitle )
    {
        ops.Add( b =>
        {
            b.OpenComponent( 0, typeof( Seo ) );
            b.AddAttribute( 1, nameof( Seo.Canonical ), url );
            b.AddAttribute( 2, nameof( Seo.Title ), title );
            b.AddAttribute( 3, nameof( Seo.Description ), description );
            b.AddAttribute( 4, nameof( Seo.ImageUrl ), imageUrl );
            b.CloseComponent();

            b.OpenComponent( 5, typeof( BlogPageImage ) );
            b.AddAttribute( 6, nameof( BlogPageImage.Source ), imageUrl );
            b.AddAttribute( 7, nameof( BlogPageImage.Text ), imageTitle );
            b.CloseComponent();
        } );
    }

    public void AddPagePostInfo( string authorName, string authorImage, string postedOn, string readTime )
    {
        ops.Add( b =>
        {
            b.OpenComponent( 10, typeof( BlogPagePostInto ) );
            b.AddAttribute( 11, nameof( BlogPagePostInto.UserName ), authorName );
            b.AddAttribute( 12, nameof( BlogPagePostInto.ImageName ), authorImage );
            b.AddAttribute( 13, nameof( BlogPagePostInto.PostedOn ), postedOn );
            b.AddAttribute( 14, nameof( BlogPagePostInto.Read ), readTime );
            b.CloseComponent();
        } );
    }

    public void AddPageTitle( HeadingBlock h ) => ops.Add( b => RenderSimpleBlock( b, typeof( BlogPageTitle ), h.Inline ) );
    public void AddPageSubtitle( HeadingBlock h ) => ops.Add( b => RenderSimpleBlock( b, typeof( BlogPageSubtitle ), h.Inline ) );

    public void AddPageHeading( HeadingBlock h )
    {
        ops.Add( b =>
        {
            b.OpenComponent( 20, typeof( Heading ) );
            b.AddAttribute( 21, nameof( Heading.Size ), Enum.Parse( typeof( HeadingSize ), $"Is{h.Level}" ) );
            b.AddAttribute( 22, nameof( Heading.ChildContent ), (RenderFragment)( bb => RenderInlines( bb, h.Inline ) ) );
            b.CloseComponent();
        } );
    }

    public void AddPageLead( ParagraphBlock p )
    {
        ops.Add( b =>
        {
            b.OpenComponent( 20, typeof( Lead ) );
            b.AddAttribute( 22, nameof( Lead.ChildContent ), (RenderFragment)( bb => RenderInlines( bb, p.Inline ) ) );
            b.CloseComponent();
        } );
    }

    public void AddPageParagraph( ParagraphBlock p )
    {
        if ( p.Inline is null )
            return;

        if ( p.Inline.FirstChild is LinkInline li && li.IsImage )
        {
            var title = string.IsNullOrEmpty( li.Title ) ? li.FirstChild?.ToString() : li.Title;
            var imageSource = rewriteImageUrl( li.Url );
            ops.Add( b =>
            {
                b.OpenComponent( 30, typeof( BlogPageImageModal ) );
                b.AddAttribute( 31, nameof( BlogPageImageModal.ImageSource ), imageSource );
                b.AddAttribute( 32, nameof( BlogPageImageModal.ImageTitle ), title );
                b.CloseComponent();
            } );
            return;
        }

        ops.Add( b =>
        {
            b.OpenComponent( 40, typeof( BlogPageParagraph ) );
            b.AddAttribute( 41, nameof( BlogPageParagraph.ChildContent ), (RenderFragment)( bb => RenderInlines( bb, p.Inline ) ) );
            b.CloseComponent();
        } );
    }

    public void AddPageQuote( QuoteBlock q )
    {
        foreach ( var block in q )
        {
            if ( block is ParagraphBlock p )
            {
                ops.Add( b =>
                {
                    b.OpenComponent( 50, typeof( BlogPageParagraph ) );
                    b.AddAttribute( 51, nameof( BlogPageParagraph.ChildContent ), (RenderFragment)( bb =>
                    {
                        bb.OpenElement( 52, "blockquote" );
                        RenderInlines( bb, p.Inline );
                        bb.CloseElement();
                    } ) );
                    b.CloseComponent();
                } );
            }
        }
    }

    public void AddPageList( ListBlock list )
    {
        ops.Add( b =>
        {
            b.OpenComponent( 60, typeof( BlogPageList ) );
            if ( list.IsOrdered )
                b.AddAttribute( 61, nameof( BlogPageList.Ordered ), true );
            b.AddAttribute( 62, nameof( BlogPageList.ChildContent ), (RenderFragment)( bb =>
            {
                foreach ( ListItemBlock item in list )
                {
                    bb.OpenComponent( 63, typeof( BlogPageListItem ) );
                    bb.AddAttribute( 64, nameof( BlogPageListItem.ChildContent ), (RenderFragment)( bbb =>
                    {
                        foreach ( var child in item )
                        {
                            if ( child is ParagraphBlock p )
                                RenderInlines( bbb, p.Inline );
                            else if ( child is FencedCodeBlock code )
                                PersistCodeBlockInternal( bbb, code );
                        }
                    } ) );
                    bb.CloseComponent();
                }
            } ) );
            b.CloseComponent();
        } );
    }

    public void PersistCodeBlock( FencedCodeBlock code, int _ ) =>
        ops.Add( b => PersistCodeBlockInternal( b, code ) );

    public RenderFragment Build() => b => { foreach ( var op in ops ) op( b ); };

    // helpers
    private static void RenderSimpleBlock( RenderTreeBuilder b, Type component, ContainerInline? inline )
    {
        b.OpenComponent( 100, component );
        b.AddAttribute( 101, "ChildContent", (RenderFragment)( bb => RenderInlines( bb, inline ) ) );
        b.CloseComponent();
    }

    private static void RenderInlines( RenderTreeBuilder b, ContainerInline inline, Func<string, string> rewriteImageUrl = null )
    {
        if ( inline is null )
            return;

        foreach ( var child in inline )
        {
            switch ( child )
            {
                case EmphasisInline em when em.DelimiterCount == 2:
                    b.OpenElement( 110, "strong" );
                    b.AddContent( 111, string.Join( "", em ) );
                    b.CloseElement();
                    break;

                case EmphasisInline em1 when em1.DelimiterCount == 1:
                    b.OpenElement( 112, "span" );
                    b.AddAttribute( 113, "class", "fst-italic" );
                    b.AddContent( 114, string.Join( "", em1 ) );
                    b.CloseElement();
                    break;

                case LinkInline link when !link.IsImage:
                    b.OpenComponent( 120, typeof( Anchor ) );
                    b.AddAttribute( 121, nameof( Anchor.To ), link.Url );
                    var title = string.IsNullOrEmpty( link.Title ) ? link.FirstChild?.ToString() : link.Title;
                    b.AddAttribute( 122, nameof( Anchor.Title ), $"Link to {title}" );
                    b.AddAttribute( 123, nameof( Anchor.ChildContent ), (RenderFragment)( bb => bb.AddContent( 124, link.FirstChild?.ToString() ) ) );
                    b.CloseComponent();
                    break;

                case LinkInline img when img.IsImage:
                    var t = string.IsNullOrEmpty( img.Title ) ? img.FirstChild?.ToString() : img.Title;
                    var imageSource = rewriteImageUrl is null ? img.Url : rewriteImageUrl( img.Url );
                    b.OpenComponent( 130, typeof( BlogPageImageModal ) );
                    b.AddAttribute( 131, nameof( BlogPageImageModal.ImageSource ), imageSource );
                    b.AddAttribute( 132, nameof( BlogPageImageModal.ImageTitle ), t );
                    b.CloseComponent();
                    break;

                case CodeInline ci:
                    var content = ci.Content;
                    var isTag = content.StartsWith( '<' ) && content.EndsWith( '>' );
                    b.OpenComponent( 140, typeof( Code ) );
                    if ( isTag )
                        b.AddAttribute( 141, nameof( Code.Tag ), true );
                    b.AddAttribute( 142, nameof( Code.ChildContent ), (RenderFragment)( bb => bb.AddMarkupContent( 143,
                        isTag ? content.Trim( '<', '>' ) : content.Replace( "<", "&lt;" ).Replace( ">", "&gt;" ) ) ) );
                    b.CloseComponent();
                    break;

                default:
                    b.AddContent( 150, child.ToString() );
                    break;
            }
        }
    }

    private void RenderInlines( RenderTreeBuilder b, ContainerInline inline )
        => RenderInlines( b, inline, rewriteImageUrl );

    private void PersistCodeBlockInternal( RenderTreeBuilder b, FencedCodeBlock code )
    {
        // Preserve your naming scheme so legacy "Code" lookup keeps working if needed
        var info = code.Info ?? string.Empty;
        var codeName = ( info.Contains( '|' )
            ? $"{blogName}_{info[( info.IndexOf( '|' ) + 1 )..]}"
            : $"{blogName}{++codeIndex}" ).Replace( ".razor", "" );

        // Extract the raw code text from the fenced block
        var raw = code.Lines.ToString();

        // Map Markdig info to the language tokens your MarkupBuilder expects ("cs","css","powershell", or default HTML/Razor handling)
        var lang = ParseLanguage( info );

        // Build fully formatted HTML (same as your previous compile-time generator)
        var html = markupBuilder.Build( raw, lang );

        // Render BlogPageSourceBlock with inner HTML
        b.OpenComponent( 0, typeof( BlogPageSourceBlock ) );
        //b.AddAttribute( 1, "Code", codeName );
        b.AddAttribute( 2, nameof( BlogPageSourceBlock.ChildContent ), (RenderFragment)( bb =>
        {
            bb.AddMarkupContent( 0, html );
        } ) );
        b.CloseComponent();
    }

    private static string ParseLanguage( string info )
    {
        if ( string.IsNullOrWhiteSpace( info ) )
            return ""; // triggers HTML/Razor path in MarkupBuilder

        var t = info.Split( new[] { ' ', '|', '\t' }, StringSplitOptions.RemoveEmptyEntries )
                    .FirstOrDefault()?.ToLowerInvariant() ?? "";

        // Match your MarkupBuilder’s cases
        return t switch
        {
            "csharp" or "cs" => "cs",
            "css" => "css",
            "powershell" or "ps" or "ps1" => "powershell",
            // anything else (html, razor, etc.) falls into the "else" branch in MarkupBuilder
            _ => ""
        };
    }
}