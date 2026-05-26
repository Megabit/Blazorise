namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides information for one item shown in a native SVG chart tooltip.
/// </summary>
public sealed class SvgChartTooltipItemContext
{
    #region Properties

    /// <summary>
    /// Gets the series name.
    /// </summary>
    public string SeriesName { get; init; }

    /// <summary>
    /// Gets the zero-based series index.
    /// </summary>
    public int SeriesIndex { get; init; }

    /// <summary>
    /// Gets the zero-based point index within the series.
    /// </summary>
    public int PointIndex { get; init; }

    /// <summary>
    /// Gets the category or X value associated with the point.
    /// </summary>
    public object Category { get; init; }

    /// <summary>
    /// Gets the point value.
    /// </summary>
    public object Value { get; init; }

    /// <summary>
    /// Gets the resolved point color.
    /// </summary>
    public string Color { get; init; }

    /// <summary>
    /// Gets the source point event arguments.
    /// </summary>
    public SvgChartPointEventArgs Point { get; init; }

    #endregion
}