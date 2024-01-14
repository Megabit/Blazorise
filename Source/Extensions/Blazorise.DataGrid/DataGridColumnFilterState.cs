namespace Blazorise.DataGrid;

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