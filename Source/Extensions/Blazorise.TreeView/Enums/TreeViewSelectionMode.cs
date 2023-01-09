namespace Blazorise.TreeView;

/// <summary>
/// Defines the selection mode of the <see cref="TreeView{TNode}"/>.
/// </summary>
public enum TreeViewSelectionMode
{
    /// <summary>
    /// The <see cref="TreeView{TNode}"/> can only select one node at a time.
    /// </summary>
    Single,

    /// <summary>
    /// The <see cref="TreeView{TNode}"/> can select multiple nodes with checkbox support.
    /// </summary>
    Multiple
}
