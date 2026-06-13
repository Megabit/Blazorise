#region Using directives
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
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazoriseReportingCsvDataSource( this IServiceCollection services )
    {
        services.AddReportDataSourceProvider<CsvReportDataSourceProvider>();

        return services;
    }

    #endregion
}