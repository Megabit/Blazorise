#region Using directives
using System;
using Blazorise.CodeEditor;
using Blazorise.Licensing;
using Blazorise.Pdf;
using Blazorise.Reporting.Internal;
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
        services.AddBlazoriseCodeEditor();
        services.AddBlazorisePdf();
        services.TryAddScoped<IReportDataSourceProviderRegistry, ReportDataSourceProviderRegistry>();
        services.TryAddScoped<IReportElementPluginRegistry, ReportElementPluginRegistry>();
        services.TryAddScoped<ReportDataSourceResolver>();
        services.TryAddScoped<IReportRenderer>( serviceProvider => new ReportRenderer(
            serviceProvider.GetRequiredService<ReportDataSourceResolver>(),
            serviceProvider.GetRequiredService<IReportElementPluginRegistry>(),
            serviceProvider.GetService<BlazoriseLicenseChecker>() ) );
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportDataSourceProvider, ObjectReportDataSourceProvider>() );
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportDataSourceProvider, DataSetReportDataSourceProvider>() );

        return services;
    }
}