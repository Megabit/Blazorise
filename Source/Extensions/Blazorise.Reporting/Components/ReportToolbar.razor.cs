#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares custom toolbar content for the containing report.
/// </summary>
public partial class ReportToolbar : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterToolbar( ChildContent );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Toolbar items rendered by the report toolbar area.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}