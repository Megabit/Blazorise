using Microsoft.AspNetCore.Components;

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
            Data = Data,
        } );
    }

    /// <summary>
    /// Name used by report bands and fields to reference this data source.
    /// </summary>
    [Parameter] public string Name { get; set; } = "Default";

    /// <summary>
    /// Object or enumerable exposed to the report designer and preview renderer, including nested objects and collections.
    /// </summary>
    [Parameter] public object Data { get; set; }
}