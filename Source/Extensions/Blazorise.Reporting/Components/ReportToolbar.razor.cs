using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Declares custom toolbar content for the containing report.
/// </summary>
public partial class ReportToolbar : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterToolbar( ChildContent );
    }

    /// <summary>
    /// Toolbar items rendered by the report toolbar area.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}