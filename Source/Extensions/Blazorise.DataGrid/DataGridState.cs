using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Blazorise.DataGrid;

/// <summary>
/// A DataGrid state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridState<TItem>
{
    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items for each page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the current datagrid editing state.
    /// </summary>
    public DataGridEditState EditState { get; set; }

    /// <summary>
    /// Gets or sets the currently editing item.
    /// </summary>
    public TItem EditItem { get; set; }

    /// <summary>
    /// Gets or sets currently selected row.
    /// </summary>
    public TItem SelectedRow { get; set; }

    /// <summary>
    /// Gets or sets currently selected rows.
    /// </summary>
    public List<TItem> SelectedRows { get; set; }

    /// <summary>
    /// Gets or sets the sort state for the DataGrid columns.
    /// </summary>
    public List<DataGridColumnSortState<TItem>> ColumnSortStates { get; set; }

    /// <summary>
    /// Gets or sets the filter state for the DataGrid columns.
    /// </summary>
    public List<DataGridColumnFilterState<TItem>> ColumnFilterStates { get; set; }

    /// <summary>
    /// Sets the DataGrid to the EditState.New.
    /// </summary>
    public void SetNewState()
    {
        EditState = DataGridEditState.New;
    }

    /// <summary>
    /// Sets the DataGrid to the EditState.Edit and provides the edit item.
    /// </summary>
    /// <param name="editItem"></param>
    public void SetEditState( TItem editItem )
    {
        EditState = DataGridEditState.Edit;
        EditItem = editItem;
    }
    /// <summary>
    /// Adds a new sort state for the DataGrid column.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="sortDirection"></param>
    public void AddSortState( string fieldName, SortDirection sortDirection )
    {
        ColumnSortStates ??= new();
        ColumnSortStates.Add( new DataGridColumnSortState<TItem>( fieldName, sortDirection ) );
    }

    /// <summary>
    /// Adds a new sort state for the DataGrid column.
    /// </summary>
    /// <param name="fieldGetter"></param>
    /// <param name="sortDirection"></param>
    public void AddSortState<TValue>( Expression<Func<TItem, TValue>> fieldGetter, SortDirection sortDirection )
    {
        var fieldName = ExtractFieldName( fieldGetter );
        ColumnSortStates ??= new();
        ColumnSortStates.Add( new DataGridColumnSortState<TItem>( fieldName, sortDirection ) );
    }

    /// <summary>
    /// Adds a new filter state for the DataGrid column.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="searchValue"></param>
    public void AddFilterState( string fieldName, object searchValue )
    {
        ColumnFilterStates ??= new();
        ColumnFilterStates.Add( new DataGridColumnFilterState<TItem>( fieldName, searchValue ) );
    }

    /// <summary>
    /// Adds a new filter state for the DataGrid column.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="fieldGetter"></param>
    /// <param name="searchValue"></param>
    public void AddFilterState<TValue>( Expression<Func<TItem, TValue>> fieldGetter, object searchValue )
    {
        var fieldName = ExtractFieldName( fieldGetter );
        ColumnFilterStates ??= new();
        ColumnFilterStates.Add( new DataGridColumnFilterState<TItem>( fieldName, searchValue ) );
    }

    /// <summary>
    /// Extracts the field name from the expression.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="fieldGetter"></param>
    /// <returns></returns>
    private static string ExtractFieldName<TValue>( Expression<Func<TItem, TValue>> fieldGetter )
    {
        return ( fieldGetter.Body as MemberExpression ).Member.Name;
    }
}

/// <summary>
/// A DataGrid column sort state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridColumnSortState<TItem>
{
    public DataGridColumnSortState( string fieldName, SortDirection sortDirection )
    {
        FieldName = fieldName;
        SortDirection = sortDirection;
    }

    public string FieldName { get; }

    public SortDirection SortDirection { get; }
}

/// <summary>
/// A DataGrid column filter state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridColumnFilterState<TItem>
{
    public DataGridColumnFilterState( string fieldName, object searchValue )
    {
        FieldName = fieldName;
        SearchValue = searchValue;
    }

    public string FieldName { get; }

    public object SearchValue { get; }
}