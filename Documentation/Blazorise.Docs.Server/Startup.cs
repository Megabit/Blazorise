using System;
using System.IO.Compression;
using System.Linq;
using Blazored.LocalStorage;
using Blazorise.Bootstrap5;
using Blazorise.Captcha.ReCaptcha;
using Blazorise.Docs.Core;
using Blazorise.Docs.Models;
using Blazorise.Docs.Options;
using Blazorise.Docs.Server.Infrastructure;
using Blazorise.Docs.Services;
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

namespace Blazorise.Docs.Server;

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
        // Add services to the container.
        services
            .AddRazorComponents()
            .AddInteractiveServerComponents().AddHubOptions( options =>
            {
                options.MaximumReceiveMessageSize = 1024 * 1024 * 100;
            } );

        services.AddHttpContextAccessor();

        services
            .AddBlazorise( options =>
            {
                options.ProductToken = Configuration["Licensing:ProductToken"];
                options.Immediate = true; // optional
            } )
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            .AddBlazoriseRichTextEdit()
            .AddBlazoriseFluentValidation()
            .AddBlazoriseGoogleReCaptcha( x => x.SiteKey = Configuration[key: "ReCaptchaSiteKey"] );

        services.Configure<AppSettings>( options => Configuration.Bind( options ) );
        services.AddHttpClient();
        services.AddValidatorsFromAssembly( typeof( App ).Assembly );

        services.AddBlazoredLocalStorage();

        services.AddMemoryCache();
        services.AddScoped<Shared.Data.EmployeeData>();
        services.AddScoped<Shared.Data.CountryData>();
        services.AddScoped<Shared.Data.PageEntryData>();

        var emailOptions = Configuration.GetSection( "Email" ).Get<EmailOptions>();
        services.AddSingleton<IEmailOptions>( serviceProvider => emailOptions );

        services.AddSingleton<EmailSender>();

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
    public void Configure( WebApplication app )
    {
        if ( !app.Environment.IsDevelopment() )
        {
            app.UseResponseCompression();
        }

        if ( !app.Environment.IsDevelopment() )
        {
            app.UseExceptionHandler( "/Error" );
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        //app.UseRouting();

        app.MapGet( "/robots.txt", SeoGenerator.GenerateRobots );
        app.MapGet( "/sitemap.txt", SeoGenerator.GenerateSitemap );
        app.MapGet( "/sitemap.xml", SeoGenerator.GenerateSitemapXml );
        app.MapGet( "/feed.rss", SeoGenerator.GenerateRssFeed );
    }
}
