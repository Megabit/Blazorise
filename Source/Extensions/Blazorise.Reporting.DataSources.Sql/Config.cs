#region Using directives
using System;
using System.Runtime.Versioning;
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
    /// <exception cref="PlatformNotSupportedException">Thrown when the provider is registered in a browser runtime.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the configured maximum command timeout is not positive.</exception>
    [UnsupportedOSPlatform( "browser" )]
    public static IServiceCollection AddBlazoriseReportingSqlDataSource( this IServiceCollection services, Action<SqlReportDataSourceOptions> options = default )
    {
        if ( OperatingSystem.IsBrowser() )
            throw new PlatformNotSupportedException( "The SQL report data source executes database commands directly and can only be registered in a server application. Load authorized data through a server API in Blazor WebAssembly." );

        SqlReportDataSourceOptions sqlOptions = new();

        options?.Invoke( sqlOptions );
        sqlOptions.Validate();

        services.AddSingleton( sqlOptions );
        services.AddReportDataSourceProvider<SqlReportDataSourceProvider>();

        return services;
    }

    #endregion
}