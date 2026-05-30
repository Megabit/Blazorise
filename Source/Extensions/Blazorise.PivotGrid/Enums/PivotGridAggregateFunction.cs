namespace Blazorise.PivotGrid;

/// <summary>
/// Defines the built-in aggregate operations supported by <see cref="PivotGrid{TItem}"/>.
/// </summary>
public enum PivotGridAggregateFunction
{
    /// <summary>
    /// Counts all records in the cell.
    /// </summary>
    Count,

    /// <summary>
    /// Counts records that contain a non-null value in the aggregate field.
    /// </summary>
    CountNonNull,

    /// <summary>
    /// Sums numeric values.
    /// </summary>
    Sum,

    /// <summary>
    /// Averages numeric values.
    /// </summary>
    Average,

    /// <summary>
    /// Finds the lowest comparable value.
    /// </summary>
    Min,

    /// <summary>
    /// Finds the highest comparable value.
    /// </summary>
    Max
}