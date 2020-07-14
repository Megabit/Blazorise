using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using System.Reflection;
using Microsoft.AspNetCore.Components.Server;

namespace Blazorise.Demo.Bootstrap.RC
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddBlazorise( options =>
                {
                    options.ChangeTextOnKeyPress = true;
                } )
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddServerSideBlazor().AddHubOptions( ( o ) =>
            {
                o.MaximumReceiveMessageSize = 1024 * 1024 * 100;
            } );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            // this is required to be here or otherwise the messages between server and client will be too large and
            // the connection will be lost.
            //app.UseSignalR( route => route.MapHub<ComponentHub>( ComponentHub.DefaultPath, o =>
            //{
            //    o.ApplicationMaxBufferSize = 1024 * 1024 * 100; // larger size
            //    o.TransportMaxBufferSize = 1024 * 1024 * 100; // larger size
            //} ) );

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage( "/_Host" );
            } );
        }
    }
}
