namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines how time values are mapped on a native SVG chart axis.
/// </summary>
public enum SvgChartTimeScale
{
    /// <summary>
    /// Time values are rendered by their ordinal position with equal spacing.
    /// </summary>
    Ordinal,

    /// <summary>
    /// Time values are rendered on a continuous scale based on elapsed time.
    /// </summary>
    Continuous
}