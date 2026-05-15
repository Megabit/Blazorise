#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a prepared PivotGrid result.
/// </summary>
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
    /// Gets data columns.
    /// </summary>
    public IReadOnlyList<PivotGridDataColumn<TItem>> DataColumns { get; }

    /// <summary>
    /// Gets result rows.
    /// </summary>
    public IReadOnlyList<PivotGridResultRow<TItem>> Rows { get; }

    /// <summary>
    /// Gets whether the result contains aggregate info.
    /// </summary>
    public bool HasValues => Aggregates.Count > 0;
}