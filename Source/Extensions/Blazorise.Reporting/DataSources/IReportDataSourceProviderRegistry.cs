#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Exposes the report data source providers registered by the host application.
/// </summary>
public interface IReportDataSourceProviderRegistry
{
    #region Methods

    /// <summary>
    /// Finds a provider by its stored provider type.
    /// </summary>
    /// <param name="type">The provider type.</param>
    /// <returns>The matching provider, or null when the provider is not registered.</returns>
    IReportDataSourceProvider FindProvider( string type );

    #endregion

    #region Properties

    /// <summary>
    /// Providers available to the report designer.
    /// </summary>
    IReadOnlyList<IReportDataSourceProvider> Providers { get; }

    #endregion
}