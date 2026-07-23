#region Using directives
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides service registration helpers for report data source providers.
/// </summary>
public static class ReportDataSourceProviderServiceCollectionExtensions
{
    #region Methods

    /// <summary>
    /// Registers a report data source provider with the reporting provider registry.
    /// </summary>
    /// <typeparam name="TProvider">Provider implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddReportDataSourceProvider<TProvider>( this IServiceCollection services )
        where TProvider : class, IReportDataSourceProvider
    {
        services.TryAddScoped<IReportDataSourceProviderRegistry, ReportDataSourceProviderRegistry>();
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportDataSourceProvider, TProvider>() );

        return services;
    }

    #endregion
}