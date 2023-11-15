using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
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

    private static string GetBaseUrl( HttpContext context )
    {
        return $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
    }
}