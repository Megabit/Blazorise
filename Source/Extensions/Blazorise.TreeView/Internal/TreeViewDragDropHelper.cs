using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.TreeView.EventArguments;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazorise.TreeView.Internal;

internal sealed class TreeViewDragDropHelper<TNode>( TreeView<TNode> treeView, Func<Task> stateHasChanged )
{
    #region Properties

    /// <summary>
    /// The node that is currently being dragged.
    /// </summary>
    public TreeViewNodeState<TNode> DraggedNode { get; set; }

    /// <summary>
    /// The node that is currently the target of a drag and drop operation.
    /// </summary>
    public TreeViewNodeState<TNode> ActiveDropNode { get; private set; }

    /// <summary>
    /// Indicates whether the active drop target will insert the dragged node as a child.
    /// </summary>
    public bool ActiveDropAsChild { get; private set; }

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

    public Task OnDragStart( TreeViewNodeState<TNode> nodeState, DragEventArgs eventArgs )
    {
        if ( !treeView.Draggable || nodeState is null )
            return Task.CompletedTask;

        if ( treeView.CanDragNode?.Invoke( nodeState.Node ) is not true )
            return Task.CompletedTask;

        DraggedNode = nodeState;
        return Task.CompletedTask;
    }

    public Task OnDragOver( TreeViewNodeState<TNode> nodeState, DragEventArgs eventArgs )
    {
        bool dropAsChild = ShouldDropAsChild( eventArgs );

        if ( IsDropAllowed( nodeState, dropAsChild ) )
            return SetActiveDropNode( nodeState, dropAsChild ) ?? Task.CompletedTask;

        return SetActiveDropNode( null, false ) ?? Task.CompletedTask;
    }

    public async Task OnDrop( TreeViewNodeState<TNode> nodeState, DragEventArgs eventArgs )
    {
        if ( DraggedNode is null || nodeState is null )
            return;

        TreeViewNodeState<TNode> draggedNodeState = DraggedNode;
        bool dropAsChild = ShouldDropAsChild( eventArgs );
        TreeViewNodeState<TNode> newParentNodeState = GetDropParentNodeState( nodeState, dropAsChild );
        int oldIndex = GetNodeIndex( draggedNodeState );
        int newIndex = GetDropIndex( draggedNodeState, nodeState, dropAsChild );

        if ( !IsDropAllowed( nodeState, dropAsChild ) )
            return;

        TreeViewNodeDragEventArgs<TNode> dragEventArgs = new(
            eventArgs,
            draggedNodeState.Node,
            newParentNodeState is null ? default : newParentNodeState.Node,
            draggedNodeState.Parent is null ? default : draggedNodeState.Parent.Node,
            newIndex,
            oldIndex );

        if ( treeView.NodeDropped.HasDelegate )
            await treeView.NodeDropped.InvokeAsync( dragEventArgs );

        await SetActiveDropNode( null, false );
        DraggedNode = null;
    }

    public async Task OnDragEnd( TreeViewNodeState<TNode> nodeState, DragEventArgs eventArgs )
    {
        await SetActiveDropNode( null, false );
        DraggedNode = null;
    }

    public bool IsNodeDraggable( TreeViewNodeState<TNode> nodeState )
    {
        if ( !treeView.Draggable || nodeState is null )
            return false;

        return treeView.CanDragNode?.Invoke( nodeState.Node ) ?? true;
    }

    public bool IsDropAllowed( TreeViewNodeState<TNode> nodeState, bool dropAsChild = true )
    {
        if ( !treeView.Draggable || DraggedNode is null )
            return false;

        if ( !dropAsChild && !treeView.Reorderable )
            return false;

        TreeViewNodeState<TNode> draggedNodeState = DraggedNode;
        TreeViewNodeState<TNode> newParentNodeState = GetDropParentNodeState( nodeState, dropAsChild );
        int oldIndex = GetNodeIndex( draggedNodeState );
        int newIndex = GetDropIndex( draggedNodeState, nodeState, dropAsChild );

        if ( ReferenceEquals( draggedNodeState, nodeState ) )
            return false;

        if ( nodeState != null && IsDescendantOf( nodeState, draggedNodeState ) )
            return false;

        TreeViewNodeDragEventArgs<TNode> dragEventArgs = new(
            null,
            draggedNodeState.Node,
            newParentNodeState is null ? default : newParentNodeState.Node,
            draggedNodeState.Parent is null ? default : draggedNodeState.Parent.Node,
            newIndex,
            oldIndex );

        return treeView.CanDropNode?.Invoke( dragEventArgs ) ?? true;
    }

    private static TreeViewNodeState<TNode> GetDropParentNodeState( TreeViewNodeState<TNode> targetNodeState, bool dropAsChild )
    {
        if ( targetNodeState is null )
            return null;

        return dropAsChild ? targetNodeState : targetNodeState.Parent;
    }

    private int GetDropIndex( TreeViewNodeState<TNode> draggedNodeState, TreeViewNodeState<TNode> targetNodeState, bool dropAsChild )
    {
        TreeViewNodeState<TNode> dropParentNodeState = GetDropParentNodeState( targetNodeState, dropAsChild );
        IList<TreeViewNodeState<TNode>> destinationNodeStates = dropParentNodeState?.Children ?? treeView?.RootNodeStates;

        if ( destinationNodeStates is null )
            return 0;

        if ( !dropAsChild && targetNodeState is not null )
        {
            int targetIndex = destinationNodeStates.IndexOf( targetNodeState );

            if ( targetIndex < 0 )
                return destinationNodeStates.Count;

            if ( ReferenceEquals( draggedNodeState.Parent, dropParentNodeState ) )
            {
                int oldIndex = GetNodeIndex( draggedNodeState );

                return oldIndex < targetIndex
                    ? targetIndex - 1
                    : targetIndex;
            }

            return targetIndex;
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

    private bool ShouldDropAsChild( DragEventArgs eventArgs )
    {
        const double dropBeforeZoneHeight = 8;

        if ( !treeView.Reorderable )
            return true;

        return eventArgs.OffsetY > dropBeforeZoneHeight;
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

    internal async Task SetActiveDropNode( TreeViewNodeState<TNode> nodeState, bool asChild )
    {
        if ( ReferenceEquals( ActiveDropNode, nodeState ) && ActiveDropAsChild == asChild )
            return;

        ActiveDropNode = nodeState;
        ActiveDropAsChild = asChild;

        await stateHasChanged();
    }

    #endregion
}
