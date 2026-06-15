namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a PivotGrid data column.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridDataColumn<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridDataColumn{TItem}"/>.
    /// </summary>
    /// <param name="column">Column axis item.</param>
    /// <param name="aggregate">Aggregate info.</param>
    public PivotGridDataColumn( PivotGridAxisItem<TItem> column, PivotGridAggregateInfo<TItem> aggregate )
    {
        Column = column;
        Aggregate = aggregate;
    }

    /// <summary>
    /// Gets the column axis item.
    /// </summary>
    public PivotGridAxisItem<TItem> Column { get; }

    /// <summary>
    /// Gets the aggregate info.
    /// </summary>
    public PivotGridAggregateInfo<TItem> Aggregate { get; }
}