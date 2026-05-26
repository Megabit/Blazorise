namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines how SVG chart interactions resolve related points.
/// </summary>
public enum SvgChartInteractionMode
{
    /// <summary>
    /// Resolves only the interacted point.
    /// </summary>
    Nearest,

    /// <summary>
    /// Resolves all visible points at the same point index.
    /// </summary>
    Index,

    /// <summary>
    /// Resolves all visible points in the same series.
    /// </summary>
    Dataset
}