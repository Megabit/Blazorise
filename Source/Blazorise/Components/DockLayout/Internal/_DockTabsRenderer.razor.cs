#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a tabbed dock pane group inside a dock layout tree.
/// </summary>
public partial class _DockTabsRenderer : BaseComponent
{
    #region Members

    private string activePaneName;

    private DockPane activePane;

    private DockPaneState activePaneState;

    private DockPanePosition groupPosition;

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
            return;
        }

        if ( Layout is null || !Layout.TryGetPane( activePaneName, out activePane ) )
            activePane = null;

        activePaneState = Layout?.GetPaneState( activePaneName );
        groupPosition = Layout?.GetDockNodePosition( Node ) ?? activePane?.EffectivePosition ?? DockPanePosition.Center;

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
            builder.Append( ClassProvider.DockPaneAutoHide( AutoHide ) );
            builder.Append( "dock-tabs-host" );
        }

        base.BuildClasses( builder );
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

    private Task BeginPaneTabDrag( string paneName, PointerEventArgs eventArgs )
        => Layout?.BeginPaneTabDrag( paneName, eventArgs ) ?? Task.CompletedTask;

    private Task ActivateTab( string paneName )
        => Layout?.ActivateTab( Node, paneName ) ?? Task.CompletedTask;

    private Task ClosePane( string paneName )
        => Layout?.ClosePane( paneName ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private string ActivePaneName => activePaneName;

    private DockPane ActivePane => activePane;

    private bool Visible => Node is not null && ActivePane is not null && activePaneState?.Visible != false;

    private bool AutoHide => Layout?.IsTabGroupAutoHidden( Node ) == true;

    private string TabsClass => TabsOnTop ? $"{Layout?.GetDockPaneTabsClass()} dock-pane-tabs-top" : Layout?.GetDockPaneTabsClass();

    private bool TabsVisible => Node?.Panes?.Count > 1 || ActivePane?.EffectiveShowTab == true;

    private DockPanePosition GroupPosition => groupPosition;

    private bool IsPaneClosable( string paneName )
        => Layout?.IsPaneClosable( paneName ) == true;

    private bool TabsOnTop => GroupPosition == DockPanePosition.Center;

    private bool Collapsed => activePaneState?.Collapsed == true || AutoHide;

    private string PaneSize => Node?.Size ?? activePaneState?.Size ?? ActivePane?.Size;

    private bool CanResize => ActivePane?.Resizable == true && SplitterDock is not null;

    private ElementReference ElementRef
    {
        get => default;
        set
        {
            if ( ActivePane is not null )
                ActivePane.ElementRef = value;
        }
    }

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