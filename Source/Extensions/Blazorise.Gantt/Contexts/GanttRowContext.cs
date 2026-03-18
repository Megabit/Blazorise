namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering row content in the tree pane.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttRowContext<TItem>
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    public GanttRowContext( TItem item, int level, bool hasChildren, bool expanded )
    {
        Item = item;
        Level = level;
        HasChildren = hasChildren;
        Expanded = expanded;
    }

    /// <summary>
    /// The data item.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// The tree depth level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Indicates whether the item has child items.
    /// </summary>
    public bool HasChildren { get; }

    /// <summary>
    /// Indicates whether the item is expanded.
    /// </summary>
    public bool Expanded { get; }
}