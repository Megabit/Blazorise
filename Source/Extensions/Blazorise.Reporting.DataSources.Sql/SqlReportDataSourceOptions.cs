#region Using directives
using System;
using System.Collections.Generic;
using System.Data.Common;
#endregion

namespace Blazorise.Reporting.DataSources.Sql;

/// <summary>
/// Configures SQL report data source connections available to report definitions.
/// </summary>
public sealed class SqlReportDataSourceOptions
{
    #region Members

    internal const int DefaultMaximumCommandTimeout = 30;

    #endregion

    #region Methods

    internal void Validate()
    {
        if ( MaximumCommandTimeout <= 0 )
            throw new ArgumentOutOfRangeException( nameof( MaximumCommandTimeout ), "The maximum SQL report command timeout must be greater than zero seconds." );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Named connection factories displayed by the designer and referenced by report data source definitions.
    /// </summary>
    public Dictionary<string, Func<IServiceProvider, DbConnection>> Connections { get; } = new( StringComparer.OrdinalIgnoreCase );

    /// <summary>
    /// Determines whether a report-supplied query may execute against a named connection. Queries are denied when no policy is configured.
    /// </summary>
    /// <remarks>
    /// Prefer an exact allowlist of application-owned queries. Do not authorize queries by checking only for a leading SQL keyword.
    /// An application restricted to trusted SQL authors may deliberately allow every query for a registered connection and rely on a dedicated read-only database identity.
    /// </remarks>
    public Func<string, string, bool> QueryAllowed { get; set; }

    /// <summary>
    /// Maximum command timeout, in seconds, that a report definition may request. Queries without a timeout use this value.
    /// </summary>
    public int MaximumCommandTimeout { get; set; } = DefaultMaximumCommandTimeout;

    #endregion
}