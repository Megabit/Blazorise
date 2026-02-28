namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering a tree header cell.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttTreeHeaderCellContext<TItem>
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    /// <param name="gantt">Parent Gantt component.</param>
    /// <param name="column">Header column kind.</param>
    /// <param name="text">Header display text.</param>
    /// <param name="sortable">Indicates whether the header can trigger sorting.</param>
    /// <param name="showSortIcon">Indicates whether sort icon should be shown.</param>
    /// <param name="sortDirection">Current sort direction for this header.</param>
    public GanttTreeHeaderCellContext( Gantt<TItem> gantt, GanttTreeColumn column, string text, bool sortable, bool showSortIcon, SortDirection sortDirection )
    {
        Gantt = gantt;
        Column = column;
        Text = text;
        Sortable = sortable;
        ShowSortIcon = showSortIcon;
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Gets the parent Gantt component.
    /// </summary>
    public Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Gets the header column kind.
    /// </summary>
    public GanttTreeColumn Column { get; }

    /// <summary>
    /// Gets header display text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets whether this header can trigger sorting.
    /// </summary>
    public bool Sortable { get; }

    /// <summary>
    /// Gets whether sort icon should be rendered.
    /// </summary>
    public bool ShowSortIcon { get; }

    /// <summary>
    /// Gets current sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }
}