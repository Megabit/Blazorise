#region Using directives

using System;

#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides all the information about the current column sorting.
/// </summary>
public class DataGridSortChangedEventArgs : EventArgs
{
    /// <summary>
    /// Default constructors.
    /// </summary>
    /// <param name="fieldName">The column's sort field name.</param>
    /// <param name="columnFieldName">The column's field name.</param>
    /// <param name="sortDirection">Column sort direction.</param>
    public DataGridSortChangedEventArgs( string fieldName, string columnFieldName, SortDirection sortDirection )
    {
        FieldName = fieldName;
        ColumnFieldName = columnFieldName;
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Gets the field name used to apply sorting on this column. 
    /// The name is either <see cref="BaseDataGridColumn{TItem}.Field"/> or <see cref="DataGridColumn{TItem}.SortField"/> if specified.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the field name that is defined on this column. The name is always the same as in <see cref="BaseDataGridColumn{TItem}.Field"/>.
    /// </summary>
    public string ColumnFieldName { get; set; }

    /// <summary>
    /// Gets the new sort direction of the specified field name.
    /// </summary>
    public SortDirection SortDirection { get; }
}