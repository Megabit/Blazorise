#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a named data source available to report bands and fields.
/// </summary>
public partial class ReportDataSource : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterDataSource( new()
        {
            Name = Name,
            Type = Type,
            Data = Data,
            Settings = Settings,
            Schema = Schema,
        } );
    }

    /// <summary>
    /// Name used by report bands and fields to reference this data source.
    /// </summary>
    [Parameter] public string Name { get; set; } = "Default";

    /// <summary>
    /// Data source provider type used to resolve schema and runtime data.
    /// </summary>
    [Parameter] public string Type { get; set; } = ObjectReportDataSourceProvider.ProviderType;

    /// <summary>
    /// Object or enumerable exposed to the report designer and preview renderer, including nested objects and collections.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Provider-specific settings stored with the report definition.
    /// </summary>
    [Parameter] public Dictionary<string, object> Settings { get; set; } = [];

    /// <summary>
    /// Field schema exposed by providers that do not use a reflected object model.
    /// </summary>
    [Parameter] public ReportDataSourceSchema Schema { get; set; }
}