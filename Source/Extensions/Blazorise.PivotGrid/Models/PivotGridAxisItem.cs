#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

public class PivotGridAxisItem<TItem>
{
    public PivotGridAxisItem( IReadOnlyList<object> values, IReadOnlyList<TItem> items, int level, bool isTotal, bool isGrandTotal )
    {
        Values = values;
        Items = items;
        Level = level;
        IsTotal = isTotal;
        IsGrandTotal = isGrandTotal;
    }

    public IReadOnlyList<object> Values { get; }

    public IReadOnlyList<TItem> Items { get; }

    public int Level { get; }

    public bool IsTotal { get; }

    public bool IsGrandTotal { get; }
}