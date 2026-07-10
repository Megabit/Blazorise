#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines declarative animation behavior for a native SVG chart.
/// </summary>
public class SvgChartAnimation : SvgChartPluginBase
{
    #region Members

    private ComponentParameterInfo<bool> paramEnabled;

    private ComponentParameterInfo<TimeSpan> paramDuration;

    private ComponentParameterInfo<TimeSpan> paramDelay;

    private ComponentParameterInfo<SvgChartAnimationEasing> paramEasing;

    private ComponentParameterInfo<bool> paramAnimateOnLoad;

    private ComponentParameterInfo<bool> paramAnimateOnUpdate;

    private ComponentParameterInfo<SvgChartGeometryAnimationOptions> paramGeometry;

    private ComponentParameterInfo<SvgChartOpacityAnimationOptions> paramOpacity;

    private ComponentParameterInfo<SvgChartStrokeAnimationOptions> paramStroke;

    private ComponentParameterInfo<SvgChartTransformAnimationOptions> paramTransform;

    private ComponentParameterInfo<SvgChartPathAnimationOptions> paramPath;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Enabled, out paramEnabled );
        parameters.TryGetParameter( Duration, out paramDuration );
        parameters.TryGetParameter( Delay, out paramDelay );
        parameters.TryGetParameter( Easing, out paramEasing );
        parameters.TryGetParameter( AnimateOnLoad, out paramAnimateOnLoad );
        parameters.TryGetParameter( AnimateOnUpdate, out paramAnimateOnUpdate );
        parameters.TryGetParameter( Geometry, out paramGeometry );
        parameters.TryGetParameter( Opacity, out paramOpacity );
        parameters.TryGetParameter( Stroke, out paramStroke );
        parameters.TryGetParameter( Transform, out paramTransform );
        parameters.TryGetParameter( Path, out paramPath );

        return base.SetParametersAsync( parameters );
    }

    internal SvgChartAnimationOptions ResolveOptions( SvgChartAnimationOptions fallback )
    {
        fallback ??= new();

        return new()
        {
            Enabled = paramEnabled.GetValueOrDefault( fallback.Enabled ),
            Duration = paramDuration.GetValueOrDefault( fallback.Duration ),
            Delay = paramDelay.GetValueOrDefault( fallback.Delay ),
            Easing = paramEasing.GetValueOrDefault( fallback.Easing ),
            AnimateOnLoad = paramAnimateOnLoad.GetValueOrDefault( fallback.AnimateOnLoad ),
            AnimateOnUpdate = paramAnimateOnUpdate.GetValueOrDefault( fallback.AnimateOnUpdate ),
            Geometry = paramGeometry.GetValueOrDefault( fallback.Geometry ),
            Opacity = paramOpacity.GetValueOrDefault( fallback.Opacity ),
            Stroke = paramStroke.GetValueOrDefault( fallback.Stroke ),
            Transform = paramTransform.GetValueOrDefault( fallback.Transform ),
            Path = paramPath.GetValueOrDefault( fallback.Path )
        };
    }

    #endregion

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