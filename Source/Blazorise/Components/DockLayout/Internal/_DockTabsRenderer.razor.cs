#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a tabbed dock pane group inside a dock layout tree.
/// </summary>
public partial class _DockTabsRenderer : _BaseDockRenderer
{
    #region Members

    private string activePaneName;

    private DockPane activePane;

    private DockPaneState activePaneState;

    private DockPanePosition groupPosition;

    private DockPaneTabPosition tabPosition;

    private int version;

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="_DockTabsRenderer"/> constructor.
    /// </summary>
    public _DockTabsRenderer()
    {
        TabsClassBuilder = new( BuildTabsClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        RefreshState();
    }

    private void RefreshState()
    {

        activePaneName = Context?.GetActiveTabPaneName( Node );

        if ( string.IsNullOrWhiteSpace( activePaneName ) )
        {
            activePane = null;
            activePaneState = null;
            groupPosition = DockPanePosition.Center;
            tabPosition = DockPaneTabPosition.Top;
        }

        else
        {
            if ( Context is null || !Context.TryGetPane( activePaneName, out activePane ) )
                activePane = null;

            activePaneState = Context?.GetPaneState( activePaneName );
            groupPosition = Context?.GetDockNodePosition( Node ) ?? activePane?.EffectivePosition ?? DockPanePosition.Center;
            tabPosition = Context?.GetDockNodeTabPosition( Node, groupPosition ) ?? DockPaneTabPosition.Top;
        }
    }

    /// <inheritdoc/>
    private protected override bool IsAffected( DockLayoutChange change )
        => change.Kind == DockLayoutChangeKind.Pane && Node?.Panes?.Contains( change.PaneName ) == true;

    /// <inheritdoc/>
    private protected override void OnDockLayoutChanged( DockLayoutChange change )
    {
        RefreshState();
        DirtyClasses();
        DirtyStyles();
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
            builder.Append( ClassProvider.DockPaneBordered(), Bordered );
            builder.Append( ClassProvider.DockPaneTabsHost() );
        }

        base.BuildClasses( builder );
    }

    private void BuildTabsClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneTabs() );
        builder.Append( ClassProvider.DockPaneTabsPosition( GroupPosition ) );
        builder.Append( ClassProvider.DockPaneTabPosition( TabPosition ) );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        TabsClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( ActivePane is not null )
        {
            builder.Append( $"--dock-pane-size:{PaneSize}", GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"--dock-pane-min-size:{ActivePane.MinSize}", GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( ActivePane.MinSize ) );
            builder.Append( $"--dock-pane-max-size:{ActivePane.MaxSize}", GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( ActivePane.MaxSize ) );
        }

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    private string ActivePaneName => activePaneName;

    private DockPane ActivePane => activePane;

    private bool Visible => Node is not null && ActivePane is not null && activePaneState?.Visible != false && activePaneState?.AutoHide != true;

    private string TabsClassNames => TabsClassBuilder.Class;

    private bool TabsVisible => Node?.Panes?.Count > 1 || ActivePane?.EffectiveShowTab == true;

    private DockPanePosition GroupPosition => groupPosition;

    private DockPaneTabPosition TabPosition => tabPosition;

    private bool TabsOnTop => TabPosition == DockPaneTabPosition.Top;

    private bool Collapsed => activePaneState?.Collapsed == true;

    private string PaneSize => Node?.Size ?? activePaneState?.Size ?? ActivePane?.Size;

    private bool CanResize => ActivePane?.Resizable == true && SplitterDock is not null;

    private bool Bordered => Context?.IsDockPaneBordered( GroupPosition ) == true;

    private ElementReference CapturedElementRef
    {
        get => default;
        set
        {
            if ( ActivePane is not null )
                ActivePane.ElementRef = value;
        }
    }

    private ClassBuilder TabsClassBuilder { get; set; }

    private DockNodeState Node => Context?.GetNode( NodeId );

    /// <summary>
    /// Gets or sets the tab node id to render.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Gets or sets the local splitter side for the rendered tab group.
    /// </summary>
    [Parameter] public DockPanePosition? SplitterDock { get; set; }

    /// <summary>
    /// Gets or sets the split node that owns the rendered tab group splitter.
    /// </summary>
    [Parameter] public string SplitNodeId { get; set; }

    /// <summary>
    /// Gets or sets the node render version.
    /// </summary>
    [Parameter]
    public int Version
    {
        get => version;
        set
        {
            if ( version == value )
                return;

            version = value;
            RefreshState();
            DirtyClasses();
            DirtyStyles();
        }
    }

    #endregion
}