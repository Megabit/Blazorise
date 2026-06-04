using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportDataSource : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    protected override void OnParametersSet()
    {
        ReportContext?.RegisterDataSource( new()
        {
            Name = Name,
            Data = Data,
        } );
    }

    [Parameter] public string Name { get; set; } = "Default";

    [Parameter] public object Data { get; set; }
}