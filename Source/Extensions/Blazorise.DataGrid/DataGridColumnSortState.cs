namespace Blazorise.DataGrid;

/// <summary>
/// A DataGrid column sort state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridColumnSortState<TItem>
{
    /// <summary>
    /// Initializes a new instance of column sort state.
    /// </summary>
    /// <param name="fieldName">Field name.</param>
    /// <param name="sortDirection">Current sort direction.</param>
    public DataGridColumnSortState( string fieldName, SortDirection sortDirection )
    {
        FieldName = fieldName;
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Gets the column or datasource field name.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the column sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }
}
