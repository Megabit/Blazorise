#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a PivotGrid aggregate value cell.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridCell<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridCell{TItem}"/>.
    /// </summary>
    /// <param name="dataColumn">Data column.</param>
    /// <param name="value">Raw aggregate value.</param>
    /// <param name="formattedValue">Formatted aggregate value.</param>
    /// <param name="items">Source items included in the cell.</param>
    /// <param name="isRowTotal">Whether the cell belongs to a row total column.</param>
    /// <param name="isColumnTotal">Whether the cell belongs to a column total row.</param>
    /// <param name="isGrandTotal">Whether the cell is the grand total intersection.</param>
    public PivotGridCell( PivotGridDataColumn<TItem> dataColumn, object value, string formattedValue, IReadOnlyList<TItem> items, bool isRowTotal, bool isColumnTotal, bool isGrandTotal )
    {
        DataColumn = dataColumn;
        Value = value;
        FormattedValue = formattedValue;
        Items = items;
        IsRowTotal = isRowTotal;
        IsColumnTotal = isColumnTotal;
        IsGrandTotal = isGrandTotal;
    }

    /// <summary>
    /// Gets the data column.
    /// </summary>
    public PivotGridDataColumn<TItem> DataColumn { get; }

    /// <summary>
    /// Gets the raw aggregate value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the formatted aggregate value.
    /// </summary>
    public string FormattedValue { get; }

    /// <summary>
    /// Gets source items included in the cell.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Gets whether the cell belongs to a row total column.
    /// </summary>
    public bool IsRowTotal { get; }

    /// <summary>
    /// Gets whether the cell belongs to a column total row.
    /// </summary>
    public bool IsColumnTotal { get; }

    /// <summary>
    /// Gets whether the cell is the grand total intersection.
    /// </summary>
    public bool IsGrandTotal { get; }
}