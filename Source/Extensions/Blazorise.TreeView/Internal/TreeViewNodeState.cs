using System;
using System.Collections.Generic;

namespace Blazorise.TreeView.Internal;

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
    /// <param name="hasChildren">Indicates if the node has any child node.</param>
    /// <param name="expanded">True if the node should be expanded.</param>
    public TreeViewNodeState( TNode node, bool hasChildren, bool expanded )
    {
        Key = Guid.NewGuid().ToString();
        Node = node;
        HasChildren = hasChildren;
        Expanded = expanded;
    }

    /// <summary>
    /// Unique key of each node state, used for rendering optimization.
    /// </summary>
    internal string Key { get; private set; }

    /// <summary>
    /// Reference to the component that is rendering current node.
    /// </summary>
    public _TreeViewNode<TNode> ViewRef { get; internal set; }

    /// <summary>
    /// Gets the node attached to the state.
    /// </summary>
    public TNode Node { get; set; }

    /// <summary>
    /// Indicates if the node is expanded.
    /// </summary>
    public bool Expanded { get; set; }

    /// <summary>
    /// Indicates if the node was auto expanded. Can happen only once when node is first loaded.
    /// </summary>
    public bool AutoExpanded { get; set; }

    /// <summary>
    /// Indicates if the node has any child node.
    /// </summary>
    public bool HasChildren { get; }

    /// <summary>
    /// List of all child node that belongs to this node.
    /// </summary>
    public List<TreeViewNodeState<TNode>> Children { get; set; } = new();
}