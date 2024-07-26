namespace Blazorise.DataGrid;

/// <summary>
/// A DataGrid column filter state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridColumnFilterState<TItem>
{
    /// <summary>
    /// Initializes a new instance of column filter state.
    /// </summary>
    /// <param name="fieldName">Field name.</param>
    /// <param name="searchValue">Current search value.</param>
    public DataGridColumnFilterState( string fieldName, object searchValue )
    {
        FieldName = fieldName;
        SearchValue = searchValue;
    }

    /// <summary>
    /// Gets the column or datasource field name.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the column search value.
    /// </summary>
    public object SearchValue { get; }
}
