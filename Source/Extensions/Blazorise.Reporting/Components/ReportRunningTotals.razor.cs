#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Groups declarative running total fields available to a report definition.
/// </summary>
public partial class ReportRunningTotals : ComponentBase
{
    #region Properties

    /// <summary>
    /// Running total field components declared for the parent report.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}