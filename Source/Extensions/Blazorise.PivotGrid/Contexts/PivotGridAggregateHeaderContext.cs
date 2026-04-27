#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Context passed to PivotGrid aggregate header templates.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridAggregateHeaderContext<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridAggregateHeaderContext{TItem}"/>.
    /// </summary>
    public PivotGridAggregateHeaderContext( PivotGridAggregate<TItem> aggregate, string caption, PivotGridDataColumn<TItem> dataColumn, IReadOnlyList<object> columnValues, bool isColumnTotal, bool isGrandTotal )
    {
        Aggregate = aggregate;
        Caption = caption;
        DataColumn = dataColumn;
        ColumnValues = columnValues;
        IsColumnTotal = isColumnTotal;
        IsGrandTotal = isGrandTotal;
    }

    /// <summary>
    /// Gets the aggregate field being rendered.
    /// </summary>
    public PivotGridAggregate<TItem> Aggregate { get; }

    /// <summary>
    /// Gets the default aggregate header caption.
    /// </summary>
    public string Caption { get; }

    /// <summary>
    /// Gets the data column represented by this header.
    /// </summary>
    public PivotGridDataColumn<TItem> DataColumn { get; }

    /// <summary>
    /// Gets the column dimension values for this aggregate header.
    /// </summary>
    public IReadOnlyList<object> ColumnValues { get; }

    /// <summary>
    /// Gets whether the aggregate header belongs to a column subtotal.
    /// </summary>
    public bool IsColumnTotal { get; }

    /// <summary>
    /// Gets whether the aggregate header belongs to the grand total column.
    /// </summary>
    public bool IsGrandTotal { get; }
}