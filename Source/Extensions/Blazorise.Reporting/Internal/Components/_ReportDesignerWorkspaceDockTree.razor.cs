#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Defines the initial dock tree for the report designer workspace.
/// </summary>
public partial class _ReportDesignerWorkspaceDockTree
{
    #region Members

    private DockPane surfacePane;

    private DockPaneBody fieldsExplorerPaneBody;

    private DockPaneBody propertiesPaneBody;

    private DockPaneBody reportExplorerPaneBody;

    private DockPaneBody surfacePaneBody;

    private DockPaneBody toolboxPaneBody;

    #endregion

    #region Methods

    internal Task RefreshSurface()
        => surfacePane?.Refresh() ?? Task.CompletedTask;

    internal ElementReference? GetPaneBodyElement( string paneName )
    {
        DockPaneBody paneBody = ResolvePaneBody( paneName );

        return paneBody?.ElementRef;
    }

    private DockPaneBody ResolvePaneBody( string paneName )
    {
        if ( string.Equals( paneName, ToolboxPaneName, System.StringComparison.Ordinal ) )
            return toolboxPaneBody;

        if ( string.Equals( paneName, FieldsExplorerPaneName, System.StringComparison.Ordinal ) )
            return fieldsExplorerPaneBody;

        if ( string.Equals( paneName, SurfacePaneName, System.StringComparison.Ordinal ) )
            return surfacePaneBody;

        if ( string.Equals( paneName, PropertiesPaneName, System.StringComparison.Ordinal ) )
            return propertiesPaneBody;

        if ( string.Equals( paneName, ReportExplorerPaneName, System.StringComparison.Ordinal ) )
            return reportExplorerPaneBody;

        return null;
    }

    #endregion

    #region Properties

    private string SurfaceBodyClass
        => ShowRulers
            ? "b-report-designer-surface b-report-designer-surface-rulers"
            : "b-report-designer-surface";

    private IFluentSpacing SurfaceBodyPadding
        => ShowRulers
            ? Padding.Is0
            : Padding.Is3;

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
    /// Defines whether ruler chrome is visible around the designer surface.
    /// </summary>
    [Parameter] public bool ShowRulers { get; set; }

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