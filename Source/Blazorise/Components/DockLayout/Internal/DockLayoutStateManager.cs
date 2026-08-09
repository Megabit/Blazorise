#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

internal sealed class DockLayoutStateManager
{
    #region Methods

    public DockLayoutState CreatePersistenceSnapshot( DockLayoutState state )
    {
        Dictionary<string, string> groupIds = new();

        return CreateSnapshot( state, false, groupIds );
    }

    public DockLayoutState CreateRuntimeSnapshot( DockLayoutState state )
        => CreateSnapshot( state, true, null );

    private static DockLayoutState CreateSnapshot( DockLayoutState state, bool includeRuntimeState, Dictionary<string, string> groupIds )
        => new()
        {
            Root = CloneNode( state?.Root, includeRuntimeState ),
            Panes = state?.Panes?.Select( pane => ClonePane( pane, includeRuntimeState, groupIds ) ).ToList() ?? new(),
            Rails = state?.Rails?.Select( rail => CloneRail( rail, includeRuntimeState, groupIds ) ).ToList() ?? new(),
        };

    public DockPaneState EnsurePaneState( DockLayoutState state, DockPane pane )
    {
        DockPaneState paneState = FindPaneState( state, pane.ResolvedName );

        if ( paneState is not null )
            return paneState;

        paneState = new()
        {
            Name = pane.ResolvedName,
            Position = DockLayoutTreeQuery.GetInitialPanePosition( pane ),
            Size = pane.Size,
            Collapsed = pane.Collapsed,
            AutoHide = pane.AutoHide,
            Visible = pane.Visible,
            Order = state.Panes.Count,
        };

        state.Panes.Add( paneState );

        return paneState;
    }

    public DockPaneState FindPaneState( DockLayoutState state, string paneName )
        => state.Panes.FirstOrDefault( x => x.Name == paneName );

    public IReadOnlyList<DockRailItemState> GetRailItems( DockLayoutState state, IReadOnlyDictionary<string, DockPane> panes, DockPanePosition position )
    {
        EnsureAutoHideRailItems( state, panes );

        return state.Rails.FirstOrDefault( x => x.Position == position )?.Items
            .OrderBy( x => x.Order )
            .ToArray()
            ?? [];
    }

    public DockRailItemState FindRailItem( DockLayoutState state, string paneName )
        => string.IsNullOrWhiteSpace( paneName )
            ? null
            : state.Rails.SelectMany( x => x.Items ).FirstOrDefault( x => x.PaneName == paneName );

    public DockRailItemState FindRailItemBySourceTabPaneName( DockLayoutState state, string paneName )
        => string.IsNullOrWhiteSpace( paneName )
            ? null
            : state.Rails.SelectMany( x => x.Items ).FirstOrDefault( x => x.SourceTabPaneName == paneName );

    public DockRailItemState FindRailItemBySourceTargetPaneName( DockLayoutState state, string paneName )
        => string.IsNullOrWhiteSpace( paneName )
            ? null
            : state.Rails.SelectMany( x => x.Items ).FirstOrDefault( x => x.SourceTargetPaneName == paneName );

    public void SetPaneAutoHide( DockLayoutState state, string paneName, bool autoHide )
    {
        DockPaneState paneState = FindPaneState( state, paneName );

        if ( paneState is not null )
            paneState.AutoHide = autoHide;
    }

    public void AddRailItem( DockLayoutState state, DockPanePosition position, DockRailItemState item )
    {
        if ( item is null || string.IsNullOrWhiteSpace( item.PaneName ) )
            return;

        RemoveRailItem( state, item.PaneName );

        DockRailState rail = EnsureRailState( state, position );

        item.Order = rail.Items.Count;
        rail.Items.Add( item );
    }

    public DockRailItemState RemoveRailItem( DockLayoutState state, string paneName )
    {
        if ( string.IsNullOrWhiteSpace( paneName ) )
            return null;

        foreach ( DockRailState rail in state.Rails )
        {
            DockRailItemState item = rail.Items.FirstOrDefault( x => x.PaneName == paneName );

            if ( item is not null )
            {
                rail.Items.Remove( item );
                NormalizeRailOrder( rail );

                return item;
            }
        }

        return null;
    }

    public void UpdateRailGroupSourceTabPane( DockLayoutState state, string sourceGroupId, string sourceTabPaneName )
    {
        if ( string.IsNullOrWhiteSpace( sourceGroupId ) || string.IsNullOrWhiteSpace( sourceTabPaneName ) )
            return;

        foreach ( DockRailItemState item in state.Rails.SelectMany( x => x.Items ).Where( x => x.SourceGroupId == sourceGroupId ) )
            item.SourceTabPaneName = sourceTabPaneName;
    }

    public void Normalize( DockLayoutState state, DockLayoutRegistry registry, DockLayoutTreeQuery query, ref int nextNodeId )
    {
        EnsureAutoHideRailItems( state, registry.Panes );
        NormalizeRails( state, registry.Panes );

        if ( state.Root is null )
            return;

        UpdateNextNodeId( state.Root, ref nextNodeId );
        EnsureNodeIds( state.Root, ref nextNodeId );
        state.Root = DockLayoutNormalizer.Normalize( state.Root, registry.Panes, state.Panes );
        EnsureNodeIds( state.Root, ref nextNodeId );
        SyncPanePositionsFromTree( state, query );
        EnsureAutoHideRailItems( state, registry.Panes );
        NormalizeRails( state, registry.Panes );
    }

    private void SyncPanePositionsFromTree( DockLayoutState state, DockLayoutTreeQuery query )
    {
        foreach ( DockPaneState paneState in state.Panes )
        {
            if ( paneState.AutoHide )
                continue;

            DockPanePosition? position = query.FindPanePosition( state.Root, paneState.Name );

            if ( position is not null )
                paneState.Position = position.Value;
        }
    }

    private void EnsureAutoHideRailItems( DockLayoutState state, IReadOnlyDictionary<string, DockPane> panes )
    {
        foreach ( DockPaneState paneState in state.Panes.Where( x => IsPaneAutoHidden( state, x ) ) )
        {
            if ( !panes.ContainsKey( paneState.Name ) || FindRailItem( state, paneState.Name ) is not null )
                continue;

            paneState.AutoHide = true;

            AddRailItem( state, ToRailPosition( paneState.Position ), new()
            {
                PaneName = paneState.Name,
                SourceGroupId = paneState.Name,
                SourcePosition = paneState.Position,
                SourceSize = paneState.Size,
            } );
        }
    }

    private void NormalizeRails( DockLayoutState state, IReadOnlyDictionary<string, DockPane> panes )
    {
        for ( int railIndex = state.Rails.Count - 1; railIndex >= 0; railIndex-- )
        {
            DockRailState rail = state.Rails[railIndex];

            rail.Items.RemoveAll( item =>
            {
                DockPaneState paneState = FindPaneState( state, item.PaneName );
                bool autoHidden = paneState is not null && IsPaneAutoHidden( state, paneState );

                if ( autoHidden )
                    paneState.AutoHide = true;

                return string.IsNullOrWhiteSpace( item.PaneName )
                    || !panes.ContainsKey( item.PaneName )
                    || paneState?.Visible == false
                    || !autoHidden;
            } );

            if ( rail.Items.Count == 0 )
                state.Rails.RemoveAt( railIndex );
            else
                NormalizeRailOrder( rail );
        }
    }

    private static DockRailState EnsureRailState( DockLayoutState state, DockPanePosition position )
    {
        position = ToRailPosition( position );

        DockRailState rail = state.Rails.FirstOrDefault( x => x.Position == position );

        if ( rail is not null )
            return rail;

        rail = new()
        {
            Position = position,
        };

        state.Rails.Add( rail );

        return rail;
    }

    private static void NormalizeRailOrder( DockRailState rail )
    {
        for ( int i = 0; i < rail.Items.Count; i++ )
            rail.Items[i].Order = i;
    }

    private static bool IsPaneAutoHidden( DockLayoutState state, DockPaneState paneState )
        => !string.IsNullOrWhiteSpace( paneState?.Name )
            && paneState.Visible
            && ( paneState.AutoHide || state.Root is not null && !DockLayoutTreeQuery.ContainsPane( state.Root, paneState.Name ) );

    private static void UpdateNextNodeId( DockNodeState node, ref int nextNodeId )
    {
        const string nodeIdPrefix = "dock-node-";

        if ( node is null )
            return;

        if ( node.Id?.StartsWith( nodeIdPrefix, StringComparison.Ordinal ) == true
             && int.TryParse( node.Id.AsSpan( nodeIdPrefix.Length ), out int nodeId ) )
        {
            nextNodeId = Math.Max( nextNodeId, nodeId );
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            UpdateNextNodeId( node.First, ref nextNodeId );
            UpdateNextNodeId( node.Second, ref nextNodeId );
        }
    }

    private static void EnsureNodeIds( DockNodeState node, ref int nextNodeId )
    {
        if ( node is null )
            return;

        if ( string.IsNullOrWhiteSpace( node.Id ) )
            node.Id = $"dock-node-{++nextNodeId}";

        if ( node.Kind == DockNodeKind.Split )
        {
            EnsureNodeIds( node.First, ref nextNodeId );
            EnsureNodeIds( node.Second, ref nextNodeId );
        }
    }

    private static DockNodeState CloneNode( DockNodeState node, bool includeRuntimeState )
    {
        if ( node is null )
            return null;

        return new()
        {
            Id = includeRuntimeState ? node.Id : null,
            Kind = node.Kind,
            PaneName = node.PaneName,
            First = CloneNode( node.First, includeRuntimeState ),
            Second = CloneNode( node.Second, includeRuntimeState ),
            Orientation = node.Orientation,
            Ratio = node.Ratio,
            UseRatio = node.UseRatio,
            Panes = node.Panes is null ? new() : new( node.Panes ),
            ActivePane = node.ActivePane,
            Size = node.Size,
        };
    }

    private static DockPaneState ClonePane( DockPaneState pane, bool includeRuntimeState, Dictionary<string, string> groupIds )
        => new()
        {
            Name = pane.Name,
            Position = pane.Position,
            Size = pane.Size,
            Collapsed = pane.Collapsed,
            AutoHide = pane.AutoHide,
            Visible = pane.Visible,
            RestorePlacement = CloneRestorePlacement( pane.RestorePlacement, includeRuntimeState, groupIds ),
            Order = pane.Order,
        };

    private static DockRailState CloneRail( DockRailState rail, bool includeRuntimeState, Dictionary<string, string> groupIds )
        => new()
        {
            Position = rail.Position,
            Items = rail.Items?.Select( item => CloneRailItem( item, includeRuntimeState, groupIds ) ).ToList() ?? new(),
        };

    private static DockRailItemState CloneRailItem( DockRailItemState item, bool includeRuntimeState, Dictionary<string, string> groupIds )
        => new()
        {
            PaneName = item.PaneName,
            SourceGroupId = CloneSourceGroupId( item.SourceGroupId, groupIds ),
            SourceTabPaneName = item.SourceTabPaneName,
            SourcePosition = item.SourcePosition,
            SourceSize = item.SourceSize,
            SourceSplitRatio = item.SourceSplitRatio,
            SourceSplitUseRatio = item.SourceSplitUseRatio,
            SourceGroupTargetPaneName = item.SourceGroupTargetPaneName,
            SourceGroupTargetNodeId = includeRuntimeState ? item.SourceGroupTargetNodeId : null,
            SourceGroupZone = item.SourceGroupZone,
            SourceGroupSplitRatio = item.SourceGroupSplitRatio,
            SourceGroupSplitUseRatio = item.SourceGroupSplitUseRatio,
            SourceTargetPaneName = item.SourceTargetPaneName,
            SourceTargetNodeId = includeRuntimeState ? item.SourceTargetNodeId : null,
            SourceZone = item.SourceZone,
            SourceIndex = item.SourceIndex,
            Order = item.Order,
        };

    private static DockPaneRestoreState CloneRestorePlacement( DockPaneRestoreState restorePlacement, bool includeRuntimeState, Dictionary<string, string> groupIds )
    {
        if ( restorePlacement is null )
            return null;

        return new()
        {
            SourceGroupId = CloneSourceGroupId( restorePlacement.SourceGroupId, groupIds ),
            SourceTabPaneName = restorePlacement.SourceTabPaneName,
            SourcePosition = restorePlacement.SourcePosition,
            SourceSize = restorePlacement.SourceSize,
            SourceSplitRatio = restorePlacement.SourceSplitRatio,
            SourceSplitUseRatio = restorePlacement.SourceSplitUseRatio,
            SourceGroupTargetPaneName = restorePlacement.SourceGroupTargetPaneName,
            SourceGroupTargetNodeId = includeRuntimeState ? restorePlacement.SourceGroupTargetNodeId : null,
            SourceGroupZone = restorePlacement.SourceGroupZone,
            SourceGroupSplitRatio = restorePlacement.SourceGroupSplitRatio,
            SourceGroupSplitUseRatio = restorePlacement.SourceGroupSplitUseRatio,
            SourceTargetPaneName = restorePlacement.SourceTargetPaneName,
            SourceTargetNodeId = includeRuntimeState ? restorePlacement.SourceTargetNodeId : null,
            SourceZone = restorePlacement.SourceZone,
            SourceIndex = restorePlacement.SourceIndex,
        };
    }

    private static string CloneSourceGroupId( string sourceGroupId, Dictionary<string, string> groupIds )
    {
        if ( string.IsNullOrWhiteSpace( sourceGroupId ) || groupIds is null )
            return sourceGroupId;

        if ( !groupIds.TryGetValue( sourceGroupId, out string groupId ) )
        {
            groupId = $"dock-state-group-{groupIds.Count + 1}";
            groupIds.Add( sourceGroupId, groupId );
        }

        return groupId;
    }

    private static DockPanePosition ToRailPosition( DockPanePosition position )
        => position == DockPanePosition.Center ? DockPanePosition.Right : position;

    #endregion
}