#region Using directives
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Renders the report designer page tabs.
/// </summary>
public partial class ReportDesignerPageTabs
{
    #region Properties

    [CascadingParameter( Name = "ReportDesigner" )] internal _ReportDesigner Designer { get; set; }

    /// <summary>
    /// Gets the current page-tab state version.
    /// </summary>
    [Parameter] public int Version { get; set; }

    #endregion
}