#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Groups declarative formula fields available to a report definition.
/// </summary>
public partial class ReportFormulaFields : ComponentBase
{
    #region Properties

    /// <summary>
    /// Formula field components declared for the parent report.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}