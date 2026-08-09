#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Default report data source provider registry backed by dependency injection.
/// </summary>
public sealed class ReportDataSourceProviderRegistry : IReportDataSourceProviderRegistry
{
    #region Members

    private readonly IReadOnlyList<IReportDataSourceProvider> providers;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new data source provider registry.
    /// </summary>
    /// <param name="providers">Providers registered by the host application.</param>
    public ReportDataSourceProviderRegistry( IEnumerable<IReportDataSourceProvider> providers )
    {
        this.providers = providers?.ToList() ?? [];
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public IReportDataSourceProvider FindProvider( string type )
    {
        if ( string.IsNullOrWhiteSpace( type ) )
            type = ObjectReportDataSourceProvider.ProviderType;

        return providers.FirstOrDefault( provider => string.Equals( provider.Type, type, StringComparison.OrdinalIgnoreCase ) );
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public IReadOnlyList<IReportDataSourceProvider> Providers => providers;

    #endregion
}