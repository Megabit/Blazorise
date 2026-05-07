#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.TreeView.EventArguments;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.TreeView.Internal;

internal sealed class TreeViewDragDropBehavior<TNode>( TreeView<TNode> treeView, Func<Task> stateHasChanged )
{
    #region Members

    private TreeViewNodeState<TNode> draggedNode;
    private TreeViewNodeState<TNode> activeDropNode;
    private TreeViewDropIndicator activeDropIndicator;

    #endregion

    #region Methods

    public async Task Attach( ElementReference @ref, string elementId )
    {
        if ( !treeView.Draggable )
            return;

        await treeView.JSDragDropModule.Initialize( @ref, elementId );
    }

    public async Task Detach( ElementReference @ref, string elementId )
    {
        await treeView.JSDragDropModule.Destroy( @ref, elementId );
    }

    public Task OnDragStart( TreeViewNodeState<TNode> nodeState )
    {
        if ( !treeView.Draggable || nodeState is null )
            return Task.CompletedTask;

        if ( treeView.CanDragNode?.Invoke( nodeState.Node ) is not true )
            return Task.CompletedTask;

        draggedNode = nodeState;
        return Task.CompletedTask;
    }

    public Task OnDragOver( TreeViewNodeState<TNode> nodeState, DragEventArgs eventArgs )
    {
        TreeViewDropIndicator dropIndicator = GetDropIndicator( eventArgs );

        if ( IsDropAllowed( nodeState, dropIndicator, eventArgs, true ) )
            return SetActiveDropNode( nodeState, dropIndicator );

        return SetActiveDropNode( null, TreeViewDropIndicator.None );
    }

    public async Task OnDrop( TreeViewNodeState<TNode> nodeState, DragEventArgs eventArgs )
    {
        if ( draggedNode is null || nodeState is null )
            return;

        TreeViewNodeState<TNode> draggedNodeState = draggedNode;
        TreeViewDropIndicator dropIndicator = ReferenceEquals( activeDropNode, nodeState )
            ? activeDropIndicator
            : GetDropIndicator( eventArgs );
        TreeViewNodeState<TNode> newParentNodeState = GetDropParentNodeState( nodeState, dropIndicator );
        int oldIndex = GetNodeIndex( draggedNodeState );
        int newIndex = GetDropIndex( draggedNodeState, nodeState, dropIndicator );

        if ( !IsDropAllowed( nodeState, dropIndicator, eventArgs, true ) )
            return;

        TreeViewNodeDragEventArgs<TNode> dragEventArgs = new(
            eventArgs,
            draggedNodeState.Node,
            newParentNodeState is null ? default : newParentNodeState.Node,
            draggedNodeState.Parent is null ? default : draggedNodeState.Parent.Node,
            newIndex,
            oldIndex );

        if ( treeView.NodeDropped.HasDelegate )
        {
            await treeView.NodeDropped.InvokeAsync( dragEventArgs );
        }
        else
        {
            await treeView.TryMoveNodeAsync( draggedNodeState, newParentNodeState, oldIndex, newIndex );
        }

        await SetActiveDropNode( null, TreeViewDropIndicator.None );
        draggedNode = null;
    }

    public async Task OnDragEnd()
    {
        await SetActiveDropNode( null, TreeViewDropIndicator.None );
        draggedNode = null;
    }

    public bool IsNodeDraggable( TreeViewNodeState<TNode> nodeState )
    {
        if ( !treeView.Draggable || nodeState is null )
            return false;

        return treeView.CanDragNode?.Invoke( nodeState.Node ) ?? true;
    }

    public bool IsDropAllowed( TreeViewNodeState<TNode> nodeState, TreeViewDropIndicator dropIndicator = TreeViewDropIndicator.DropAsChild )
        => IsDropAllowed( nodeState, dropIndicator, null, false );

    private bool IsDropAllowed( TreeViewNodeState<TNode> nodeState, TreeViewDropIndicator dropIndicator, DragEventArgs eventArgs, bool evaluateUserPredicate )
    {
        if ( !treeView.Draggable || draggedNode is null )
            return false;

        if ( dropIndicator is TreeViewDropIndicator.None )
            return false;

        if ( dropIndicator is not TreeViewDropIndicator.DropAsChild && !treeView.Reorderable )
            return false;

        TreeViewNodeState<TNode> draggedNodeState = draggedNode;
        TreeViewNodeState<TNode> newParentNodeState = GetDropParentNodeState( nodeState, dropIndicator );
        int oldIndex = GetNodeIndex( draggedNodeState );
        int newIndex = GetDropIndex( draggedNodeState, nodeState, dropIndicator );

        if ( ReferenceEquals( draggedNodeState, nodeState ) )
            return false;

        if ( nodeState != null && IsDescendantOf( nodeState, draggedNodeState ) )
            return false;

        if ( !evaluateUserPredicate )
            return true;

        TreeViewNodeDragEventArgs<TNode> dragEventArgs = new(
            eventArgs,
            draggedNodeState.Node,
            newParentNodeState is null ? default : newParentNodeState.Node,
            draggedNodeState.Parent is null ? default : draggedNodeState.Parent.Node,
            newIndex,
            oldIndex );

        return treeView.CanDropNode?.Invoke( dragEventArgs ) ?? true;
    }

    public bool CanDropOnNode( TreeViewNodeState<TNode> nodeState )
        => IsDropAllowed( nodeState, TreeViewDropIndicator.DropAsChild )
           || IsDropAllowed( nodeState, TreeViewDropIndicator.InsertBefore )
           || IsDropAllowed( nodeState, TreeViewDropIndicator.InsertAfter );

    private static TreeViewNodeState<TNode> GetDropParentNodeState( TreeViewNodeState<TNode> targetNodeState, TreeViewDropIndicator dropIndicator )
    {
        if ( targetNodeState is null )
            return null;

        return dropIndicator is TreeViewDropIndicator.DropAsChild
            ? targetNodeState
            : targetNodeState.Parent;
    }

    private int GetDropIndex( TreeViewNodeState<TNode> draggedNodeState, TreeViewNodeState<TNode> targetNodeState, TreeViewDropIndicator dropIndicator )
    {
        TreeViewNodeState<TNode> dropParentNodeState = GetDropParentNodeState( targetNodeState, dropIndicator );
        IList<TreeViewNodeState<TNode>> destinationNodeStates = dropParentNodeState?.Children ?? treeView?.RootNodeStates;

        if ( destinationNodeStates is null )
            return 0;

        if ( dropIndicator is TreeViewDropIndicator.InsertBefore or TreeViewDropIndicator.InsertAfter && targetNodeState is not null )
        {
            int targetIndex = destinationNodeStates.IndexOf( targetNodeState );

            if ( targetIndex < 0 )
                return destinationNodeStates.Count;

            int targetDropIndex = dropIndicator is TreeViewDropIndicator.InsertAfter
                ? targetIndex + 1
                : targetIndex;

            if ( ReferenceEquals( draggedNodeState.Parent, dropParentNodeState ) )
            {
                int oldIndex = GetNodeIndex( draggedNodeState );

                return oldIndex < targetDropIndex
                    ? targetDropIndex - 1
                    : targetDropIndex;
            }

            return targetDropIndex;
        }

        int newIndex = destinationNodeStates.Count;

        if ( ReferenceEquals( draggedNodeState.Parent, dropParentNodeState ) )
        {
            int oldIndex = GetNodeIndex( draggedNodeState );

            if ( oldIndex >= 0 && oldIndex < newIndex )
                newIndex--;
        }

        return newIndex;
    }

    private TreeViewDropIndicator GetDropIndicator( DragEventArgs eventArgs )
    {
        const double dropEdgeZoneHeight = 8;
        const double nodeTitleMinHeight = 28;

        if ( !treeView.Reorderable )
            return TreeViewDropIndicator.DropAsChild;

        if ( eventArgs.OffsetY <= dropEdgeZoneHeight )
            return TreeViewDropIndicator.InsertBefore;

        if ( eventArgs.OffsetY >= nodeTitleMinHeight - dropEdgeZoneHeight )
            return TreeViewDropIndicator.InsertAfter;

        return TreeViewDropIndicator.DropAsChild;
    }

    private int GetNodeIndex( TreeViewNodeState<TNode> nodeState )
    {
        IList<TreeViewNodeState<TNode>> siblingNodeStates = nodeState?.Parent?.Children ?? treeView.RootNodeStates;

        return siblingNodeStates?.IndexOf( nodeState ) ?? -1;
    }

    private static bool IsDescendantOf( TreeViewNodeState<TNode> potentialDescendant, TreeViewNodeState<TNode> potentialAncestor )
    {
        TreeViewNodeState<TNode> current = potentialDescendant?.Parent;

        while ( current is not null )
        {
            if ( ReferenceEquals( current, potentialAncestor ) )
                return true;

            current = current.Parent;
        }

        return false;
    }

    internal async Task SetActiveDropNode( TreeViewNodeState<TNode> nodeState, TreeViewDropIndicator dropIndicator )
    {
        if ( ReferenceEquals( activeDropNode, nodeState ) && activeDropIndicator == dropIndicator )
            return;

        activeDropNode = nodeState;
        activeDropIndicator = dropIndicator;

        await stateHasChanged();
    }

    public TreeViewDropIndicator GetDropState( TreeViewNodeState<TNode> target )
    {
        if ( !treeView.Draggable || !ReferenceEquals( activeDropNode, target ) )
        {
            return TreeViewDropIndicator.None;
        }

        return activeDropIndicator;
    }

    #endregion
}