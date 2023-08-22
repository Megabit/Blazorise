using System;
using System.IO.Compression;
using System.Linq;
using Blazorise.Bootstrap5;
using Blazorise.Docs.Models;
using Blazorise.Docs.Options;
using Blazorise.Docs.Server.Infrastructure;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blazorise.Docs.Server
{
    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddServerSideBlazor().AddHubOptions( ( o ) =>
            {
                o.MaximumReceiveMessageSize = 1024 * 1024 * 100;
            } );

            services.AddHttpContextAccessor();

            services
              .AddBlazorise( options =>
              {
                  options.Immediate = true; // optional
              } )
              .AddBootstrap5Providers()
              .AddFontAwesomeIcons()
              .AddBlazoriseRichTextEdit()
              .AddBlazoriseFluentValidation();

            services.AddValidatorsFromAssembly( typeof( App ).Assembly );

            services.AddMemoryCache();
            services.AddScoped<Shared.Data.EmployeeData>();
            services.AddScoped<Shared.Data.CountryData>();
            services.AddScoped<Shared.Data.PageEntryData>();

            services.AddResponseCompression( options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            } );

            services.Configure<BrotliCompressionProviderOptions>( options =>
            {
                options.Level = CompressionLevel.Fastest;
            } );

            services.Configure<GzipCompressionProviderOptions>( options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            } );

            services.AddHsts( options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays( 365 );
            } );

            services.AddSingleton( new DocsVersionOptions
            {
                Versions = Configuration.GetSection( "DocsVersions" ).Get<DocsVersion[]>().ToList()
            } );
            
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            app.UseResponseCompression();

            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler( "/Error" );
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapGet( "/robots.txt", async context =>
                {
                    await Seo.GenerateRobots( context );
                } );

                endpoints.MapGet( "/sitemap.txt", async context =>
                {
                    await Seo.GenerateSitemap( context );
                } );

                endpoints.MapHealthChecks( "/healthcheck" );

                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage( "/_Host" );
            } );
        }
    }
}
