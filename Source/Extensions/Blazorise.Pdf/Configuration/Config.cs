#region Using directives
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides service registration helpers for Blazorise PDF generation.
/// </summary>
public static class Config
{
    #region Methods

    /// <summary>
    /// Adds the Blazorise PDF generation services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazorisePdf( this IServiceCollection services )
    {
        services.TryAddScoped<IPdfGenerator, PdfGenerator>();
        services.TryAddScoped<IPdfResourceResolver, PdfResourceResolver>();
        services.TryAddScoped<IPdfRenderProvider, SimplePdfRenderProvider>();

        return services;
    }

    /// <summary>
    /// Enables HTTP image and font resources for PDF generation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureHttpClientBuilder">Optional HTTP client builder configuration.</param>
    /// <param name="configureOptions">Optional resource policy configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazorisePdfHttpResources( this IServiceCollection services, Action<IHttpClientBuilder> configureHttpClientBuilder = null, Action<PdfHttpResourceOptions> configureOptions = null )
    {
        services.AddBlazorisePdf();

        PdfHttpResourceOptions options = new();
        configureOptions?.Invoke( options );
        options.Validate();

        services.AddSingleton( options );

        IHttpClientBuilder httpClientBuilder = services.AddHttpClient( HttpPdfResourceResolver.HttpClientName );

        if ( !OperatingSystem.IsBrowser() )
        {
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler( () => new HttpClientHandler
            {
                AllowAutoRedirect = false,
            } );
        }

        configureHttpClientBuilder?.Invoke( httpClientBuilder );

        services.Replace( ServiceDescriptor.Scoped<IPdfResourceResolver, HttpPdfResourceResolver>() );

        return services;
    }

    #endregion
}