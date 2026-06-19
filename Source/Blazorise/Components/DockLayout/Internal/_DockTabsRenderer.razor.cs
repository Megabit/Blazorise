#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a tabbed dock pane group inside a dock layout tree.
/// </summary>
public partial class _DockTabsRenderer : BaseComponent
{
    #region Constructors

    /// <summary>
    /// Default <see cref="_DockTabsRenderer"/> constructor.
    /// </summary>
    public _DockTabsRenderer()
    {
        TabsClassBuilder = new( BuildTabsClasses );
    }

    #endregion

    #region Members

    private string activePaneName;

    private DockPane activePane;

    private DockPaneState activePaneState;

    private DockPanePosition groupPosition;

    private DockPaneTabsPlacement tabsPlacement;

    #endregion

    #region Methods

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        activePaneName = Layout?.GetActiveTabPaneName( Node );

        if ( string.IsNullOrWhiteSpace( activePaneName ) )
        {
            activePane = null;
            activePaneState = null;
            groupPosition = DockPanePosition.Center;
            tabsPlacement = DockPaneTabsPlacement.Top;
            return;
        }

        if ( Layout is null || !Layout.TryGetPane( activePaneName, out activePane ) )
            activePane = null;

        activePaneState = Layout?.GetPaneState( activePaneName );
        groupPosition = Layout?.GetDockNodePosition( Node ) ?? activePane?.EffectivePosition ?? DockPanePosition.Center;
        tabsPlacement = Layout?.GetDockNodeTabsPlacement( Node, groupPosition ) ?? DockPaneTabsPlacement.Top;

        DirtyClasses();
        DirtyStyles();
        TabsClassBuilder.Dirty();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ActivePane is not null )
        {
            builder.Append( ClassProvider.DockPane( GroupPosition, CanResize, Collapsed ) );
            builder.Append( ClassProvider.DockPanePosition( GroupPosition ) );
            builder.Append( ClassProvider.DockPaneResizable( CanResize ) );
            builder.Append( ClassProvider.DockPaneCollapsed( Collapsed ) );
            builder.Append( ClassProvider.DockPaneAutoHide( AutoHide ) );
            builder.Append( ClassProvider.DockPaneBordered(), Bordered );
            builder.Append( ClassProvider.DockPaneTabsHost() );
        }

        base.BuildClasses( builder );
    }

    private void BuildTabsClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneTabs() );
        builder.Append( ClassProvider.DockPaneTabsPosition( GroupPosition ) );
        builder.Append( ClassProvider.DockPaneTabsPlacement( TabsPlacement ) );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( ActivePane is not null )
        {
            builder.Append( $"--dock-pane-size:{PaneSize}", !AutoHide && GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"--dock-pane-min-size:{ActivePane.MinSize}", !AutoHide && GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( ActivePane.MinSize ) );
            builder.Append( $"--dock-pane-max-size:{ActivePane.MaxSize}", !AutoHide && GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( ActivePane.MaxSize ) );
        }

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    private string ActivePaneName => activePaneName;

    private DockPane ActivePane => activePane;

    private bool Visible => Node is not null && ActivePane is not null && activePaneState?.Visible != false;

    private bool AutoHide => Layout?.IsTabGroupAutoHidden( Node ) == true;

    private string TabsClassNames => TabsClassBuilder.Class;

    private bool TabsVisible => Node?.Panes?.Count > 1 || ActivePane?.EffectiveShowTab == true;

    private DockPanePosition GroupPosition => groupPosition;

    private DockPaneTabsPlacement TabsPlacement => tabsPlacement;

    private bool TabsOnTop => TabsPlacement == DockPaneTabsPlacement.Top;

    private bool Collapsed => activePaneState?.Collapsed == true || AutoHide;

    private string PaneSize => Node?.Size ?? activePaneState?.Size ?? ActivePane?.Size;

    private bool CanResize => ActivePane?.Resizable == true && SplitterDock is not null;

    private bool Bordered => Layout?.IsDockPaneBordered( GroupPosition ) == true;

    private ElementReference ElementRef
    {
        get => default;
        set
        {
            if ( ActivePane is not null )
                ActivePane.ElementRef = value;
        }
    }

    protected ClassBuilder TabsClassBuilder { get; private set; }

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the tab node to render.
    /// </summary>
    [Parameter] public DockNodeState Node { get; set; }

    /// <summary>
    /// Gets or sets the local splitter side for the rendered tab group.
    /// </summary>
    [Parameter] public DockPanePosition? SplitterDock { get; set; }

    /// <summary>
    /// Gets or sets the split node that owns the rendered tab group splitter.
    /// </summary>
    [Parameter] public string SplitNodeId { get; set; }

    #endregion
}