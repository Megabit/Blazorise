using System.Collections.Generic;

namespace Blazorise.TreeView;

/// <summary>
/// Holds the state of the <see cref="TreeView{TNode}"/>.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public record struct TreeViewState<TNode>
{
    #region Properties

    /// <summary>
    /// Defines the currently selected node.
    /// </summary>
    public TNode SelectedNode { get; init; }

    /// <summary>
    /// Defines the list of currently selected nodes.
    /// </summary>
    public IList<TNode> SelectedNodes { get; init; }

    /// <summary>
    /// Defines the selection mode of the <see cref="TreeView{TNode}"/>.
    /// </summary>
    public TreeViewSelectionMode SelectionMode { get; set; }

    #endregion
}