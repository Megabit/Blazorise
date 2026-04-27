namespace Blazorise.TreeView.Internal;

/// <summary>
/// Drop state defination when a node is dragged over another node.
/// </summary>
internal enum TargetDropState
{
    /// <summary>
    /// Nothing
    /// </summary>
    None,

    /// <summary>
    /// Node will be dropped as a child of the target node.
    /// </summary>
    DropAsChild,

    /// <summary>
    /// Node will be inserted before the target node.
    /// </summary>
    InsertBefore
}