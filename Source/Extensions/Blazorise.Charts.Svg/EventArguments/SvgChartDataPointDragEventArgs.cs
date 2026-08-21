#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides information for a native SVG chart data point drag event.
/// </summary>
public sealed class SvgChartDataPointDragEventArgs : EventArgs
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
    /// Gets the category associated with the point.
    /// </summary>
    public object Category { get; init; }

    /// <summary>
    /// Gets the X value at the beginning of the drag, when the series has a continuous X value.
    /// </summary>
    public double? OriginalXValue { get; init; }

    /// <summary>
    /// Gets the Y value at the beginning of the drag.
    /// </summary>
    public double? OriginalYValue { get; init; }

    /// <summary>
    /// Gets the current X value, when the series has a continuous X value.
    /// </summary>
    public double? XValue { get; init; }

    /// <summary>
    /// Gets the current Y value.
    /// </summary>
    public double? YValue { get; init; }

    /// <summary>
    /// Gets the configured drag mode.
    /// </summary>
    public SvgChartDataDragMode Mode { get; init; }

    /// <summary>
    /// Gets whether the drag was canceled and the original values were restored.
    /// </summary>
    public bool Canceled { get; init; }

    #endregion
}