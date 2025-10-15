using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Blazorise.Docs.BlogRuntime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Blazorise.Docs.Server.Infrastructure;

public class SeoGenerator
{
    public static async Task GenerateRobots( HttpContext context )
    {
        var baseUrl = GetBaseUrl( context );

        context.Response.ContentType = "text/plain";

        await context.Response.WriteAsync( $"User-agent: *\n" );
        await context.Response.WriteAsync( $"Disallow: \n\n" );

        await context.Response.WriteAsync( $"Sitemap: {baseUrl}/sitemap.txt" );
    }

    public static async Task GenerateSitemap( HttpContext context )
    {
        var pages = typeof( App ).Assembly.ExportedTypes.Where( p => p.IsSubclassOf( typeof( ComponentBase ) )
                                                                     && p.Namespace.StartsWith( "Blazorise.Docs.Pages" )
                                                                     && p.Namespace != "Blazorise.Docs.Docs.Examples" );

        var baseUrl = GetBaseUrl( context );

        foreach ( var page in pages )
        {
            if ( page.CustomAttributes != null )
            {
                foreach ( var routeAttribute in page.GetCustomAttributes<RouteAttribute>() )
                {
                    await context.Response.WriteAsync( $"{baseUrl}{routeAttribute.Template}\n" );
                }
            }
        }
    }

    public static async Task GenerateSitemapXml( HttpContext context )
    {
        var pages = typeof( App ).Assembly.ExportedTypes.Where( p => p.IsSubclassOf( typeof( ComponentBase ) )
                                                                     && p.Namespace.StartsWith( "Blazorise.Docs.Pages" )
                                                                     && p.Namespace != "Blazorise.Docs.Docs.Examples" );

        var baseUrl = GetBaseUrl( context );
        var urls = pages.Where( x => x.CustomAttributes is not null ).SelectMany( x => x.GetCustomAttributes<RouteAttribute>() ).Select( x => $"{baseUrl}{x.Template}" ).ToList();

        var sitemap = new XElement( "urlset",
                 new XAttribute( XNamespace.Xmlns + "x", "http://www.sitemaps.org/schemas/sitemap/0.9" ),
                 from url in urls
                 select new XElement( "url",
                     new XElement( "loc", url ),
                     new XElement( "lastmod", DateTime.UtcNow.ToString( "yyyy-MM-ddTHH:mm:ssZ" ) ) ) );

        await context.Response.WriteAsync( sitemap.ToString() );
    }

    // temprary class to hold page info
    // TODO: remove this once we have a proper blog system in place
    private class PageEntry
    {
        public string Title { get; set; }
        public string Permalink { get; set; }
        public string Summary { get; set; }
        public string PostedOn { get; set; }
    }

    public static async Task GenerateRssFeed( HttpContext context, IBlogProvider blogProvider )
    {
        var baseUrl = GetBaseUrl( context );
        var blogPages = ( await blogProvider.GetListAsync() ).Select( x => new PageEntry { Title = x.Title, Permalink = x.Permalink, PostedOn = x.PostedOn, Summary = x.Summary } ).ToList();
        var newsPages = Pages.News.Index.NewsEntries.Select( x => new PageEntry { Title = x.Text, Permalink = x.Url, PostedOn = x.PostedOn, Summary = x.Description } ).ToList();

        var pages = blogPages.Concat( newsPages ).ToList();

        var sitemap = new XElement( "rss",
                 new XAttribute( XNamespace.Xmlns + "atom", "http://www.w3.org/2005/Atom" ),
                 new XAttribute( "version", "2.0" ),
                 new XElement( "channel",
                     new XElement( "title", "Blazorise News" ),
                     new XElement( "link", baseUrl ),
                     new XElement( "description", "Blazorise News Feed" ),
                     new XElement( "language", "en" ),
                     new XElement( "lastBuildDate", DateTime.UtcNow.ToString( "R" ) ),
                     from p in pages
                     orderby p.PostedOn descending
                     select new XElement( "item",
                         new XElement( "title", p.Title ),
                         new XElement( "link", CombineUrl( baseUrl, p.Permalink ) ),
                         new XElement( "description", p.Summary ),
                         new XElement( "pubDate", DateTime.TryParse( p.PostedOn, CultureInfo.InvariantCulture, out var dt ) ? dt.ToString( "R" ) : null ) ) ) );

        await context.Response.WriteAsync( sitemap.ToString() );
    }

    private static string CombineUrl( string baseUrl, string permalink )
    {
        if ( string.IsNullOrWhiteSpace( permalink ) )
            return baseUrl;

        permalink = permalink.Trim();
        if ( permalink.StartsWith( "http", StringComparison.OrdinalIgnoreCase ) )
            return permalink;

        if ( !permalink.StartsWith( "/" ) )
            permalink = "/" + permalink;

        return baseUrl + permalink;
    }

    private static string GetBaseUrl( HttpContext context )
    {
        return $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
    }
}