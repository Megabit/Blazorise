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
    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public List<DataGridColumnSortState<TItem>> ColumnSortStates { get; set; }

    public List<DataGridColumnFilterState<TItem>> ColumnFilterStates { get; set; }

    public void AddSortState( string fieldName, SortDirection sortDirection )
    {
        ColumnSortStates ??= new();
        ColumnSortStates.Add( new DataGridColumnSortState<TItem>( fieldName, sortDirection ) );
    }

    public void AddSortState<TValue>( Expression<Func<TItem, TValue>> fieldGetter, SortDirection sortDirection )
    {
        var fieldName = ExtractFieldName( fieldGetter );
        ColumnSortStates ??= new();
        ColumnSortStates.Add( new DataGridColumnSortState<TItem>( fieldName, sortDirection ) );
    }

    public void AddFilterState( string fieldName, object searchValue )
    {
        ColumnFilterStates ??= new();
        ColumnFilterStates.Add( new DataGridColumnFilterState<TItem>( fieldName, searchValue ) );
    }

    public void AddFilterState<TValue>( Expression<Func<TItem, TValue>> fieldGetter, object searchValue )
    {
        var fieldName = ExtractFieldName( fieldGetter );
        ColumnFilterStates ??= new();
        ColumnFilterStates.Add( new DataGridColumnFilterState<TItem>( fieldName, searchValue ) );
    }

    private string ExtractFieldName<TValue>( Expression<Func<TItem, TValue>> fieldGetter )
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