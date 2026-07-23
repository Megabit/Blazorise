#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Groups the declarative data sources available to a report definition.
/// </summary>
public partial class ReportDataSources : ComponentBase
{
    #region Properties

    /// <summary>
    /// Data source components declared for the parent report.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}