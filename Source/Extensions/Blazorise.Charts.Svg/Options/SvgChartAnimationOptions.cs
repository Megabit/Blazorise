#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines general animation options for native SVG charts.
/// </summary>
public class SvgChartAnimationOptions
{
    #region Properties

    /// <summary>
    /// Defines whether general chart animations are enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Defines the duration used by chart animations.
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds( 400 );

    /// <summary>
    /// Defines the delay before chart animations start.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Defines the easing function used by chart animations.
    /// </summary>
    public SvgChartAnimationEasing Easing { get; set; } = SvgChartAnimationEasing.EaseOut;

    /// <summary>
    /// Defines whether chart elements animate when the chart first renders.
    /// </summary>
    public bool AnimateOnLoad { get; set; } = true;

    /// <summary>
    /// Defines whether chart elements animate when chart data or options update.
    /// </summary>
    public bool AnimateOnUpdate { get; set; } = true;

    /// <summary>
    /// Defines geometry animation options.
    /// </summary>
    public SvgChartGeometryAnimationOptions Geometry { get; set; } = new();

    /// <summary>
    /// Defines opacity animation options.
    /// </summary>
    public SvgChartOpacityAnimationOptions Opacity { get; set; } = new();

    /// <summary>
    /// Defines stroke animation options.
    /// </summary>
    public SvgChartStrokeAnimationOptions Stroke { get; set; } = new();

    /// <summary>
    /// Defines transform animation options.
    /// </summary>
    public SvgChartTransformAnimationOptions Transform { get; set; } = new();

    /// <summary>
    /// Defines path animation options.
    /// </summary>
    public SvgChartPathAnimationOptions Path { get; set; } = new();

    #endregion
}