#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

public class PivotGridResultRow<TItem>
{
    public PivotGridResultRow( PivotGridAxisItem<TItem> row, IReadOnlyList<PivotGridCell<TItem>> cells )
    {
        Row = row;
        Cells = cells;
    }

    public PivotGridAxisItem<TItem> Row { get; }

    public IReadOnlyList<PivotGridCell<TItem>> Cells { get; }

}