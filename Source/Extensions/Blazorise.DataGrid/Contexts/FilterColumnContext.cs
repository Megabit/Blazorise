using System;
using Microsoft.AspNetCore.Components;

namespace Blazorise.DataGrid;

/// <summary>
/// Context for the Filter Column.
/// </summary>
public class FilterColumnContext<TItem>
{
    #region Constructors

    /// <summary>
    /// Constructor for context.
    /// </summary>
    public FilterColumnContext( DataGridColumn<TItem> column, Func<DataGridColumnFilterMethod> getFilterMethod, Func<object> getSearchValue, EventCallback<object> filterChanged, EventCallback<DataGridColumnFilterMethod?> filterMethodChanged, EventCallback filter, EventCallback clearFilter )
    {
        Column = column;
        GetFilterMethod = getFilterMethod;
        FilterChanged = filterChanged;
        FilterMethodChanged = filterMethodChanged;
        GetSearchValue = getSearchValue;
        Filter = filter;
        ClearFilter = clearFilter;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The column that is being filtered.
    /// </summary>
    public DataGridColumn<TItem> Column { get; set; }

    /// <summary>
    /// Gets the filter method that is being used.
    /// </summary>
    public Func<DataGridColumnFilterMethod> GetFilterMethod { get; set; }

    /// <summary>
    /// Gets the current search value
    /// </summary>
    public Func<object> GetSearchValue { get; set; }

    /// <summary>
    /// Triggers filter changed event.
    /// </summary>
    public EventCallback<object> FilterChanged { get; set; }

    /// <summary>
    /// Triggers filter method changed event.
    /// </summary>
    public EventCallback<DataGridColumnFilterMethod?> FilterMethodChanged { get; set; }

    /// <summary>
    /// Triggers filter event for this column.
    /// </summary>
    public EventCallback Filter { get; set; }

    /// <summary>
    /// Triggers clear filter event for this column.
    /// </summary>
    public EventCallback ClearFilter { get; set; }

    #endregion
}