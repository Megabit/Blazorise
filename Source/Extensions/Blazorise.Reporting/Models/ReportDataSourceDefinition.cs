#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a named data source exposed to report fields.
/// </summary>
public sealed class ReportDataSourceDefinition
{
    /// <summary>
    /// Stable identifier used by persisted report state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Data source name used by bands and field expressions.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Provider type used to resolve designer schema and runtime data.
    /// </summary>
    public string ProviderType { get; set; } = ObjectReportDataSourceProvider.ProviderType;

    /// <summary>
    /// Source object or enumerable used when resolving report fields and data source paths.
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// Provider-specific settings stored with the report definition.
    /// </summary>
    public Dictionary<string, object> Settings { get; set; } = [];

    /// <summary>
    /// Field schema exposed by providers that cannot be reflected from a live object.
    /// </summary>
    public ReportDataSourceSchema Schema { get; set; }
}