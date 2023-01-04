namespace Blazorise.TreeView;

/// <summary>
/// Holds the state of the node.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public record TreeViewNodeState<TNode>
{
    /// <summary>
    /// Default state constructor.
    /// </summary>
    /// <param name="node">Node that is being referenced.</param>
    /// <param name="expanded">True if the node should be expanded.</param>
    public TreeViewNodeState( TNode node, bool expanded )
    {
        Node = node;
        Expanded = expanded;
    }

    public TNode Node { get; set; }

    public bool Expanded { get; set; }
}