#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

internal sealed class DockLayoutStateManager
{
    #region Methods

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

    public IReadOnlyList<DockRailItemState> RemoveRailGroup( DockLayoutState state, DockRailItemState item )
    {
        if ( item is null )
            return [];

        IReadOnlyList<DockRailItemState> groupItems = string.IsNullOrWhiteSpace( item.SourceGroupId )
            ? [item]
            : state.Rails
                .SelectMany( x => x.Items )
                .Where( x => x.SourceGroupId == item.SourceGroupId )
                .OrderBy( x => x.SourceIndex )
                .ToArray();

        foreach ( DockRailItemState groupItem in groupItems )
            RemoveRailItem( state, groupItem.PaneName );

        return groupItems;
    }

    public void Normalize( DockLayoutState state, DockLayoutRegistry registry, DockLayoutTreeQuery query, ref int nextNodeId )
    {
        EnsureAutoHideRailItems( state, registry.Panes );
        NormalizeRails( state, registry.Panes );

        if ( state.Root is null )
            return;

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

    private static DockPanePosition ToRailPosition( DockPanePosition position )
        => position == DockPanePosition.Center ? DockPanePosition.Right : position;

    #endregion
}