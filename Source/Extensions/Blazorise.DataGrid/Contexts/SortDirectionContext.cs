namespace Blazorise.DataGrid;

/// <summary>
/// Context for the sort direction template.
/// </summary>
/// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
public class SortDirectionContext<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortDirectionContext{TItem}"/> class.
    /// </summary>
    /// <param name="dataGrid">The parent data grid.</param>
    /// <param name="column">The column associated with the sort.</param>
    /// <param name="sortDirection">The current sort direction.</param>
    public SortDirectionContext( DataGrid<TItem> dataGrid, DataGridColumn<TItem> column, SortDirection sortDirection )
    {
        DataGrid = dataGrid;
        Column = column;
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Gets the parent data grid.
    /// </summary>
    public DataGrid<TItem> DataGrid { get; }

    /// <summary>
    /// Gets the column associated with the sort.
    /// </summary>
    public DataGridColumn<TItem> Column { get; }

    /// <summary>
    /// Gets the current sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }

    /// <summary>
    /// Gets the current sort mode.
    /// </summary>
    public DataGridSortMode SortMode => DataGrid?.SortMode ?? DataGridSortMode.Single;

    /// <summary>
    /// Gets the sort order of the column.
    /// </summary>
    public int SortOrder => Column?.SortOrder ?? default;
}