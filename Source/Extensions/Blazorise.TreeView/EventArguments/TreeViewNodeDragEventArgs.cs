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
    public TreeViewNodeDragEventArgs( DragEventArgs eventArgs, TNode dragged, TNode newParent, TNode oldParent )
    {
        DraggedNode = dragged;
        NewParentNode = newParent;
        OldParentNode = oldParent;
        DragEventArgs = eventArgs;
    }

    /// <summary>
    /// The dragged tree node.
    /// </summary>
    public TNode DraggedNode { get; }

    /// <summary>
    /// The target node where the node is dragged over or dropped on.
    /// Can be null if over root of TreeView.
    /// </summary>
    public TNode NewParentNode { get; }

    /// <summary>
    /// The source parent node that the dragged node came from.
    /// Can be null if source was root of TreeView.
    /// </summary>
    public TNode OldParentNode { get; }

    /// <summary>
    /// The original drag event args.
    /// </summary>
    public DragEventArgs DragEventArgs { get; }
}
