#region Using directives
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides service registration helpers for report element plugins.
/// </summary>
public static class ReportElementPluginServiceCollectionExtensions
{
    #region Methods

    /// <summary>
    /// Registers a custom report element plugin with the report element plugin registry.
    /// </summary>
    /// <typeparam name="TPlugin">Plugin implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddReportElementPlugin<TPlugin>( this IServiceCollection services )
        where TPlugin : class, IReportElementPlugin
    {
        services.TryAddScoped<IReportElementPluginRegistry, ReportElementPluginRegistry>();
        services.TryAddEnumerable( ServiceDescriptor.Scoped<IReportElementPlugin, TPlugin>() );

        return services;
    }

    #endregion
}