#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a prepared PivotGrid result.
/// </summary>
/// <remarks>
/// When returned by an external data provider, the result must keep a consistent table shape:
/// <see cref="DataColumns"/> defines the cell order, and every item in <see cref="Rows"/> must contain one cell for every data column in the same order.
/// During virtualization, data columns and aggregate metadata must remain stable across requests.
/// Row fields, column fields, aggregate info, data columns, rows, and cells should not be <c>null</c>. PivotGrid will normalize inconsistent provider results before rendering.
/// </remarks>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridResult<TItem>
{
    private static readonly PivotGridResult<TItem> empty = new( [], [], [], [], [] );

    /// <summary>
    /// Initializes a new <see cref="PivotGridResult{TItem}"/>.
    /// </summary>
    /// <param name="rowFields">Row field info.</param>
    /// <param name="columnFields">Column field info.</param>
    /// <param name="aggregates">Aggregate info.</param>
    /// <param name="dataColumns">Data columns.</param>
    /// <param name="rows">Result rows.</param>
    public PivotGridResult( IReadOnlyList<PivotGridFieldInfo<TItem>> rowFields, IReadOnlyList<PivotGridFieldInfo<TItem>> columnFields, IReadOnlyList<PivotGridAggregateInfo<TItem>> aggregates, IReadOnlyList<PivotGridDataColumn<TItem>> dataColumns, IReadOnlyList<PivotGridResultRow<TItem>> rows )
    {
        RowFields = rowFields;
        ColumnFields = columnFields;
        Aggregates = aggregates;
        DataColumns = dataColumns;
        Rows = rows;
    }

    /// <summary>
    /// Gets an empty PivotGrid result.
    /// </summary>
    public static PivotGridResult<TItem> Empty => empty;

    /// <summary>
    /// Gets row field info.
    /// </summary>
    public IReadOnlyList<PivotGridFieldInfo<TItem>> RowFields { get; }

    /// <summary>
    /// Gets column field info.
    /// </summary>
    public IReadOnlyList<PivotGridFieldInfo<TItem>> ColumnFields { get; }

    /// <summary>
    /// Gets aggregate info.
    /// </summary>
    public IReadOnlyList<PivotGridAggregateInfo<TItem>> Aggregates { get; }

    /// <summary>
    /// Gets data columns. The order of these columns defines the required order of each row's cells.
    /// </summary>
    public IReadOnlyList<PivotGridDataColumn<TItem>> DataColumns { get; }

    /// <summary>
    /// Gets result rows. Each row must contain the same number of cells as <see cref="DataColumns"/>.
    /// </summary>
    public IReadOnlyList<PivotGridResultRow<TItem>> Rows { get; }

    /// <summary>
    /// Gets whether the result contains aggregate info.
    /// </summary>
    public bool HasValues => Aggregates.Count > 0;
}