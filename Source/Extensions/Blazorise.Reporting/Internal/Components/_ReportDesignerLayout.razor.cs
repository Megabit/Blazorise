#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

    private static readonly string[] scrollablePaneNames =
    [
        ToolboxPaneName,
        FieldsExplorerPaneName,
        SurfacePaneName,
        PropertiesPaneName,
        ReportExplorerPaneName,
    ];

    private string activePanelPaneName = PropertiesPaneName;

    private bool? dockLayoutToolbarVisible;

    private DockLayout dockLayout;

    private DockPane toolbarPane;

    private _ReportDesignerWorkspaceDockTree workspaceDockTree;

    private ReportToolbarDockContext toolbarDockContext;

    private Div designerElement;

    private DotNetObjectReference<_ReportDesignerLayout> dotNetObjectReference;

    private JSReportingModule reportingModule;

    private int restoredPaneScrollVersion = -1;

    private ReportDesignerRefreshState renderedRefreshState;

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
        if ( firstRender && Shortcut is not null )
        {
            EnsureReportingModule();
            dotNetObjectReference ??= DotNetObjectReference.Create( this );

            await DocumentObserver.EnsureInitializedAsync();
            await reportingModule.StartDesignerKeyboardShortcuts( designerElement.ElementRef, dotNetObjectReference );
        }

        if ( RefreshState.Toolbar != renderedRefreshState.Toolbar )
        {
            if ( ShowToolbar && toolbarPane is not null )
                await toolbarPane.Refresh();
        }

        renderedRefreshState = RefreshState;

        if ( PaneScrollRestoreVersion != restoredPaneScrollVersion && PaneScrollPositions?.Count > 0 )
        {
            restoredPaneScrollVersion = PaneScrollRestoreVersion;
            await RestorePaneScrollPositions( PaneScrollPositions );
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
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

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Receives a designer keyboard shortcut from the reporting JavaScript module.
    /// </summary>
    /// <param name="shortcut">Shortcut command name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnDesignerShortcut( string shortcut )
    {
        if ( !Enum.TryParse( shortcut, out ReportDesignerShortcut designerShortcut ) || Shortcut is null )
            return Task.CompletedTask;

        return InvokeAsync( () => Shortcut.Invoke( designerShortcut ) );
    }

    private void ActivateSelectedPanelTab()
    {
        string paneName = ResolvePanelPaneName( SelectedPanelTab );

        if ( string.IsNullOrWhiteSpace( paneName ) )
            return;

        DockNodeState tabsNode = FindTabsNode( State.Root, paneName );

        if ( tabsNode is not null
             && tabsNode.Panes.Contains( PropertiesPaneName )
             && tabsNode.Panes.Contains( ReportExplorerPaneName )
             && !string.Equals( tabsNode.ActivePane, paneName, StringComparison.Ordinal ) )
        {
            tabsNode.ActivePane = paneName;
        }
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

        bool propertiesDocked = DockTreeContainsPane( state?.Root, PropertiesPaneName );
        bool reportExplorerDocked = DockTreeContainsPane( state?.Root, ReportExplorerPaneName );

        if ( reportExplorerDocked && !propertiesDocked )
            return nameof( ReportDesignerPanelTab.Explorer );

        if ( propertiesDocked && !reportExplorerDocked )
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

        if ( dockLayoutToolbarVisible is null )
            dockLayoutToolbarVisible = DockStateContainsPane( ToolbarPaneName );

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

        State.Root = toolbarVisible
            ? CreateSplitNode(
                "report-dock-root",
                CreatePaneNode( "report-dock-toolbar", ToolbarPaneName ),
                CreateWorkspaceNode(),
                DockSplitOrientation.Vertical,
                0.08d )
            : CreateWorkspaceNode();

        State.Panes.Clear();
        dockLayoutToolbarVisible = toolbarVisible;
    }

    private bool DockStateContainsPane( string paneName )
        => DockTreeContainsPane( State.Root, paneName )
            || DockRailContainsPane( State, paneName )
            || DockAutoHideContainsPane( State, paneName )
            || DockPaneStateContainsPane( State, paneName );

    private Task ShowDockPane( string paneName )
        => dockLayout?.ShowPane( paneName ) ?? Task.CompletedTask;

    internal Task ShowPropertiesPane()
        => ShowDockPane( PropertiesPaneName );

    internal Task RefreshSurface()
        => workspaceDockTree?.RefreshSurface() ?? Task.CompletedTask;

    internal async Task CapturePaneScrollPositions( Dictionary<string, ( double Left, double Top )> scrollPositions )
    {
        if ( scrollPositions is null || workspaceDockTree is null )
            return;

        EnsureReportingModule();

        foreach ( string paneName in scrollablePaneNames )
        {
            ElementReference? element = workspaceDockTree.GetPaneBodyElement( paneName );

            if ( element is null )
                continue;

            double[] position = await reportingModule.GetScrollPosition( element.Value );

            if ( position is not { Length: >= 2 } )
                continue;

            scrollPositions[paneName] = ( position[0], position[1] );
        }
    }

    internal async Task RestorePaneScrollPositions( IReadOnlyDictionary<string, ( double Left, double Top )> scrollPositions )
    {
        if ( scrollPositions is null || scrollPositions.Count == 0 || workspaceDockTree is null )
            return;

        EnsureReportingModule();

        foreach ( string paneName in scrollablePaneNames )
        {
            if ( !scrollPositions.TryGetValue( paneName, out ( double Left, double Top ) position ) )
                continue;

            ElementReference? element = workspaceDockTree.GetPaneBodyElement( paneName );

            if ( element is null )
                continue;

            await reportingModule.SetScrollPosition( element.Value, position.Left, position.Top );
        }
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

    private static bool DockPaneStateContainsPane( DockLayoutState state, string paneName )
        => state?.Panes?.Any( paneState =>
            string.Equals( paneState.Name, paneName, StringComparison.Ordinal ) ) == true;

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    #endregion

    #region Properties

    private ReportToolbarDockContext ToolbarDockContext
        => toolbarDockContext ??= new( dockPaneOptions, ShowDockPane );

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    [Inject] private IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Docking state preserved by the parent report across designer component lifetimes.
    /// </summary>
    [Parameter] public DockLayoutState State { get; set; } = new();

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
    /// Saved scroll positions for designer dock panes.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, ( double Left, double Top )> PaneScrollPositions { get; set; }

    /// <summary>
    /// Version used to request a one-time pane scroll restoration.
    /// </summary>
    [Parameter] public int PaneScrollRestoreVersion { get; set; }

    /// <summary>
    /// Targeted pane refresh state applied after pane content has rendered.
    /// </summary>
    [Parameter] public ReportDesignerRefreshState RefreshState { get; set; }

    /// <summary>
    /// Raised when a standard designer keyboard shortcut is pressed.
    /// </summary>
    [Parameter] public Func<ReportDesignerShortcut, Task> Shortcut { get; set; }

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