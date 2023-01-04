namespace Blazorise.TreeView;

/// <summary>
/// Holds the state of the treeview.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public record struct TreeViewState<TNode>
{
    #region Properties

    /// <summary>
    /// Gets or sets the currently selected node.
    /// </summary>
    public TNode SelectedNode { get; init; }

    #endregion
}