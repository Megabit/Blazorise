namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering timeline item content.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttItemContext<TItem>
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    public GanttItemContext( TItem item, bool hasChildren, bool expanded )
    {
        Item = item;
        HasChildren = hasChildren;
        Expanded = expanded;
    }

    /// <summary>
    /// The data item.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Indicates whether the item has child items.
    /// </summary>
    public bool HasChildren { get; }

    /// <summary>
    /// Indicates whether the item is expanded.
    /// </summary>
    public bool Expanded { get; }
}