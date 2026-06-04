using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportToolbar : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    protected override void OnParametersSet()
    {
        ReportContext?.RegisterToolbar( ChildContent );
    }

    [Parameter] public RenderFragment ChildContent { get; set; }
}