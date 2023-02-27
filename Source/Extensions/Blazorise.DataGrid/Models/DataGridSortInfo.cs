namespace Blazorise.DataGrid.Models;

/// <summary>
/// Holds the sort order information of a specific column
/// </summary>
public class DataGridSortInfo
{
    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="field">Field name of the column.</param>
    /// <param name="sortField">Sort field name of the column.</param>
    /// <param name="sortDirection"><inheritdoc cref="SortDirection"/></param>
    /// <param name="sortOrder">Sort index of the column.</param>
    public DataGridSortInfo( string field, string sortField, SortDirection sortDirection, int sortOrder )
    {
        Field = field;
        SortField = sortField;
        SortDirection = sortDirection;
        SortOrder = sortOrder;
    }

    /// <summary>
    /// Gets the column or datasource field name.
    /// </summary>
    public string Field { get; init; }

    /// <summary>
    /// Gets the column or datasource field name that should be considered to sort.
    /// </summary>
    public string SortField { get; init; }

    /// <summary>
    /// Gets the column sort direction.
    /// </summary>
    public SortDirection SortDirection { get; init; }

    /// <summary>
    /// Gets the index by which the columns should be sorted.
    /// </summary>
    public int SortOrder { get; init; }
}