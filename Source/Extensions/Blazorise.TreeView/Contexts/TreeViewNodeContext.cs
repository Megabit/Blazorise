namespace Blazorise.TreeView;

/// <summary>
/// Holds contextual data for tree view node styling.
/// </summary>
/// <typeparam name="TNode">Type of the node.</typeparam>
public sealed class TreeViewNodeContext<TNode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewNodeContext{TNode}"/>.
    /// </summary>
    /// <param name="node">The node instance.</param>
    /// <param name="hasChildren">Indicates if the node has children.</param>
    /// <param name="expanded">Indicates if the node is expanded.</param>
    /// <param name="disabled">Indicates if the node is disabled.</param>
    /// <param name="autoExpanded">Indicates if the node was auto-expanded.</param>
    /// <param name="selected">Indicates if the node is selected.</param>
    /// <param name="checked">Indicates if the node is checked.</param>
    /// <param name="selectionMode">Current tree view selection mode.</param>
    public TreeViewNodeContext( TNode node, bool hasChildren, bool expanded, bool disabled, bool autoExpanded, bool selected, bool @checked, TreeViewSelectionMode selectionMode )
    {
        Node = node;
        HasChildren = hasChildren;
        Expanded = expanded;
        Disabled = disabled;
        AutoExpanded = autoExpanded;
        Selected = selected;
        Checked = @checked;
        SelectionMode = selectionMode;
    }

    /// <summary>
    /// Gets the node instance.
    /// </summary>
    public TNode Node { get; }

    /// <summary>
    /// Indicates if the node has children.
    /// </summary>
    public bool HasChildren { get; }

    /// <summary>
    /// Indicates if the node is expanded.
    /// </summary>
    public bool Expanded { get; }

    /// <summary>
    /// Indicates if the node is disabled.
    /// </summary>
    public bool Disabled { get; }

    /// <summary>
    /// Indicates if the node was auto-expanded.
    /// </summary>
    public bool AutoExpanded { get; }

    /// <summary>
    /// Indicates if the node is selected.
    /// </summary>
    public bool Selected { get; }

    /// <summary>
    /// Indicates if the node is checked.
    /// </summary>
    public bool Checked { get; }

    /// <summary>
    /// Gets the current selection mode.
    /// </summary>
    public TreeViewSelectionMode SelectionMode { get; }
}