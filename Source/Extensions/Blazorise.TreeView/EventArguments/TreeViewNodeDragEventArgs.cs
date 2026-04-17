using Microsoft.AspNetCore.Components.Web;

namespace Blazorise.TreeView.EventArguments;

/// <summary>
/// <see cref="TreeView{TNode}"/> node drag and drop event arguments.
/// </summary>
/// <typeparam name="TNode">The node item type.</typeparam>
public class TreeViewNodeDragEventArgs<TNode>
{
    /// <summary>
    /// Creates a new instance of <see cref="TreeViewNodeDragEventArgs{TNode}"/>.
    /// </summary>
    public TreeViewNodeDragEventArgs( DragEventArgs eventArgs, TNode dragged, TNode newParent, TNode oldParent, int newIndex, int oldIndex )
    {
        DraggedNode = dragged;
        NewParentNode = newParent;
        OldParentNode = oldParent;
        NewIndex = newIndex;
        OldIndex = oldIndex;
        DragEventArgs = eventArgs;
    }

    /// <summary>
    /// The dragged tree node.
    /// </summary>
    public TNode DraggedNode { get; }

    /// <summary>
    /// The destination parent node where the dragged node will be inserted.
    /// Can be null if the destination is the TreeView root.
    /// </summary>
    public TNode NewParentNode { get; }

    /// <summary>
    /// The source parent node that the dragged node came from.
    /// Can be null if source was root of TreeView.
    /// </summary>
    public TNode OldParentNode { get; }

    /// <summary>
    /// The destination index within the new parent node children collection.
    /// If <see cref="NewParentNode"/> is null, the index is relative to the TreeView root nodes.
    /// </summary>
    public int NewIndex { get; }

    /// <summary>
    /// The original index within the old parent node children collection.
    /// If <see cref="OldParentNode"/> is null, the index is relative to the TreeView root nodes.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// The original drag event args.
    /// </summary>
    public DragEventArgs DragEventArgs { get; }
}
