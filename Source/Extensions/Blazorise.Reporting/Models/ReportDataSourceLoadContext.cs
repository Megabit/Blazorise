#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides renderer context to data source providers when data is loaded.
/// </summary>
public sealed class ReportDataSourceLoadContext
{
    #region Properties

    /// <summary>
    /// Default data object supplied directly to the report component.
    /// </summary>
    public object DefaultData { get; set; }

    /// <summary>
    /// Parameter values available to provider-specific data loading.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = [];

    #endregion
}