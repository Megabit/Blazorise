#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

internal sealed class DockLayoutContext
{
    #region Events

    internal event Action<DockLayoutChange> Changed;

    #endregion

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

    internal void NotifyChanged( DockLayoutChange change )
        => Changed?.Invoke( change );

    internal DockNodeState GetNode( string nodeId )
        => layout?.GetNode( nodeId );

    internal DockPaneState GetPaneState( string paneName )
        => layout?.GetPaneState( paneName );

    internal DockPanePosition GetPanePosition( DockPane pane )
        => layout?.GetPanePosition( pane ) ?? pane?.EffectivePosition ?? DockPanePosition.Center;

    internal DockPanePosition? GetDockNodePosition( DockNodeState node )
        => layout?.GetDockNodePosition( node );

    internal bool CanResizeDockNode( DockNodeState node )
        => layout?.CanResizeDockNode( node ) == true;

    internal DockPaneTabPosition GetDockNodeTabPosition( DockNodeState node, DockPanePosition position )
        => layout?.GetDockNodeTabPosition( node, position ) ?? DockPaneTabPosition.Top;

    internal string GetDockSplitStyle( DockNodeState node )
        => layout?.GetDockSplitStyle( node );

    internal bool IsDockPaneBordered( DockPanePosition position )
        => layout?.IsDockPaneBordered( position ) == true;

    internal string GetActiveTabPaneName( DockNodeState node )
        => layout?.GetActiveTabPaneName( node );

    internal string GetPaneCaption( string paneName )
        => layout?.GetPaneCaption( paneName ) ?? paneName;

    internal IReadOnlyList<DockRailItemState> GetRailItems( DockPanePosition position )
        => layout?.GetRailItems( position ) ?? [];

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

    internal Task BeginNodeResize( ElementReference elementRef, string paneName, string nodeId, DockPanePosition dock, PointerEventArgs eventArgs )
        => layout?.BeginNodeResize( elementRef, paneName, nodeId, dock, eventArgs ) ?? Task.CompletedTask;

    internal Task TogglePaneAutoHide( DockPane pane )
        => layout?.TogglePaneAutoHide( pane ) ?? Task.CompletedTask;

    internal Task ExpandPaneAutoHide( DockPane pane )
        => layout?.ExpandPaneAutoHide( pane ) ?? Task.CompletedTask;

    internal Task ClosePane( DockPane pane )
        => layout?.ClosePane( pane ) ?? Task.CompletedTask;

    internal Task ClosePane( string paneName )
        => layout?.ClosePane( paneName ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    internal DockContent Content => layout?.Content;

    internal DockZone? ActiveDockZone => layout?.ActiveDockZone;

    internal string ActiveDockCompassZoneKey => layout?.ActiveDockCompassZoneKey;

    internal double DockCompassX => layout?.DockCompassX ?? 0;

    internal double DockCompassY => layout?.DockCompassY ?? 0;

    #endregion
}