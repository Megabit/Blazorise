#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

internal sealed class DockLayoutTreeQuery
{
    #region Members

    private readonly DockLayoutRegistry registry;

    private readonly DockLayoutStateManager stateManager;

    private readonly Func<DockLayoutState> getState;

    private readonly Func<int> getStateVersion;

    private readonly Dictionary<DockNodeState, DockPanePosition?> dockNodePositionCache = new();

    private int dockNodePositionCacheVersion = -1;

    #endregion

    #region Constructors

    public DockLayoutTreeQuery( DockLayoutRegistry registry, DockLayoutStateManager stateManager, Func<DockLayoutState> getState, Func<int> getStateVersion )
    {
        this.registry = registry;
        this.stateManager = stateManager;
        this.getState = getState;
        this.getStateVersion = getStateVersion;
    }

    #endregion

    #region Methods

    public DockNodeState GetNode( string nodeId )
        => string.IsNullOrWhiteSpace( nodeId ) ? null : FindNodeById( getState().Root, nodeId );

    public DockPanePosition GetPanePosition( DockPane pane )
        => FindPanePosition( getState().Root, pane.ResolvedName )
            ?? stateManager.FindPaneState( getState(), pane.ResolvedName )?.Position
            ?? GetInitialPanePosition( pane );

    public DockPanePosition? GetPanePosition( string paneName )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
            return null;

        if ( registry.TryGetPane( paneName, out DockPane pane ) )
            return GetPanePosition( pane );

        return stateManager.FindPaneState( getState(), paneName )?.Position;
    }

    public DockPanePosition? GetDockNodePosition( DockNodeState node )
    {
        if ( node is null )
            return null;

        // Resolving a node position walks the entire subtree, and it is requested multiple times
        // per node during a single render pass, so the results are memoized until the layout state
        // version changes or the caches are explicitly invalidated after a state mutation.
        int stateVersion = getStateVersion();

        if ( stateVersion != dockNodePositionCacheVersion )
        {
            dockNodePositionCache.Clear();
            dockNodePositionCacheVersion = stateVersion;
        }

        if ( dockNodePositionCache.TryGetValue( node, out DockPanePosition? position ) )
            return position;

        position = ComputeDockNodePosition( node );
        dockNodePositionCache[node] = position;

        return position;
    }

    public void InvalidateCaches()
    {
        dockNodePositionCache.Clear();
        dockNodePositionCacheVersion = -1;
    }

    private DockPanePosition? ComputeDockNodePosition( DockNodeState node )
    {
        if ( node.Kind == DockNodeKind.Pane )
            return GetPanePosition( node.PaneName );

        if ( node.Kind == DockNodeKind.Tabs )
        {
            foreach ( string paneName in node.Panes )
            {
                DockPanePosition? position = GetPanePosition( paneName );

                if ( position == DockPanePosition.Center )
                    return DockPanePosition.Center;
            }

            return GetPanePosition( GetActiveTabPaneName( node ) );
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            DockPanePosition? firstPosition = GetDockNodePosition( node.First );
            DockPanePosition? secondPosition = GetDockNodePosition( node.Second );

            if ( firstPosition == DockPanePosition.Center || secondPosition == DockPanePosition.Center )
                return DockPanePosition.Center;

            if ( firstPosition is not null && firstPosition == secondPosition )
                return firstPosition;
        }

        return null;
    }

    public bool CanResizeDockNode( DockNodeState node )
    {
        DockPanePosition? position = GetDockNodePosition( node );

        if ( position is null || position == DockPanePosition.Center )
            return false;

        string paneName = GetFirstDockNodePaneName( node );

        return registry.TryGetPane( paneName, out DockPane pane ) && pane.Resizable;
    }

    public string GetActiveTabPaneName( DockNodeState node )
    {
        if ( node is null || node.Kind != DockNodeKind.Tabs )
            return null;

        if ( !string.IsNullOrWhiteSpace( node.ActivePane ) && node.Panes.Contains( node.ActivePane ) )
            return node.ActivePane;

        return node.Panes.Count > 0 ? node.Panes[0] : null;
    }

    public DockNodeState FindTabsNode( string paneName )
        => FindTabsNode( getState().Root, paneName );

    public static DockNodeState FindTabsNode( DockNodeState node, string paneName )
    {
        if ( node is null || string.IsNullOrWhiteSpace( paneName ) )
            return null;

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( paneName ) )
            return node;

        if ( node.Kind == DockNodeKind.Split )
            return FindTabsNode( node.First, paneName ) ?? FindTabsNode( node.Second, paneName );

        return null;
    }

    public string GetFirstDockNodePaneName( DockNodeState node )
        => node?.Kind switch
        {
            DockNodeKind.Pane => node.PaneName,
            DockNodeKind.Tabs => GetActiveTabPaneName( node ) ?? node.Panes.FirstOrDefault(),
            DockNodeKind.Split => GetFirstDockNodePaneName( node.First ) ?? GetFirstDockNodePaneName( node.Second ),
            _ => null,
        };

    public DockNodePlacement FindPanePlacement( string paneName )
        => FindPanePlacement( getState().Root, paneName, null, false );

    public DockNodePlacement FindDockNodePlacement( DockNodeState dockNode )
        => dockNode is null ? null : FindDockNodePlacement( getState().Root, dockNode, null, false );

    public static DockNodeState FindNodeById( DockNodeState node, string nodeId )
    {
        if ( node is null || string.IsNullOrWhiteSpace( nodeId ) )
            return null;

        if ( node.Id == nodeId )
            return node;

        if ( node.Kind == DockNodeKind.Split )
            return FindNodeById( node.First, nodeId ) ?? FindNodeById( node.Second, nodeId );

        return null;
    }

    public static bool ContainsPane( DockNodeState node, string paneName )
        => node?.Kind switch
        {
            DockNodeKind.Pane => node.PaneName == paneName,
            DockNodeKind.Tabs => node.Panes.Contains( paneName ),
            DockNodeKind.Split => ContainsPane( node.First, paneName ) || ContainsPane( node.Second, paneName ),
            _ => false,
        };

    public DockPanePosition? FindPanePosition( DockNodeState node, string paneName )
        => FindPanePosition( node, paneName, DockPanePosition.Center );

    private DockPanePosition? FindPanePosition( DockNodeState node, string paneName, DockPanePosition inheritedPosition )
    {
        if ( node is null || string.IsNullOrWhiteSpace( paneName ) )
            return null;

        return node.Kind switch
        {
            DockNodeKind.Pane => node.PaneName == paneName ? ResolvePanePosition( paneName, inheritedPosition ) : null,
            DockNodeKind.Tabs => node.Panes.Contains( paneName ) ? ResolvePanePosition( paneName, inheritedPosition ) : null,
            DockNodeKind.Split => FindPanePosition( node.First, paneName, GetFirstSplitPosition( node, inheritedPosition ) )
                ?? FindPanePosition( node.Second, paneName, GetSecondSplitPosition( node, inheritedPosition ) ),
            _ => null,
        };
    }

    public bool ShouldKeepSinglePaneTabNode( string paneName )
        => registry.TryGetPane( paneName, out DockPane pane ) && pane.DockRole == DockRole.Document && pane.EffectiveShowTab;

    public string GetFirstPaneName( DockPanePosition position, IReadOnlyList<string> excludedPaneNames )
        => getState().Panes
            .Where( x => x.Visible && !x.AutoHide && x.Position == position && !excludedPaneNames.Contains( x.Name ) )
            .OrderBy( x => x.Order )
            .Select( x => x.Name )
            .FirstOrDefault();

    public static DockPanePosition GetInitialPanePosition( DockPane pane )
        => pane.DockRole == DockRole.Document ? DockPanePosition.Center : pane.Dock;

    private DockPanePosition ResolvePanePosition( string paneName, DockPanePosition inheritedPosition )
        => registry.TryGetPane( paneName, out DockPane pane ) && pane.DockRole == DockRole.Document
            ? DockPanePosition.Center
            : inheritedPosition;

    private bool ShouldPreserveInheritedPosition( DockNodeState node, DockPanePosition inheritedPosition )
        => inheritedPosition != DockPanePosition.Center && HasOnlyPanesInPosition( node, inheritedPosition );

    private bool HasOnlyPanesInPosition( DockNodeState node, DockPanePosition position )
        => node?.Kind switch
        {
            DockNodeKind.Pane => IsPaneInPosition( node.PaneName, position ),
            DockNodeKind.Tabs => node.Panes.Count > 0 && node.Panes.All( paneName => IsPaneInPosition( paneName, position ) ),
            DockNodeKind.Split => HasOnlyPanesInPosition( node.First, position ) && HasOnlyPanesInPosition( node.Second, position ),
            _ => false,
        };

    private bool IsPaneInPosition( string paneName, DockPanePosition position )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
            return false;

        if ( registry.TryGetPane( paneName, out DockPane pane ) && pane.DockRole == DockRole.Document )
            return position == DockPanePosition.Center;

        return stateManager.FindPaneState( getState(), paneName )?.Position == position;
    }

    private DockPanePosition GetFirstSplitPosition( DockNodeState node, DockPanePosition inheritedPosition )
        => ShouldPreserveInheritedPosition( node, inheritedPosition )
            ? inheritedPosition
            : node.Orientation == DockSplitOrientation.Horizontal ? DockPanePosition.Left : DockPanePosition.Top;

    private DockPanePosition GetSecondSplitPosition( DockNodeState node, DockPanePosition inheritedPosition )
        => ShouldPreserveInheritedPosition( node, inheritedPosition )
            ? inheritedPosition
            : node.Orientation == DockSplitOrientation.Horizontal ? DockPanePosition.Right : DockPanePosition.Bottom;

    private static DockNodePlacement FindPanePlacement( DockNodeState node, string paneName, DockNodeState parent, bool firstChild )
    {
        if ( node is null || string.IsNullOrWhiteSpace( paneName ) )
            return null;

        if ( node.Kind == DockNodeKind.Pane && node.PaneName == paneName )
            return CreatePlacement( parent, firstChild );

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( paneName ) )
            return CreatePlacement( parent, firstChild );

        if ( node.Kind == DockNodeKind.Split )
            return FindPanePlacement( node.First, paneName, node, true )
                ?? FindPanePlacement( node.Second, paneName, node, false );

        return null;
    }

    private static DockNodePlacement FindDockNodePlacement( DockNodeState node, DockNodeState dockNode, DockNodeState parent, bool firstChild )
    {
        if ( node is null || dockNode is null )
            return null;

        if ( ReferenceEquals( node, dockNode ) || !string.IsNullOrWhiteSpace( dockNode.Id ) && node.Id == dockNode.Id )
            return CreatePlacement( parent, firstChild );

        if ( node.Kind == DockNodeKind.Split )
            return FindDockNodePlacement( node.First, dockNode, node, true )
                ?? FindDockNodePlacement( node.Second, dockNode, node, false );

        return null;
    }

    private static DockNodePlacement CreatePlacement( DockNodeState parent, bool firstChild )
    {
        if ( parent?.Kind != DockNodeKind.Split )
            return null;

        DockNodeState sibling = firstChild ? parent.Second : parent.First;

        if ( sibling is null )
            return null;

        DockZone zone = parent.Orientation == DockSplitOrientation.Horizontal
            ? firstChild ? DockZone.Left : DockZone.Right
            : firstChild ? DockZone.Top : DockZone.Bottom;

        return new( sibling, zone, parent );
    }

    #endregion

    #region Nested types

    public sealed record DockNodePlacement( DockNodeState Sibling, DockZone Zone, DockNodeState Parent );

    #endregion
}