#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

public class PivotGridCell<TItem>
{
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

    public PivotGridDataColumn<TItem> DataColumn { get; }

    public object Value { get; }

    public string FormattedValue { get; }

    public IReadOnlyList<TItem> Items { get; }

    public bool IsRowTotal { get; }

    public bool IsColumnTotal { get; }

    public bool IsGrandTotal { get; }
}