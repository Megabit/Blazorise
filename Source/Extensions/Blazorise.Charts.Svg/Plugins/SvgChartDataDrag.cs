#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines declarative data point dragging behavior for a native SVG chart.
/// </summary>
public class SvgChartDataDrag : SvgChartPluginBase
{
    #region Members

    private ComponentParameterInfo<bool> paramEnabled;

    private ComponentParameterInfo<SvgChartDataDragMode> paramMode;

    private ComponentParameterInfo<double?> paramXStep;

    private ComponentParameterInfo<double?> paramYStep;

    private ComponentParameterInfo<double> paramHitRadius;

    private ComponentParameterInfo<bool> paramShowTooltip;

    private ComponentParameterInfo<Func<SvgChartPointEventArgs, bool>> paramCanDrag;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Enabled, out paramEnabled );
        parameters.TryGetParameter( Mode, out paramMode );
        parameters.TryGetParameter( XStep, out paramXStep );
        parameters.TryGetParameter( YStep, out paramYStep );
        parameters.TryGetParameter( HitRadius, out paramHitRadius );
        parameters.TryGetParameter( ShowTooltip, out paramShowTooltip );
        parameters.TryGetParameter( CanDrag, out paramCanDrag );

        return base.SetParametersAsync( parameters );
    }

    internal SvgChartDataDragOptions ResolveOptions( SvgChartDataDragOptions fallback )
    {
        fallback ??= new();

        return new()
        {
            Enabled = paramEnabled.GetValueOrDefault( fallback.Enabled ),
            Mode = paramMode.GetValueOrDefault( fallback.Mode ),
            XStep = paramXStep.GetValueOrDefault( fallback.XStep ),
            YStep = paramYStep.GetValueOrDefault( fallback.YStep ),
            HitRadius = paramHitRadius.GetValueOrDefault( fallback.HitRadius ),
            ShowTooltip = paramShowTooltip.GetValueOrDefault( fallback.ShowTooltip ),
            CanDrag = paramCanDrag.GetValueOrDefault( fallback.CanDrag )
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether data point dragging is enabled.
    /// </summary>
    [Parameter] public bool Enabled { get; set; }

    /// <summary>
    /// Defines the axes along which data points can be dragged.
    /// </summary>
    [Parameter] public SvgChartDataDragMode Mode { get; set; } = SvgChartDataDragMode.Y;

    /// <summary>
    /// Defines the optional increment to which dragged X values are snapped.
    /// </summary>
    [Parameter] public double? XStep { get; set; }

    /// <summary>
    /// Defines the optional increment to which dragged Y values are snapped.
    /// </summary>
    [Parameter] public double? YStep { get; set; }

    /// <summary>
    /// Defines the minimum pointer hit radius in SVG units.
    /// </summary>
    [Parameter] public double HitRadius { get; set; } = 12;

    /// <summary>
    /// Defines whether the point tooltip is shown and updated while dragging. The default is <see langword="false"/>.
    /// </summary>
    [Parameter] public bool ShowTooltip { get; set; }

    /// <summary>
    /// Defines an optional predicate that determines whether an individual point can be dragged.
    /// </summary>
    [Parameter] public Func<SvgChartPointEventArgs, bool> CanDrag { get; set; }

    /// <summary>
    /// Occurs when a data point drag starts.
    /// </summary>
    [Parameter] public EventCallback<SvgChartDataPointDragEventArgs> DragStarted { get; set; }

    /// <summary>
    /// Occurs when a dragged data point value changes.
    /// </summary>
    [Parameter] public EventCallback<SvgChartDataPointDragEventArgs> Dragging { get; set; }

    /// <summary>
    /// Occurs when a data point drag ends or is canceled.
    /// </summary>
    [Parameter] public EventCallback<SvgChartDataPointDragEventArgs> DragEnded { get; set; }

    /// <inheritdoc/>
    public override bool RendersContent => false;

    #endregion
}