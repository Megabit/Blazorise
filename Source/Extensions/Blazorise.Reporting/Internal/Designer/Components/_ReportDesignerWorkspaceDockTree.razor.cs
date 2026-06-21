#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Defines the initial dock tree for the report designer workspace.
/// </summary>
public partial class _ReportDesignerWorkspaceDockTree
{
    #region Properties

    /// <summary>
    /// Name of the toolbox dock pane.
    /// </summary>
    [Parameter] public string ToolboxPaneName { get; set; }

    /// <summary>
    /// Name of the fields explorer dock pane.
    /// </summary>
    [Parameter] public string FieldsExplorerPaneName { get; set; }

    /// <summary>
    /// Name of the report designer surface dock pane.
    /// </summary>
    [Parameter] public string SurfacePaneName { get; set; }

    /// <summary>
    /// Name of the properties dock pane.
    /// </summary>
    [Parameter] public string PropertiesPaneName { get; set; }

    /// <summary>
    /// Name of the report explorer dock pane.
    /// </summary>
    [Parameter] public string ReportExplorerPaneName { get; set; }

    /// <summary>
    /// Name of the active right-side panel pane.
    /// </summary>
    [Parameter] public string ActivePanelPaneName { get; set; }

    /// <summary>
    /// Content shown in the toolbox dock pane.
    /// </summary>
    [Parameter] public RenderFragment ToolboxPanel { get; set; }

    /// <summary>
    /// Content shown in the fields explorer dock pane.
    /// </summary>
    [Parameter] public RenderFragment FieldsExplorerPanel { get; set; }

    /// <summary>
    /// Content shown in the central designer surface.
    /// </summary>
    [Parameter] public RenderFragment Surface { get; set; }

    /// <summary>
    /// Content shown in the right properties dock pane.
    /// </summary>
    [Parameter] public RenderFragment PropertiesPanel { get; set; }

    /// <summary>
    /// Content shown in the right report explorer dock pane.
    /// </summary>
    [Parameter] public RenderFragment ReportExplorerPanel { get; set; }

    #endregion
}