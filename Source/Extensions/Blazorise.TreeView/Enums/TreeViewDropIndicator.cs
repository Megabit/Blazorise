namespace Blazorise.TreeView.Internal;

/// <summary>
/// Identifies the visual drop indicator shown for a tree node while another node is dragged over it.
/// </summary>
internal enum TreeViewDropIndicator
{
    /// <summary>
    /// No drop indicator is shown.
    /// </summary>
    None,

    /// <summary>
    /// Indicates that the dragged node can be dropped as a child of the target node.
    /// </summary>
    DropAsChild,

    /// <summary>
    /// Indicates that the dragged node can be inserted before the target node.
    /// </summary>
    InsertBefore
}