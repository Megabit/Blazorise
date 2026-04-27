namespace Blazorise.PivotGrid;

public class PivotGridDataColumn<TItem>
{
    public PivotGridDataColumn( PivotGridAxisItem<TItem> column, PivotGridAggregate<TItem> aggregate )
    {
        Column = column;
        Aggregate = aggregate;
    }

    public PivotGridAxisItem<TItem> Column { get; }

    public PivotGridAggregate<TItem> Aggregate { get; }
}