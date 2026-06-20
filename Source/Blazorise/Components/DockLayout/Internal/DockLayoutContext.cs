#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

internal sealed record DockLayoutContext
{
    #region Members

    private readonly DockLayout layout;

    #endregion

    #region Constructors

    internal DockLayoutContext( DockLayout layout )
    {
        this.layout = layout;
    }

    #endregion

    #region Methods

    internal DockNodeState GetNode( string nodeId )
        => layout?.GetNode( nodeId );

    internal DockPaneState GetPaneState( string paneName )
        => layout?.GetPaneState( paneName );

    internal DockPanePosition GetPanePosition( DockPane pane )
        => layout?.GetPanePosition( pane ) ?? pane?.EffectivePosition ?? DockPanePosition.Center;

    internal DockPanePosition? GetDockNodePosition( DockNodeState node )
        => layout?.GetDockNodePosition( node );

    internal DockPaneTabsPlacement GetDockNodeTabsPlacement( DockNodeState node, DockPanePosition position )
        => layout?.GetDockNodeTabsPlacement( node, position ) ?? DockPaneTabsPlacement.Top;

    internal string GetDockSplitStyle( DockNodeState node )
        => layout?.GetDockSplitStyle( node );

    internal bool IsTabGroupAutoHidden( DockNodeState node )
        => layout?.IsTabGroupAutoHidden( node ) == true;

    internal bool IsDockPaneBordered( DockPanePosition position )
        => layout?.IsDockPaneBordered( position ) == true;

    internal string GetActiveTabPaneName( DockNodeState node )
        => layout?.GetActiveTabPaneName( node );

    internal string GetPaneCaption( string paneName )
        => layout?.GetPaneCaption( paneName ) ?? paneName;

    internal bool TryGetPane( string paneName, out DockPane pane )
    {
        if ( layout is not null )
            return layout.TryGetPane( paneName, out pane );

        pane = null;
        return false;
    }

    internal bool IsPaneTabCloseButtonVisible( string paneName, DockPanePosition position )
        => layout?.IsPaneTabCloseButtonVisible( paneName, position ) == true;

    internal Task ActivateTab( string nodeId, string paneName )
        => layout?.ActivateTab( nodeId, paneName ) ?? Task.CompletedTask;

    internal Task BeginPaneTabDrag( string paneName, PointerEventArgs eventArgs )
        => layout?.BeginPaneTabDrag( paneName, eventArgs ) ?? Task.CompletedTask;

    internal Task BeginPaneDrag( DockPane pane, PointerEventArgs eventArgs, bool dragGroup )
        => layout?.BeginPaneDrag( pane, eventArgs, dragGroup ) ?? Task.CompletedTask;

    internal Task BeginPaneResize( DockPane pane, string nodeId, DockPanePosition dock, PointerEventArgs eventArgs )
        => layout?.BeginPaneResize( pane, nodeId, dock, eventArgs ) ?? Task.CompletedTask;

    internal Task BeginNodeResize( ElementReference elementRef, string paneName, string nodeId, DockPanePosition dock, PointerEventArgs eventArgs, string minSize = null, string maxSize = null )
        => layout?.BeginNodeResize( elementRef, paneName, nodeId, dock, eventArgs, minSize, maxSize ) ?? Task.CompletedTask;

    internal Task TogglePaneAutoHide( DockPane pane )
        => layout?.TogglePaneAutoHide( pane ) ?? Task.CompletedTask;

    internal Task OpenPaneAutoHide( DockPane pane )
        => layout?.OpenPaneAutoHide( pane ) ?? Task.CompletedTask;

    internal Task ClosePane( DockPane pane )
        => layout?.ClosePane( pane ) ?? Task.CompletedTask;

    internal Task ClosePane( string paneName )
        => layout?.ClosePane( paneName ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    internal DockContent Content => layout?.Content;

    internal DockZone? ActiveDockZone => layout?.ActiveDockZone;

    internal string ActiveDockCompassZoneKey => layout?.ActiveDockCompassZoneKey;

    #endregion
}