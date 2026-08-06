#region Using directives
using System;
using System.Net.Http;
using Blazorise.Reporting;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Reporting.DataSources.Csv;

/// <summary>
/// Provides service registration helpers for the CSV report data source provider.
/// </summary>
public static class Config
{
    #region Methods

    /// <summary>
    /// Registers the CSV report data source provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureHttpClientBuilder">Optional HTTP client configuration.</param>
    /// <param name="configureOptions">Optional CSV data source configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazoriseReportingCsvDataSource( this IServiceCollection services, Action<IHttpClientBuilder> configureHttpClientBuilder = null, Action<CsvReportDataSourceOptions> configureOptions = null )
    {
        CsvReportDataSourceOptions options = new();
        configureOptions?.Invoke( options );
        options.Validate();

        services.AddSingleton( options );

        IHttpClientBuilder httpClientBuilder = services.AddHttpClient( CsvReportDataSourceProvider.HttpClientName );

        if ( !OperatingSystem.IsBrowser() )
        {
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler( () => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
            } );
        }

        configureHttpClientBuilder?.Invoke( httpClientBuilder );
        services.AddReportDataSourceProvider<CsvReportDataSourceProvider>();

        return services;
    }

    #endregion
}