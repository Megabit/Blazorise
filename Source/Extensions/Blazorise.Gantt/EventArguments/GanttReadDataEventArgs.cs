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
        : this( view, date, viewStart, viewEnd, searchText, null, null, SortDirection.Default, cancellationToken )
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="GanttReadDataEventArgs{TItem}"/>.
    /// </summary>
    public GanttReadDataEventArgs( GanttView view, DateOnly date, DateTime viewStart, DateTime viewEnd, string searchText, string sortField, string sortColumnField, SortDirection sortDirection, CancellationToken cancellationToken )
    {
        View = view;
        Date = date;
        ViewStart = viewStart;
        ViewEnd = viewEnd;
        SearchText = searchText;
        SortField = sortField;
        SortColumnField = sortColumnField;
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
    /// Current sort field when sorting is active.
    /// </summary>
    public string SortField { get; }

    /// <summary>
    /// Field name of the sorted column.
    /// </summary>
    public string SortColumnField { get; }

    /// <summary>
    /// Current sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }

    /// <summary>
    /// Cancellation token for this request.
    /// </summary>
    public CancellationToken CancellationToken { get; }
}