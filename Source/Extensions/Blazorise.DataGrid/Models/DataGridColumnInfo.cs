#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Holds the basic information about the datagrid column.
/// </summary>
public class DataGridColumnInfo
{
    /// <summary>
    /// Initializes a new instance of column info.
    /// </summary>
    /// <param name="field">Field name.</param>
    /// <param name="searchValue">Current search value.</param>
    /// <param name="sortDirection">Current sort direction.</param>
    /// <param name="sortIndex">Sort index.</param>
    /// <param name="columnType">Current column type.</param>
    /// <param name="sortField">Sort field name.</param>
    /// <param name="filterMethod">Filter method.</param>
    public DataGridColumnInfo( string field, object searchValue, SortDirection sortDirection, int sortIndex, DataGridColumnType columnType, string sortField, DataGridFilterMethod? filterMethod )
    {
        Field = field;
        SearchValue = searchValue;
        SortDirection = sortDirection;
        SortIndex = sortIndex;
        ColumnType = columnType;
        SortField = sortField;
        FilterMethod = filterMethod;
    }

    /// <summary>
    /// Gets the column or datasource field name.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Gets the column or datasource field name that should be considered to sort.
    /// </summary>
    public string SortField { get; }

    /// <summary>
    /// Gets the column search value.
    /// </summary>
    public object SearchValue { get; }

    /// <summary>
    /// Gets the column sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }

    /// <summary>
    /// Gets the index by which the columns should be sorted.
    /// </summary>
    public int SortIndex { get; }

    /// <summary>
    /// Gets the column type.
    /// </summary>
    public DataGridColumnType ColumnType { get; }

    /// <summary>
    /// Gets the column filter method.
    /// </summary>
    public DataGridFilterMethod? FilterMethod { get; }
}