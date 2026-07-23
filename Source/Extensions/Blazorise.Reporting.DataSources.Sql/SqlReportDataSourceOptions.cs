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
    #region Properties

    /// <summary>
    /// Named connection factories that can be referenced by report data source definitions.
    /// </summary>
    public Dictionary<string, Func<IServiceProvider, DbConnection>> Connections { get; } = new( StringComparer.OrdinalIgnoreCase );

    #endregion
}