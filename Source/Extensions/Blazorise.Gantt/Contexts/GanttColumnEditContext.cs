namespace Blazorise.Gantt;

/// <summary>
/// Context passed to <see cref="BaseGanttColumn{TItem}.EditTemplate"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttColumnEditContext<TItem>
{
    /// <summary>
    /// Creates a new <see cref="GanttColumnEditContext{TItem}"/>.
    /// </summary>
    public GanttColumnEditContext( Gantt<TItem> gantt, BaseGanttColumn<TItem> column, TItem item, GanttEditState editState )
    {
        Gantt = gantt;
        Column = column;
        Item = item;
        EditState = editState;
    }

    /// <summary>
    /// Parent gantt.
    /// </summary>
    public Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Column.
    /// </summary>
    public BaseGanttColumn<TItem> Column { get; }

    /// <summary>
    /// Edited item.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Current edit state.
    /// </summary>
    public GanttEditState EditState { get; }
}