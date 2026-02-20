using Blazorise;

namespace Blazorise.DataGrid;

/// <summary>
/// Context for a detail row template.
/// </summary>
/// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
public class DetailRowContext<TItem> : BaseTemplateContext<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DetailRowContext{TItem}"/> class.
    /// </summary>
    /// <param name="item">The row item.</param>
    /// <param name="rowInfo">The row metadata.</param>
    /// <param name="rowIndex">The row index, or -1 when unavailable.</param>
    /// <param name="dataGrid">The parent data grid.</param>
    public DetailRowContext( TItem item, DataGridRowInfo<TItem> rowInfo, int rowIndex, DataGrid<TItem> dataGrid )
        : base( item )
    {
        RowInfo = rowInfo;
        RowIndex = rowIndex;
        DataGrid = dataGrid;
    }

    /// <summary>
    /// Gets the parent data grid.
    /// </summary>
    public DataGrid<TItem> DataGrid { get; }

    /// <summary>
    /// Gets the row metadata.
    /// </summary>
    public DataGridRowInfo<TItem> RowInfo { get; }

    /// <summary>
    /// Gets the row index.
    /// </summary>
    public int RowIndex { get; }

    /// <summary>
    /// Gets a value indicating whether the detail row is expanded.
    /// </summary>
    public bool Expanded => RowInfo?.DetailRowVisible ?? false;
}