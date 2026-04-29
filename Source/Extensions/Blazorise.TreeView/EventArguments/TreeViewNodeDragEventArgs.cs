using System;
using Microsoft.AspNetCore.Components.Web;

namespace Blazorise.TreeView.EventArguments;

/// <summary>
/// Provides information about a TreeView node drag-and-drop operation.
/// </summary>
/// <typeparam name="TNode">The node item type.</typeparam>
public class TreeViewNodeDragEventArgs<TNode> : EventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="TreeViewNodeDragEventArgs{TNode}"/>.
    /// </summary>
    /// <param name="eventArgs">The underlying browser drag event.</param>
    /// <param name="draggedNode">The dragged node.</param>
    /// <param name="newParent">The proposed destination parent node, or the default value when dropping at the TreeView root.</param>
    /// <param name="oldParent">The source parent node, or the default value when the dragged node comes from the TreeView root.</param>
    /// <param name="newIndex">The proposed destination index within the destination parent or root collection.</param>
    /// <param name="oldIndex">The original index within the source parent or root collection.</param>
    public TreeViewNodeDragEventArgs( DragEventArgs eventArgs, TNode draggedNode, TNode newParent, TNode oldParent, int newIndex, int oldIndex )
    {
        DraggedNode = draggedNode;
        NewParentNode = newParent;
        OldParentNode = oldParent;
        NewIndex = newIndex;
        OldIndex = oldIndex;
        DragEventArgs = eventArgs;
    }

    /// <summary>
    /// Gets the dragged node.
    /// </summary>
    public TNode DraggedNode { get; }

    /// <summary>
    /// Gets the proposed destination parent node.
    /// </summary>
    /// <remarks>When this value is the default value for <typeparamref name="TNode"/>, the destination is the TreeView root.</remarks>
    public TNode NewParentNode { get; }

    /// <summary>
    /// Gets the source parent node.
    /// </summary>
    /// <remarks>When this value is the default value for <typeparamref name="TNode"/>, the source is the TreeView root.</remarks>
    public TNode OldParentNode { get; }

    /// <summary>
    /// Gets the proposed destination index within the destination parent or root collection.
    /// </summary>
    public int NewIndex { get; }

    /// <summary>
    /// Gets the original index within the source parent or root collection.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// Gets the underlying browser drag event.
    /// </summary>
    public DragEventArgs DragEventArgs { get; }
}