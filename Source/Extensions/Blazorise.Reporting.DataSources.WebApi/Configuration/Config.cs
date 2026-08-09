#region Using directives
using System;
using System.Net;
using System.Net.Http;
using Blazorise.Reporting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Provides service registration helpers for the Web API report data source provider.
/// </summary>
public static class Config
{
    #region Methods

    /// <summary>
    /// Registers the Web API report data source provider with built-in JSON and XML response readers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureHttpClientBuilder">Optional HTTP client and delegating-handler configuration. The provider controls the primary handler in server applications.</param>
    /// <param name="configureOptions">Optional request policy and resource-limit configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazoriseReportingWebApiDataSource( this IServiceCollection services, Action<IHttpClientBuilder> configureHttpClientBuilder = null, Action<WebApiReportDataSourceOptions> configureOptions = null )
    {
        WebApiReportDataSourceOptions options = new();
        configureOptions?.Invoke( options );
        options.Validate();

        services.AddSingleton( options );
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportWebApiResponseReader, JsonReportWebApiResponseReader>() );
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportWebApiResponseReader, XmlReportWebApiResponseReader>() );

        IHttpClientBuilder httpClientBuilder = services.AddHttpClient( WebApiReportDataSourceProvider.HttpClientName );
        configureHttpClientBuilder?.Invoke( httpClientBuilder );

        if ( !OperatingSystem.IsBrowser() )
        {
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler( () => new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All,
                UseCookies = false,
                UseProxy = false,
                ConnectCallback = WebApiPublicNetworkGuard.ConnectAsync,
            } );
        }

        services.AddReportDataSourceProvider<WebApiReportDataSourceProvider>();

        return services;
    }

    /// <summary>
    /// Registers a custom Web API response reader.
    /// </summary>
    /// <typeparam name="TReader">Response reader implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazoriseReportingWebApiResponseReader<TReader>( this IServiceCollection services )
        where TReader : class, IReportWebApiResponseReader
    {
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportWebApiResponseReader, TReader>() );

        return services;
    }

    #endregion
}