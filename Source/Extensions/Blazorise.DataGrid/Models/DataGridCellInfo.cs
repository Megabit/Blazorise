namespace Blazorise.DataGrid;

/// <summary>
/// Represents information about a cell in a data grid.
/// </summary>
/// <typeparam name="TItem">The type of the item represented by the row.</typeparam>
public class DataGridCellInfo<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridCellInfo{TItem}"/> class.
    /// </summary>
    /// <param name="item">The item associated with the cell.</param>
    /// <param name="rowInfo">The information about the row that contains the cell.</param>
    /// <param name="column">The column in which the cell is located.</param>
    /// <param name="columnInfo">The information about the column that contains the cell.</param>
    /// <param name="rowIndex">The index of the row that contains the cell.</param>
    public DataGridCellInfo( TItem item, DataGridRowInfo<TItem> rowInfo, DataGridColumn<TItem> column, DataGridColumnInfo columnInfo, int rowIndex )
    {
        Item = item;
        RowInfo = rowInfo;
        Column = column;
        ColumnInfo = columnInfo;
        RowIndex = rowIndex;
    }

    /// <summary>
    /// Gets the item associated with the cell.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the index of the row that contains the cell.
    /// </summary>
    public int RowIndex { get; }

    /// <summary>
    /// Gets the information about the row that contains the cell.
    /// </summary>
    public DataGridRowInfo<TItem> RowInfo { get; }

    /// <summary>
    /// Gets the information about the column that contains the cell.
    /// </summary>
    public DataGridColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Gets the column in which the cell is located.
    /// </summary>
    public DataGridColumn<TItem> Column { get; }
}