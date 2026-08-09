namespace Blazorise.Reporting;

/// <summary>
/// Defines aggregate functions available for data-bound report fields.
/// </summary>
public enum ReportAggregateFunction
{
    /// <summary>
    /// Counts resolved field values.
    /// </summary>
    Count,

    /// <summary>
    /// Sums numeric field values.
    /// </summary>
    Sum,

    /// <summary>
    /// Calculates the average of numeric field values.
    /// </summary>
    Average,

    /// <summary>
    /// Resolves the smallest comparable field value.
    /// </summary>
    Minimum,

    /// <summary>
    /// Resolves the largest comparable field value.
    /// </summary>
    Maximum
}