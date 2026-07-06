#region Using directives
using System;
using Blazorise.Pdf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides service registration helpers for Blazorise Reporting.
/// </summary>
public static class Config
{
    /// <summary>
    /// Adds the Blazorise Reporting extension related services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The reporting options configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBlazoriseReporting( this IServiceCollection services, Action<ReportOptions> options = default )
    {
        ReportOptions reportOptions = new();

        options?.Invoke( reportOptions );

        services.AddSingleton( reportOptions );
        services.AddBlazorisePdf();
        services.TryAddScoped<IReportDataSourceProviderRegistry, ReportDataSourceProviderRegistry>();
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportDataSourceProvider, ObjectReportDataSourceProvider>() );
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportDataSourceProvider, DataSetReportDataSourceProvider>() );

        return services;
    }
}