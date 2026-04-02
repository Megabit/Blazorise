using Blazorise.TreeView.Internal;
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
    /// <param name="target">The target node state.</param>
    /// <param name="dragged">The dragged node state.</param>
    /// <param name="eventArgs">The source drag event args.</param>
    public TreeViewNodeDragEventArgs( TreeViewNodeState<TNode> target, TreeViewNodeState<TNode> dragged, DragEventArgs eventArgs )
    {
        TargetState = target;
        DraggedState = dragged;
        DragEventArgs = eventArgs;
    }

    /// <summary>
    /// The target node state where the dragged node is dragged over or dropped.
    /// </summary>
    public TreeViewNodeState<TNode> TargetState { get; }

    /// <summary>
    /// The target node.
    /// </summary>
    public TNode TargetNode => TargetState.Node;

    /// <summary>
    /// The dragged node state.
    /// </summary>
    public TreeViewNodeState<TNode> DraggedState { get; }

    /// <summary>
    /// The dragged node.
    /// </summary>
    public TNode DraggedNode => DraggedState.Node;

    /// <summary>
    /// The source parent node state that the dragged node came from.
    /// </summary>
    public TreeViewNodeState<TNode> SourceState => DraggedState.Parent;

    /// <summary>
    /// The source parent node that the dragged node came from.
    /// </summary>
    public TNode SourceNode => SourceState is not null ? SourceState.Node : default;

    /// <summary>
    /// The original drag event args.
    /// </summary>
    public DragEventArgs DragEventArgs { get; }
}
