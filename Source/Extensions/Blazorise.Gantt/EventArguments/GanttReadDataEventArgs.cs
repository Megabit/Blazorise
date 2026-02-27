#region Using directives
using System;
using System.Threading;
using Blazorise;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Event arguments used for manual data loading.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttReadDataEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="GanttReadDataEventArgs{TItem}"/>.
    /// </summary>
    public GanttReadDataEventArgs( GanttView view, DateOnly date, DateTime viewStart, DateTime viewEnd, string searchText, CancellationToken cancellationToken )
        : this( view, date, viewStart, viewEnd, searchText, null, SortDirection.Default, cancellationToken )
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="GanttReadDataEventArgs{TItem}"/>.
    /// </summary>
    public GanttReadDataEventArgs( GanttView view, DateOnly date, DateTime viewStart, DateTime viewEnd, string searchText, GanttSortColumn? sortColumn, SortDirection sortDirection, CancellationToken cancellationToken )
    {
        View = view;
        Date = date;
        ViewStart = viewStart;
        ViewEnd = viewEnd;
        SearchText = searchText;
        SortColumn = sortColumn;
        SortDirection = sortDirection;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Current selected view.
    /// </summary>
    public GanttView View { get; }

    /// <summary>
    /// Current selected date.
    /// </summary>
    public DateOnly Date { get; }

    /// <summary>
    /// Current view start.
    /// </summary>
    public DateTime ViewStart { get; }

    /// <summary>
    /// Current view end.
    /// </summary>
    public DateTime ViewEnd { get; }

    /// <summary>
    /// Current search text.
    /// </summary>
    public string SearchText { get; }

    /// <summary>
    /// Current sort column when sorting is active.
    /// </summary>
    public GanttSortColumn? SortColumn { get; }

    /// <summary>
    /// Current sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }

    /// <summary>
    /// Cancellation token for this request.
    /// </summary>
    public CancellationToken CancellationToken { get; }
}