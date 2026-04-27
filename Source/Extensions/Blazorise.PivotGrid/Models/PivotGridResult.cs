#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

public class PivotGridResult<TItem>
{
    private static readonly PivotGridResult<TItem> empty = new( [], [], [], [], [] );

    public PivotGridResult( IReadOnlyList<BasePivotGridField<TItem>> rowFields, IReadOnlyList<BasePivotGridField<TItem>> columnFields, IReadOnlyList<PivotGridAggregate<TItem>> aggregates, IReadOnlyList<PivotGridDataColumn<TItem>> dataColumns, IReadOnlyList<PivotGridResultRow<TItem>> rows )
    {
        RowFields = rowFields;
        ColumnFields = columnFields;
        Aggregates = aggregates;
        DataColumns = dataColumns;
        Rows = rows;
    }

    public static PivotGridResult<TItem> Empty => empty;

    public IReadOnlyList<BasePivotGridField<TItem>> RowFields { get; }

    public IReadOnlyList<BasePivotGridField<TItem>> ColumnFields { get; }

    public IReadOnlyList<PivotGridAggregate<TItem>> Aggregates { get; }

    public IReadOnlyList<PivotGridDataColumn<TItem>> DataColumns { get; }

    public IReadOnlyList<PivotGridResultRow<TItem>> Rows { get; }

    public bool HasValues => Aggregates.Count > 0;
}