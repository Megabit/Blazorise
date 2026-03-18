namespace Blazorise.Gantt;

/// <summary>
/// Represents command metadata used to decide whether a Gantt edit command is allowed.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttCommandContext<TItem>
{
    /// <summary>
    /// Creates a new <see cref="GanttCommandContext{TItem}"/>.
    /// </summary>
    /// <param name="commandType">The command being evaluated.</param>
    /// <param name="item">The target item for edit/delete operations.</param>
    /// <param name="parentItem">The parent item for add-child operations.</param>
    public GanttCommandContext( GanttCommandType commandType, TItem item = default, TItem parentItem = default )
    {
        CommandType = commandType;
        Item = item;
        ParentItem = parentItem;
    }

    /// <summary>
    /// Gets the command being evaluated.
    /// </summary>
    public GanttCommandType CommandType { get; }

    /// <summary>
    /// Gets target item for edit/delete operations.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets parent item for add-child operations.
    /// </summary>
    public TItem ParentItem { get; }
}