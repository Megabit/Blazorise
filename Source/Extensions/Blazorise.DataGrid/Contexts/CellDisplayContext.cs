namespace Blazorise.DataGrid;

/// <summary>
/// Context for display templates in a datagrid cell.
/// </summary>
/// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
public class CellDisplayContext<TItem> : BaseTemplateContext<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CellDisplayContext{TItem}"/> class.
    /// </summary>
    /// <param name="item">The row item.</param>
    /// <param name="column">The column associated with the cell.</param>
    /// <param name="rowInfo">The row metadata.</param>
    /// <param name="rowIndex">The row index, or -1 when unavailable.</param>
    /// <param name="value">The raw cell value.</param>
    /// <param name="displayValue">The formatted cell value.</param>
    /// <param name="dataGrid">The parent data grid.</param>
    public CellDisplayContext( TItem item, DataGridColumn<TItem> column, DataGridRowInfo<TItem> rowInfo, int rowIndex, object value, string displayValue, DataGrid<TItem> dataGrid )
        : base( item )
    {
        Column = column;
        RowInfo = rowInfo;
        RowIndex = rowIndex;
        Value = value;
        DisplayValue = displayValue;
        DataGrid = dataGrid;
    }

    /// <summary>
    /// Gets the parent data grid.
    /// </summary>
    public DataGrid<TItem> DataGrid { get; }

    /// <summary>
    /// Gets the column associated with the cell.
    /// </summary>
    public DataGridColumn<TItem> Column { get; }

    /// <summary>
    /// Gets the row metadata.
    /// </summary>
    public DataGridRowInfo<TItem> RowInfo { get; }

    /// <summary>
    /// Gets the row index.
    /// </summary>
    public int RowIndex { get; }

    /// <summary>
    /// Gets the raw cell value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the formatted cell value.
    /// </summary>
    public string DisplayValue { get; }
}