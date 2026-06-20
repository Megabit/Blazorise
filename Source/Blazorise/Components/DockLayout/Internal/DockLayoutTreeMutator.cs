#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

internal sealed class DockLayoutTreeMutator
{
    #region Members

    private readonly DockLayoutTreeQuery query;

    private readonly DockLayoutSizer sizer;

    #endregion

    #region Constructors

    public DockLayoutTreeMutator( DockLayoutTreeQuery query, DockLayoutSizer sizer )
    {
        this.query = query;
        this.sizer = sizer;
    }

    #endregion

    #region Methods

    public void MoveNodeToZone( DockLayoutState state, DockNodeState movingNode, IReadOnlyList<string> movingPaneNames, string targetName, string targetNodeId, DockZone zone, bool mergeWithTargetTabs, bool targetExists )
    {
        DockNodeState originalRoot = state.Root;

        state.Root = RemoveTabsNode( state.Root, movingNode );

        if ( ( zone == DockZone.Center || mergeWithTargetTabs ) && targetExists )
        {
            foreach ( string movingPaneName in movingPaneNames )
                state.Root = AddPaneToTabs( state, state.Root, targetName, movingPaneName );

            return;
        }

        if ( targetExists )
        {
            state.Root = SplitTargetNode( state, state.Root, targetName, targetNodeId, movingNode, zone );
            return;
        }

        if ( zone == DockZone.Center )
        {
            string centerPaneName = query.GetFirstPaneName( DockPanePosition.Center, movingPaneNames );

            if ( !string.IsNullOrWhiteSpace( centerPaneName ) )
            {
                foreach ( string movingPaneName in movingPaneNames )
                    state.Root = AddPaneToTabs( state, state.Root, centerPaneName, movingPaneName );

                return;
            }

            state.Root = AddNodeToCenter( state.Root, movingNode );
            return;
        }

        DockPanePosition? position = ToDockPanePosition( zone );

        if ( position is null )
        {
            state.Root = originalRoot;
            return;
        }

        state.Root = CreateOuterDockSplitNode( state.Root, movingNode, position.Value );
    }

    public void MovePaneToZone( DockLayoutState state, string paneName, string targetName, string targetNodeId, DockZone zone, bool mergeWithTargetTabs )
    {
        DockNodeState originalRoot = state.Root;
        bool targetExists = !string.IsNullOrWhiteSpace( targetName ) && targetName != paneName && DockLayoutTreeQuery.ContainsPane( state.Root, targetName );

        state.Root = RemovePaneNode( state.Root, paneName );

        DockNodeState paneNode = new()
        {
            Kind = DockNodeKind.Pane,
            PaneName = paneName,
        };

        if ( ( zone == DockZone.Center || mergeWithTargetTabs ) && targetExists )
        {
            state.Root = AddPaneToTabs( state, state.Root, targetName, paneName );
            return;
        }

        if ( targetExists )
        {
            state.Root = SplitTargetNode( state, state.Root, targetName, targetNodeId, paneNode, zone );
            return;
        }

        if ( zone == DockZone.Center )
        {
            string centerPaneName = query.GetFirstPaneName( DockPanePosition.Center, [paneName] );

            if ( !string.IsNullOrWhiteSpace( centerPaneName ) )
            {
                state.Root = AddPaneToTabs( state, state.Root, centerPaneName, paneName );
                return;
            }

            state.Root = AddNodeToCenter( state.Root, paneNode );
            return;
        }

        DockPanePosition? position = ToDockPanePosition( zone );

        if ( position is null )
        {
            state.Root = originalRoot;
            return;
        }

        state.Root = CreateOuterDockSplitNode( state.Root, paneNode, position.Value );
    }

    public DockNodeState RemovePaneNode( DockNodeState node, string paneName )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane )
            return node.PaneName == paneName ? null : node;

        if ( node.Kind == DockNodeKind.Tabs )
        {
            node.Panes.Remove( paneName );

            if ( node.Panes.Count == 0 )
                return null;

            if ( node.ActivePane == paneName )
                node.ActivePane = node.Panes[0];

            return node;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            DockNodeState first = RemovePaneNode( node.First, paneName );
            DockNodeState second = RemovePaneNode( node.Second, paneName );

            return CollapseSplitNode( node, first, second );
        }

        return node;
    }

    public DockPanePosition? GetDropPanePosition( DockPaneState targetState, DockZone zone, bool mergeWithTargetTabs )
    {
        if ( mergeWithTargetTabs )
            return targetState?.Position;

        if ( targetState is not null && targetState.Position != DockPanePosition.Center )
            return targetState.Position;

        return ToDockPanePosition( zone );
    }

    public static DockZone? ToDockZone( string value )
        => Enum.TryParse( value, out DockZone zone ) ? zone : null;

    private DockNodeState AddPaneToTabs( DockLayoutState state, DockNodeState node, string targetName, string paneName )
    {
        if ( node is null )
            return null;

        if ( node.Kind == DockNodeKind.Pane && node.PaneName == targetName )
        {
            DockNodeState tabsNode = new()
            {
                Kind = DockNodeKind.Tabs,
                ActivePane = paneName,
            };

            tabsNode.Panes.Add( targetName );
            tabsNode.Panes.Add( paneName );
            SetDockTabsNodeSize( state, tabsNode );

            return tabsNode;
        }

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( targetName ) )
        {
            if ( !node.Panes.Contains( paneName ) )
                node.Panes.Add( paneName );

            node.ActivePane = paneName;
            SetDockTabsNodeSize( state, node );

            return node;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = AddPaneToTabs( state, node.First, targetName, paneName );
            node.Second = AddPaneToTabs( state, node.Second, targetName, paneName );
        }

        return node;
    }

    private void SetDockTabsNodeSize( DockLayoutState state, DockNodeState node )
    {
        if ( sizer.IsCenterDockGroup( state, node.Panes ) )
            node.Size = null;
        else
            node.Size ??= sizer.GetDockGroupSize( state, node.Panes );
    }

    private static DockNodeState AddNodeToCenter( DockNodeState root, DockNodeState centerNode )
    {
        DockNodeState updatedRoot = ReplaceContentNode( root, centerNode, out bool replaced );

        return replaced ? updatedRoot : centerNode;
    }

    private static DockNodeState ReplaceContentNode( DockNodeState node, DockNodeState centerNode, out bool replaced )
    {
        replaced = false;

        if ( node is null )
            return centerNode;

        if ( node.Kind == DockNodeKind.Content )
        {
            replaced = true;
            return centerNode;
        }

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = ReplaceContentNode( node.First, centerNode, out bool firstReplaced );

            if ( firstReplaced )
            {
                replaced = true;
                return node;
            }

            node.Second = ReplaceContentNode( node.Second, centerNode, out bool secondReplaced );
            replaced = secondReplaced;
        }

        return node;
    }

    private static DockNodeState RemoveTabsNode( DockNodeState node, DockNodeState tabsNode )
    {
        if ( node is null )
            return null;

        if ( ReferenceEquals( node, tabsNode ) )
            return null;

        if ( node.Kind == DockNodeKind.Split )
        {
            DockNodeState first = RemoveTabsNode( node.First, tabsNode );
            DockNodeState second = RemoveTabsNode( node.Second, tabsNode );

            return CollapseSplitNode( node, first, second );
        }

        return node;
    }

    private DockNodeState SplitTargetNode( DockLayoutState state, DockNodeState node, string targetName, string targetNodeId, DockNodeState paneNode, DockZone zone )
    {
        if ( node is null )
            return null;

        if ( !string.IsNullOrWhiteSpace( targetNodeId ) && node.Id == targetNodeId )
            return CreateTargetSplitNode( state, node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Pane && node.PaneName == targetName )
            return CreateTargetSplitNode( state, node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Tabs && node.Panes.Contains( targetName ) )
            return CreateTargetSplitNode( state, node, paneNode, zone );

        if ( node.Kind == DockNodeKind.Split )
        {
            node.First = SplitTargetNode( state, node.First, targetName, targetNodeId, paneNode, zone );
            node.Second = SplitTargetNode( state, node.Second, targetName, targetNodeId, paneNode, zone );
        }

        return node;
    }

    private DockNodeState CreateTargetSplitNode( DockLayoutState state, DockNodeState targetNode, DockNodeState paneNode, DockZone zone )
    {
        string targetSize = sizer.GetDockNodeSize( state, targetNode );
        DockNodeState splitNode = zone switch
        {
            DockZone.Left => DockLayoutTreeBuilder.CreateSplitNode( paneNode, targetNode, DockSplitOrientation.Horizontal, 0.32 ),
            DockZone.Right => DockLayoutTreeBuilder.CreateSplitNode( targetNode, paneNode, DockSplitOrientation.Horizontal, 0.68 ),
            DockZone.Top => DockLayoutTreeBuilder.CreateSplitNode( paneNode, targetNode, DockSplitOrientation.Vertical, 0.32 ),
            DockZone.Bottom => DockLayoutTreeBuilder.CreateSplitNode( targetNode, paneNode, DockSplitOrientation.Vertical, 0.68 ),
            _ => targetNode,
        };

        if ( splitNode?.Kind == DockNodeKind.Split )
        {
            splitNode.Size = targetSize;
            splitNode.UseRatio = true;
        }

        return splitNode;
    }

    private static DockNodeState CreateOuterDockSplitNode( DockNodeState root, DockNodeState dockNode, DockPanePosition position )
        => position switch
        {
            DockPanePosition.Left => DockLayoutTreeBuilder.CreateSplitNode( dockNode, root, DockSplitOrientation.Horizontal, 0.22 ),
            DockPanePosition.Right => DockLayoutTreeBuilder.CreateSplitNode( root, dockNode, DockSplitOrientation.Horizontal, 0.78 ),
            DockPanePosition.Top => DockLayoutTreeBuilder.CreateSplitNode( dockNode, root, DockSplitOrientation.Vertical, 0.22 ),
            DockPanePosition.Bottom => DockLayoutTreeBuilder.CreateSplitNode( root, dockNode, DockSplitOrientation.Vertical, 0.78 ),
            _ => root,
        };

    private static DockNodeState CollapseSplitNode( DockNodeState node, DockNodeState first, DockNodeState second )
    {
        if ( first is null )
            return second;

        if ( second is null )
            return first;

        node.First = first;
        node.Second = second;

        return node;
    }

    private static DockPanePosition? ToDockPanePosition( DockZone zone )
        => zone switch
        {
            DockZone.Center => DockPanePosition.Center,
            DockZone.Left => DockPanePosition.Left,
            DockZone.Right => DockPanePosition.Right,
            DockZone.Top => DockPanePosition.Top,
            DockZone.Bottom => DockPanePosition.Bottom,
            _ => null,
        };

    #endregion
}