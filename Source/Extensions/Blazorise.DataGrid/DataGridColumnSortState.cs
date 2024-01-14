namespace Blazorise.DataGrid;

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
