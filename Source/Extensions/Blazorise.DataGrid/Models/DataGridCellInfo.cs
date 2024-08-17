#region Using directives
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Holds the basic information about the datagrid cell.
/// </summary>
public class DataGridCellInfo<TItem>
{

    /// <summary>
    /// Initializes a new instance of cell info.
    /// </summary>
    /// <param name="item">Row Item</param>
    /// <param name="rowInfo">Row Info</param>
    /// <param name="column">Column</param>
    /// <param name="columnInfo">Column Info</param>
    /// <param name="rowIndex">Row Index</param>
    public DataGridCellInfo( TItem item, DataGridRowInfo<TItem> rowInfo, DataGridColumn<TItem> column, DataGridColumnInfo columnInfo, int rowIndex )
    {
        Item = item;
        RowInfo = rowInfo;
        Column = column;
        ColumnInfo = columnInfo;
        RowIndex = rowIndex;
    }

    public TItem Item { get; }

    public int RowIndex { get; }
    public DataGridRowInfo<TItem> RowInfo { get; }

    public DataGridColumnInfo ColumnInfo { get; }

    public DataGridColumn<TItem> Column { get; }
    
}
