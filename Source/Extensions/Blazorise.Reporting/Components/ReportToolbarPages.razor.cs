#region Using directives
using Blazorise.Reporting.Internal;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Renders page selection and page creation controls in a custom report toolbar.
/// </summary>
public partial class ReportToolbarPages
{
    #region Properties

    [CascadingParameter( Name = "ReportToolbarDesigner" )] internal _ReportDesigner Designer { get; set; }

    [CascadingParameter( Name = "ReportToolbarShowPageControls" )] internal bool ShowPageControls { get; set; }

    #endregion
}