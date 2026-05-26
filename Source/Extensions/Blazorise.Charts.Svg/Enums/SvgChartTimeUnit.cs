namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the preferred unit for native SVG chart time axis labels.
/// </summary>
public enum SvgChartTimeUnit
{
    /// <summary>
    /// The unit is inferred from the label values.
    /// </summary>
    Auto,

    /// <summary>
    /// Millisecond-level labels.
    /// </summary>
    Millisecond,

    /// <summary>
    /// Second-level labels.
    /// </summary>
    Second,

    /// <summary>
    /// Minute-level labels.
    /// </summary>
    Minute,

    /// <summary>
    /// Hour-level labels.
    /// </summary>
    Hour,

    /// <summary>
    /// Day-level labels.
    /// </summary>
    Day,

    /// <summary>
    /// Month-level labels.
    /// </summary>
    Month,

    /// <summary>
    /// Year-level labels.
    /// </summary>
    Year
}