#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines declarative animation behavior for a native SVG chart.
/// </summary>
public class SvgChartAnimation : SvgChartPluginBase
{
    #region Properties

    /// <summary>
    /// Defines whether general chart animations are enabled.
    /// </summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines the duration used by chart animations.
    /// </summary>
    [Parameter] public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds( 400 );

    /// <summary>
    /// Defines the delay before chart animations start.
    /// </summary>
    [Parameter] public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Defines the easing function used by chart animations.
    /// </summary>
    [Parameter] public SvgChartAnimationEasing Easing { get; set; } = SvgChartAnimationEasing.EaseOut;

    /// <summary>
    /// Defines whether chart elements animate when the chart first renders.
    /// </summary>
    [Parameter] public bool AnimateOnLoad { get; set; } = true;

    /// <summary>
    /// Defines whether chart elements animate when chart data or options update.
    /// </summary>
    [Parameter] public bool AnimateOnUpdate { get; set; } = true;

    /// <summary>
    /// Defines geometry animation options.
    /// </summary>
    [Parameter] public SvgChartGeometryAnimationOptions Geometry { get; set; } = new();

    /// <summary>
    /// Defines opacity animation options.
    /// </summary>
    [Parameter] public SvgChartOpacityAnimationOptions Opacity { get; set; } = new();

    /// <summary>
    /// Defines stroke animation options.
    /// </summary>
    [Parameter] public SvgChartStrokeAnimationOptions Stroke { get; set; } = new();

    /// <summary>
    /// Defines transform animation options.
    /// </summary>
    [Parameter] public SvgChartTransformAnimationOptions Transform { get; set; } = new();

    /// <summary>
    /// Defines path animation options.
    /// </summary>
    [Parameter] public SvgChartPathAnimationOptions Path { get; set; } = new();

    /// <inheritdoc/>
    public override bool RendersContent => false;

    #endregion
}