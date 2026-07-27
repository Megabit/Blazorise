#region Using directives
using System;
using Blazorise.Reporting;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Reporting.DataSources.Sql;

/// <summary>
/// Provides service registration helpers for the SQL report data source provider.
/// </summary>
public static class Config
{
    #region Methods

    /// <summary>
    /// Registers the SQL report data source provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">Optional SQL provider configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazoriseReportingSqlDataSource( this IServiceCollection services, Action<SqlReportDataSourceOptions> options = default )
    {
        SqlReportDataSourceOptions sqlOptions = new();

        options?.Invoke( sqlOptions );

        services.AddSingleton( sqlOptions );
        services.AddReportDataSourceProvider<SqlReportDataSourceProvider>();

        return services;
    }

    #endregion
}