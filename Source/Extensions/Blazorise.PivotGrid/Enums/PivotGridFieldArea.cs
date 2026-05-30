namespace Blazorise.PivotGrid;

/// <summary>
/// Defines where a PivotGrid field participates in the pivot layout.
/// </summary>
public enum PivotGridFieldArea
{
    /// <summary>
    /// Field is available for runtime layout customization.
    /// </summary>
    Available,

    /// <summary>
    /// Field is used as a row dimension.
    /// </summary>
    Row,

    /// <summary>
    /// Field is used as a column dimension.
    /// </summary>
    Column,

    /// <summary>
    /// Field is used as an aggregate value.
    /// </summary>
    Aggregate,

    /// <summary>
    /// Field is used as a filter.
    /// </summary>
    Filter
}