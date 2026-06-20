#region Using directives
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

    public void SetPaneAutoHide( DockLayoutState state, string paneName, bool autoHide )
    {
        DockPaneState paneState = FindPaneState( state, paneName );

        if ( paneState is not null )
            paneState.AutoHide = autoHide;
    }

    public void Normalize( DockLayoutState state, DockLayoutRegistry registry, DockLayoutTreeQuery query, ref int nextNodeId )
    {
        if ( state.Root is null )
            return;

        EnsureNodeIds( state.Root, ref nextNodeId );
        state.Root = DockLayoutNormalizer.Normalize( state.Root, registry.Panes, state.Panes );
        EnsureNodeIds( state.Root, ref nextNodeId );
        SyncPanePositionsFromTree( state, query );
    }

    private void SyncPanePositionsFromTree( DockLayoutState state, DockLayoutTreeQuery query )
    {
        foreach ( DockPaneState paneState in state.Panes )
        {
            DockPanePosition? position = query.FindPanePosition( state.Root, paneState.Name );

            if ( position is not null )
                paneState.Position = position.Value;
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

    #endregion
}