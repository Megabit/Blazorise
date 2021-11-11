using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Blazorise.Docs.Server.Infrastructure
{
    public class Seo
    {
        public static async Task GenerateRobots( HttpContext context )
        {
            var baseUrl = GetBaseUrl( context );

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

        private static string GetBaseUrl( HttpContext context )
        {
            return $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";
        }
    }
}
