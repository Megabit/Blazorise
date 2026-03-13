namespace Blazorise.DataGrid;

/// <summary>
/// A DataGrid column filter state container.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridColumnFilterState<TItem>
{
    private DataGridColumnFilterMethod? filterMethod;

    private bool hasFilterMethod;

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

    /// <summary>
    /// Gets or sets the column filter method.
    /// </summary>
    public DataGridColumnFilterMethod? FilterMethod
    {
        get => filterMethod;
        set
        {
            filterMethod = value;
            hasFilterMethod = true;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the filter method was explicitly provided.
    /// </summary>
    internal bool HasFilterMethod => hasFilterMethod;
}