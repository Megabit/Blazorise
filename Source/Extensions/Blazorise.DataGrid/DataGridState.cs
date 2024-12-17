#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// A DataGrid state container.
/// </summary>
/// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
public class DataGridState<TItem>
{
    #region Methods

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
    /// Adds a new displaying state for the DataGrid column.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="displaying"></param>
    public void AddDisplayingState( string fieldName, bool displaying )
    {
        ColumnDisplayingStates ??= new();
        ColumnDisplayingStates.Add( new DataGridColumnDisplayingState<TItem>( fieldName, displaying ) );
    }

    /// <summary>
    /// Adds a new displaying state for the DataGrid column.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="fieldGetter"></param>
    /// <param name="displaying"></param>
    public void AddDisplayingState<TValue>( Expression<Func<TItem, TValue>> fieldGetter, bool displaying )
    {
        var fieldName = ExtractFieldName( fieldGetter );
        ColumnDisplayingStates ??= new();
        ColumnDisplayingStates.Add( new DataGridColumnDisplayingState<TItem>( fieldName, displaying ) );
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

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int Page { get; set; }

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
    /// Gets or sets the displaying state for the DataGrid columns.
    /// </summary>
    /// <remarks>
    /// If empty, columns are displayed according to DataGrid configuration.
    /// </remarks>
    public List<DataGridColumnDisplayingState<TItem>> ColumnDisplayingStates { get; set; }

    #endregion
}
