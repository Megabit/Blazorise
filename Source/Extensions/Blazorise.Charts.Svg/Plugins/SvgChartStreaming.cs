#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines declarative streaming behavior for a native SVG chart.
/// </summary>
public class SvgChartStreaming : SvgChartPluginBase
{
    #region Properties

    /// <summary>
    /// Defines whether streaming behavior is enabled.
    /// </summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines the maximum number of data points to keep. When null, points are not trimmed by count.
    /// </summary>
    [Parameter] public int? MaxDataPoints { get; set; }

    /// <summary>
    /// Defines the number of data points visible in the streaming viewport. When null, all retained points are visible.
    /// </summary>
    [Parameter] public int? VisibleDataPoints { get; set; }

    /// <summary>
    /// Defines the maximum time span to keep. When null, points are not trimmed by duration and retention is unlimited unless limited by MaxDataPoints.
    /// </summary>
    [Parameter] public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Defines the axis used for the streaming index.
    /// </summary>
    [Parameter] public SvgChartIndexAxis IndexAxis { get; set; } = SvgChartIndexAxis.X;

    /// <summary>
    /// Defines whether the streaming index axis is reversed.
    /// </summary>
    [Parameter] public bool Reverse { get; set; }

    /// <summary>
    /// Defines streaming animation options.
    /// </summary>
    [Parameter] public SvgChartStreamingAnimationOptions Animation { get; set; } = new();

    /// <summary>
    /// Defines the minimum time between chart redraws while streaming.
    /// </summary>
    [Parameter] public TimeSpan RefreshInterval { get; set; } = TimeSpan.Zero;

    /// <inheritdoc/>
    public override bool RendersContent => false;

    #endregion
}