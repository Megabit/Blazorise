#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Defines runtime data used when rendering a persisted report definition.
/// </summary>
public sealed class ReportRenderOptions
{
    #region Properties

    /// <summary>
    /// Gets or sets fallback runtime data used by report expressions and a single object data source.
    /// </summary>
    public object DefaultData { get; set; }

    /// <summary>
    /// Gets or sets runtime data keyed by the data source names persisted in the report definition.
    /// </summary>
    public IDictionary<string, object> DataSources { get; set; } = new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );

    /// <summary>
    /// Gets or sets parameter values supplied to data source providers.
    /// </summary>
    public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );

    /// <summary>
    /// Gets or sets custom report element plugins available only to this render operation.
    /// </summary>
    public IEnumerable<IReportElementPlugin> ElementPlugins { get; set; }

    #endregion
}