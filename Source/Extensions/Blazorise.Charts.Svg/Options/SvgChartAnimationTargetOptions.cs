#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines common animation options for a native SVG chart animation target.
/// </summary>
public abstract class SvgChartAnimationTargetOptions
{
    #region Properties

    /// <summary>
    /// Defines whether animations for the target are enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines the target-specific animation duration. When not set, the chart animation duration is used.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Defines the target-specific animation delay. When not set, the chart animation delay is used.
    /// </summary>
    public TimeSpan? Delay { get; set; }

    /// <summary>
    /// Defines the target-specific animation easing. When not set, the chart animation easing is used.
    /// </summary>
    public SvgChartAnimationEasing? Easing { get; set; }

    /// <summary>
    /// Defines whether the target animates when the chart first renders. When not set, the chart animation setting is used.
    /// </summary>
    public bool? AnimateOnLoad { get; set; }

    /// <summary>
    /// Defines whether the target animates when chart data or options update. When not set, the chart animation setting is used.
    /// </summary>
    public bool? AnimateOnUpdate { get; set; }

    #endregion
}