#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Event arguments for PivotGrid cell click events.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridCellClickedEventArgs<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridCellClickedEventArgs{TItem}"/>.
    /// </summary>
    public PivotGridCellClickedEventArgs( PivotGridAggregate<TItem> aggregate, object value, IReadOnlyList<object> rowValues, IReadOnlyList<object> columnValues, IReadOnlyList<TItem> items )
    {
        Aggregate = aggregate;
        Value = value;
        RowValues = rowValues;
        ColumnValues = columnValues;
        Items = items;
    }

    /// <summary>
    /// Gets the aggregate field that produced the clicked cell.
    /// </summary>
    public PivotGridAggregate<TItem> Aggregate { get; }

    /// <summary>
    /// Gets the raw aggregate value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets row dimension values for the clicked cell.
    /// </summary>
    public IReadOnlyList<object> RowValues { get; }

    /// <summary>
    /// Gets column dimension values for the clicked cell.
    /// </summary>
    public IReadOnlyList<object> ColumnValues { get; }

    /// <summary>
    /// Gets source items included in the clicked cell.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; }
}