#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides information for a native SVG chart point event.
/// </summary>
public sealed class SvgChartPointEventArgs : EventArgs
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
    /// Gets the rendered point bounds.
    /// </summary>
    public SvgChartPointBounds Bounds { get; init; }

    #endregion
}