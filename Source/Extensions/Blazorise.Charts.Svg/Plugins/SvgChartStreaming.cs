#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines declarative streaming behavior for a native SVG chart.
/// </summary>
public class SvgChartStreaming : SvgChartPluginBase
{
    #region Members

    private ComponentParameterInfo<bool> paramEnabled;

    private ComponentParameterInfo<int?> paramMaxDataPoints;

    private ComponentParameterInfo<int?> paramVisibleDataPoints;

    private ComponentParameterInfo<TimeSpan?> paramDuration;

    private ComponentParameterInfo<SvgChartIndexAxis> paramIndexAxis;

    private ComponentParameterInfo<bool> paramReverse;

    private ComponentParameterInfo<SvgChartStreamingAnimationOptions> paramAnimation;

    private ComponentParameterInfo<TimeSpan> paramRefreshInterval;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Enabled, out paramEnabled );
        parameters.TryGetParameter( MaxDataPoints, out paramMaxDataPoints );
        parameters.TryGetParameter( VisibleDataPoints, out paramVisibleDataPoints );
        parameters.TryGetParameter( Duration, out paramDuration );
        parameters.TryGetParameter( IndexAxis, out paramIndexAxis );
        parameters.TryGetParameter( Reverse, out paramReverse );
        parameters.TryGetParameter( Animation, out paramAnimation );
        parameters.TryGetParameter( RefreshInterval, out paramRefreshInterval );

        return base.SetParametersAsync( parameters );
    }

    internal SvgChartStreamingOptions ResolveOptions( SvgChartStreamingOptions fallback )
    {
        fallback ??= new();

        return new()
        {
            Enabled = paramEnabled.GetValueOrDefault( fallback.Enabled ),
            MaxDataPoints = paramMaxDataPoints.GetValueOrDefault( fallback.MaxDataPoints ),
            VisibleDataPoints = paramVisibleDataPoints.GetValueOrDefault( fallback.VisibleDataPoints ),
            Duration = paramDuration.GetValueOrDefault( fallback.Duration ),
            IndexAxis = paramIndexAxis.GetValueOrDefault( fallback.IndexAxis ),
            Reverse = paramReverse.GetValueOrDefault( fallback.Reverse ),
            Animation = paramAnimation.GetValueOrDefault( fallback.Animation ),
            RefreshInterval = paramRefreshInterval.GetValueOrDefault( fallback.RefreshInterval )
        };
    }

    #endregion

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