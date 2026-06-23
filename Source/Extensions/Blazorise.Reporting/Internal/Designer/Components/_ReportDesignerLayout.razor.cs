#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Reporting;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the dock-based report designer layout.
/// </summary>
public partial class _ReportDesignerLayout
{
    #region Members

    private const string PropertiesPaneName = "properties";

    private const string ReportExplorerPaneName = "report-explorer";

    private const string FieldsExplorerPaneName = "report-fields-explorer";

    private const string SurfacePaneName = "report-designer";

    private const string ToolbarPaneName = "report-toolbar";

    private const string ToolboxPaneName = "report-toolbox";

    private static readonly ReportToolbarDockPaneItem[] dockPaneOptions =
    [
        new( ToolboxPaneName, "Toolbox" ),
        new( FieldsExplorerPaneName, "Fields Explorer" ),
        new( PropertiesPaneName, "Properties" ),
        new( ReportExplorerPaneName, "Report Explorer" ),
    ];

    private readonly DockLayoutState dockLayoutState = new();

    private string activePanelPaneName = PropertiesPaneName;

    private bool? dockLayoutToolbarVisible;

    private DockLayout dockLayout;

    private ReportToolbarDockContext toolbarDockContext;

    private Div designerElement;

    private DotNetObjectReference<_ReportDesignerLayout> dotNetObjectReference;

    private JSReportingModule reportingModule;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        activePanelPaneName = ResolvePanelPaneName( SelectedPanelTab );

        EnsureDockLayoutState();

        ActivateSelectedPanelTab();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender && Shortcut.HasDelegate )
        {
            EnsureReportingModule();
            dotNetObjectReference ??= DotNetObjectReference.Create( this );

            await reportingModule.StartDesignerKeyboardShortcuts( designerElement.ElementRef, dotNetObjectReference );
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if ( reportingModule is not null )
        {
            try
            {
                if ( designerElement is not null )
                    await reportingModule.StopDesignerKeyboardShortcuts( designerElement.ElementRef );

                await reportingModule.DisposeAsync();
            }
            catch ( JSDisconnectedException )
            {
            }
        }

        dotNetObjectReference?.Dispose();
    }

    /// <summary>
    /// Receives a designer keyboard shortcut from the reporting JavaScript module.
    /// </summary>
    /// <param name="shortcut">Shortcut command name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnDesignerShortcut( string shortcut )
    {
        return Enum.TryParse( shortcut, out ReportDesignerShortcut designerShortcut )
            ? InvokeAsync( () => Shortcut.InvokeAsync( designerShortcut ) )
            : Task.CompletedTask;
    }

    private void ActivateSelectedPanelTab()
    {
        string paneName = ResolvePanelPaneName( SelectedPanelTab );

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return;

        DockNodeState tabsNode = FindTabsNode( dockLayoutState.Root, paneName );

        if ( tabsNode is not null && !string.Equals( tabsNode.ActivePane, paneName, StringComparison.Ordinal ) )
            tabsNode.ActivePane = paneName;
    }

    private async Task OnDockLayoutStateChanged( DockLayoutState state )
    {
        string selectedPanelTab = ResolveSelectedPanelTab( state );

        if ( !string.IsNullOrWhiteSpace( selectedPanelTab ) )
            activePanelPaneName = ResolvePanelPaneName( selectedPanelTab );

        if ( !string.IsNullOrWhiteSpace( selectedPanelTab )
             && !string.Equals( selectedPanelTab, SelectedPanelTab, StringComparison.Ordinal )
             && SelectedPanelTabChanged.HasDelegate )
        {
            await SelectedPanelTabChanged.InvokeAsync( selectedPanelTab );
        }
    }

    private string ResolveSelectedPanelTab( DockLayoutState state )
    {
        DockNodeState propertiesTabsNode = FindTabsNode( state?.Root, PropertiesPaneName );
        DockNodeState explorerTabsNode = FindTabsNode( state?.Root, ReportExplorerPaneName );

        if ( explorerTabsNode is not null && string.Equals( explorerTabsNode.ActivePane, ReportExplorerPaneName, StringComparison.Ordinal ) )
            return nameof( ReportDesignerPanelTab.Explorer );

        if ( propertiesTabsNode is not null && string.Equals( propertiesTabsNode.ActivePane, PropertiesPaneName, StringComparison.Ordinal ) )
            return nameof( ReportDesignerPanelTab.Properties );

        return null;
    }

    private static string ResolvePanelPaneName( string selectedPanelTab )
    {
        return string.Equals( selectedPanelTab, nameof( ReportDesignerPanelTab.Explorer ), StringComparison.Ordinal )
            ? ReportExplorerPaneName
            : PropertiesPaneName;
    }

    private static DockNodeState FindTabsNode( DockNodeState node, string paneName )
    {
        if ( node is null || string.IsNullOrWhiteSpace( paneName ) )
            return null;

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( paneName ) )
            return node;

        return FindTabsNode( node.First, paneName ) ?? FindTabsNode( node.Second, paneName );
    }

    private void EnsureDockLayoutState()
    {
        bool toolbarVisible = ShowToolbar && Toolbar is not null;

        if ( dockLayoutToolbarVisible == toolbarVisible
             && DockStateContainsPane( ToolboxPaneName )
             && DockStateContainsPane( FieldsExplorerPaneName )
             && DockStateContainsPane( SurfacePaneName )
             && DockStateContainsPane( PropertiesPaneName )
             && DockStateContainsPane( ReportExplorerPaneName )
             && ( !toolbarVisible || DockStateContainsPane( ToolbarPaneName ) ) )
        {
            return;
        }

        dockLayoutState.Root = toolbarVisible
            ? CreateSplitNode(
                "report-dock-root",
                CreatePaneNode( "report-dock-toolbar", ToolbarPaneName ),
                CreateWorkspaceNode(),
                DockSplitOrientation.Vertical,
                0.08d )
            : CreateWorkspaceNode();

        dockLayoutState.Panes.Clear();
        dockLayoutToolbarVisible = toolbarVisible;
    }

    private bool DockStateContainsPane( string paneName )
        => DockTreeContainsPane( dockLayoutState.Root, paneName )
            || DockRailContainsPane( dockLayoutState, paneName )
            || DockAutoHideContainsPane( dockLayoutState, paneName );

    private bool IsDockPaneOpen( string paneName )
        => dockLayout?.IsPaneOpen( paneName ) ?? DockStateContainsPane( paneName );

    private Task SetDockPaneOpen( string paneName, bool open )
    {
        if ( dockLayout is null )
            return Task.CompletedTask;

        return open
            ? dockLayout.OpenPane( paneName )
            : dockLayout.ClosePane( paneName );
    }

    private DockNodeState CreateWorkspaceNode()
    {
        return CreateSplitNode(
            "report-dock-workspace",
            CreateSplitNode(
                "report-dock-left-stack",
                CreatePaneNode( "report-dock-toolbox", ToolboxPaneName ),
                CreatePaneNode( "report-dock-fields-explorer", FieldsExplorerPaneName ),
                DockSplitOrientation.Vertical,
                0.34d ),
            CreateSplitNode(
                "report-dock-content",
                CreatePaneNode( "report-dock-surface", SurfacePaneName ),
                CreateTabsNode( "report-dock-right-tabs", activePanelPaneName, PropertiesPaneName, ReportExplorerPaneName ),
                DockSplitOrientation.Horizontal,
                0.76d ),
            DockSplitOrientation.Horizontal,
            0.24d );
    }

    private static DockNodeState CreatePaneNode( string id, string paneName )
    {
        return new()
        {
            Id = id,
            Kind = DockNodeKind.Pane,
            PaneName = paneName,
        };
    }

    private static DockNodeState CreateSplitNode( string id, DockNodeState first, DockNodeState second, DockSplitOrientation orientation, double ratio )
    {
        return new()
        {
            Id = id,
            Kind = DockNodeKind.Split,
            First = first,
            Second = second,
            Orientation = orientation,
            Ratio = ratio,
        };
    }

    private static DockNodeState CreateTabsNode( string id, string activePaneName, params string[] paneNames )
    {
        return new()
        {
            Id = id,
            Kind = DockNodeKind.Tabs,
            Panes = paneNames.ToList(),
            ActivePane = activePaneName,
        };
    }

    private static bool DockTreeContainsPane( DockNodeState node, string paneName )
    {
        if ( node is null || string.IsNullOrWhiteSpace( paneName ) )
            return false;

        return node.Kind switch
        {
            DockNodeKind.Pane => string.Equals( node.PaneName, paneName, StringComparison.Ordinal ),
            DockNodeKind.Tabs => node.Panes.Contains( paneName ),
            DockNodeKind.Split => DockTreeContainsPane( node.First, paneName ) || DockTreeContainsPane( node.Second, paneName ),
            _ => false,
        };
    }

    private static bool DockRailContainsPane( DockLayoutState state, string paneName )
        => state?.Rails?.Any( rail =>
            rail.Items.Any( item => string.Equals( item.PaneName, paneName, StringComparison.Ordinal ) ) ) == true;

    private static bool DockAutoHideContainsPane( DockLayoutState state, string paneName )
        => state?.Panes?.Any( paneState =>
            string.Equals( paneState.Name, paneName, StringComparison.Ordinal )
            && paneState.Visible
            && paneState.AutoHide ) == true;

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    #endregion

    #region Properties

    private ReportToolbarDockContext ToolbarDockContext
        => toolbarDockContext ??= new( dockPaneOptions, IsDockPaneOpen, SetDockPaneOpen );

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Content shown in the top designer toolbar pane.
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// Defines whether the top designer toolbar pane is visible.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Content shown in the left designer toolbox dock pane.
    /// </summary>
    [Parameter] public RenderFragment ToolboxPanel { get; set; }

    /// <summary>
    /// Content shown in the left designer fields explorer dock pane.
    /// </summary>
    [Parameter] public RenderFragment FieldsExplorerPanel { get; set; }

    /// <summary>
    /// Content shown in the central designer surface.
    /// </summary>
    [Parameter] public RenderFragment Surface { get; set; }

    /// <summary>
    /// Content shown in the right designer properties dock pane.
    /// </summary>
    [Parameter] public RenderFragment PropertiesPanel { get; set; }

    /// <summary>
    /// Content shown in the right designer report explorer dock pane.
    /// </summary>
    [Parameter] public RenderFragment ReportExplorerPanel { get; set; }

    /// <summary>
    /// Floating context menu shown above the designer layout.
    /// </summary>
    [Parameter] public RenderFragment ContextMenu { get; set; }

    /// <summary>
    /// Raised when a standard designer keyboard shortcut is pressed.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerShortcut> Shortcut { get; set; }

    /// <summary>
    /// Name of the selected right-side designer panel.
    /// </summary>
    [Parameter] public string SelectedPanelTab { get; set; }

    /// <summary>
    /// Raised when the selected right-side designer panel changes through dock tabs.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedPanelTabChanged { get; set; }

    #endregion
}