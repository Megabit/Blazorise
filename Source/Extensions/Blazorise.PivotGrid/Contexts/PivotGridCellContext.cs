#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Context passed to PivotGrid value cell templates.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridCellContext<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridCellContext{TItem}"/>.
    /// </summary>
    public PivotGridCellContext( PivotGridAggregate<TItem> aggregate, object value, string formattedValue, IReadOnlyList<object> rowValues, IReadOnlyList<object> columnValues, IReadOnlyList<TItem> items, bool isRowTotal, bool isColumnTotal, bool isGrandTotal )
    {
        Aggregate = aggregate;
        Value = value;
        FormattedValue = formattedValue;
        RowValues = rowValues;
        ColumnValues = columnValues;
        Items = items;
        IsRowTotal = isRowTotal;
        IsColumnTotal = isColumnTotal;
        IsGrandTotal = isGrandTotal;
    }

    /// <summary>
    /// Gets the aggregate field that produced this cell.
    /// </summary>
    public PivotGridAggregate<TItem> Aggregate { get; }

    /// <summary>
    /// Gets the raw aggregate value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the formatted aggregate value.
    /// </summary>
    public string FormattedValue { get; }

    /// <summary>
    /// Gets row dimension values for this cell.
    /// </summary>
    public IReadOnlyList<object> RowValues { get; }

    /// <summary>
    /// Gets column dimension values for this cell.
    /// </summary>
    public IReadOnlyList<object> ColumnValues { get; }

    /// <summary>
    /// Gets source items included in this cell.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Gets whether the cell belongs to a row subtotal or grand total.
    /// </summary>
    public bool IsRowTotal { get; }

    /// <summary>
    /// Gets whether the cell belongs to a column subtotal or grand total.
    /// </summary>
    public bool IsColumnTotal { get; }

    /// <summary>
    /// Gets whether the cell is the grand total intersection.
    /// </summary>
    public bool IsGrandTotal { get; }
}