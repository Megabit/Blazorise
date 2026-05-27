#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines streaming options for a native SVG chart.
/// </summary>
public class SvgChartStreamingOptions
{
    #region Properties

    /// <summary>
    /// Defines whether streaming behavior is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines the maximum number of data points to keep. When null, points are not trimmed by count.
    /// </summary>
    public int? MaxDataPoints { get; set; }

    /// <summary>
    /// Defines the number of data points visible in the streaming viewport. When null, all retained points are visible.
    /// </summary>
    public int? VisibleDataPoints { get; set; }

    /// <summary>
    /// Defines the maximum time span to keep. When null, points are not trimmed by duration and retention is unlimited unless limited by MaxDataPoints.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Defines the axis used for the streaming index.
    /// </summary>
    public SvgChartIndexAxis IndexAxis { get; set; } = SvgChartIndexAxis.X;

    /// <summary>
    /// Defines whether the streaming index axis is reversed.
    /// </summary>
    public bool Reverse { get; set; }

    /// <summary>
    /// Defines streaming animation options.
    /// </summary>
    public SvgChartStreamingAnimationOptions Animation { get; set; } = new();

    /// <summary>
    /// Defines the minimum time between chart redraws while streaming.
    /// </summary>
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.Zero;

    #endregion
}