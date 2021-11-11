using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blazorise.Demo.Bootstrap.Host
{
    public class Program
    {
        public static void Main( string[] args )
        {
            var builder = WebApplication.CreateBuilder( args );

            builder.Services.AddRazorPages();
            builder.Services
                .SetupDemoServices()
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            var app = builder.Build();

            if ( !app.Environment.IsDevelopment() )
            {
                app.UseExceptionHandler( "/Error" );
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage( "/_Host" );
            } );

            app.Run();
        }
    }
}