#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides information used to render a native SVG chart tooltip.
/// </summary>
public sealed class SvgChartTooltipContext
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
    /// Gets the tooltip interaction mode.
    /// </summary>
    public SvgChartInteractionMode InteractionMode { get; init; }

    /// <summary>
    /// Gets all points resolved by the tooltip interaction mode.
    /// </summary>
    public IReadOnlyList<SvgChartTooltipItemContext> Items { get; init; } = [];

    /// <summary>
    /// Gets the rendered point bounds.
    /// </summary>
    public SvgChartPointBounds Bounds { get; init; }

    /// <summary>
    /// Gets the resolved point color.
    /// </summary>
    public string Color { get; init; }

    /// <summary>
    /// Gets the default tooltip text.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Gets the tooltip X coordinate for the unscaled SVG viewport. The HTML overlay adjusts its position to the displayed chart size.
    /// </summary>
    public double X { get; init; }

    /// <summary>
    /// Gets the tooltip Y coordinate for the unscaled SVG viewport. The HTML overlay adjusts its position to the displayed chart size.
    /// </summary>
    public double Y { get; init; }

    /// <summary>
    /// Gets the tooltip width in SVG viewport units at the unscaled chart size. The rendered width is unaffected by chart scaling.
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Gets the tooltip height in SVG viewport units at the unscaled chart size. The rendered height is unaffected by chart scaling.
    /// </summary>
    public double Height { get; init; }

    /// <summary>
    /// Gets the source point event arguments.
    /// </summary>
    public SvgChartPointEventArgs Point { get; init; }

    #endregion
}