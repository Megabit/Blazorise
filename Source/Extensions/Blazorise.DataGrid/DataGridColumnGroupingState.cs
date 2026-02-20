namespace Blazorise.DataGrid;

/// <summary>
/// A DataGrid column grouping state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridColumnGroupingState<TItem>
{
    /// <summary>
    /// Initializes a new instance of column grouping state.
    /// </summary>
    /// <param name="fieldName">Field name.</param>
    public DataGridColumnGroupingState( string fieldName )
    {
        FieldName = fieldName;
    }

    /// <summary>
    /// Gets the column or datasource field name.
    /// </summary>
    public string FieldName { get; }
}