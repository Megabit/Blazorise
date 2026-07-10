#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the value axis of a native SVG chart.
/// </summary>
public class SvgChartValueAxis : SvgChartComponentBase
{
    #region Members

    private ComponentParameterInfo<string> paramId;

    private ComponentParameterInfo<SvgChartAxisPosition> paramPosition;

    private ComponentParameterInfo<bool> paramBeginAtZero;

    private ComponentParameterInfo<double?> paramMin;

    private ComponentParameterInfo<double?> paramMax;

    private ComponentParameterInfo<int> paramTickCount;

    private ComponentParameterInfo<bool> paramStacked;

    private ComponentParameterInfo<Func<SvgChartAxisTickContext, string>> paramTickFormatter;

    private ComponentParameterInfo<SvgChartGridLinesOptions> paramGridLines;

    private ComponentParameterInfo<SvgChartAxisLabelsOptions> paramLabelsOptions;

    private ComponentParameterInfo<string> paramTitle;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Id, out paramId );
        parameters.TryGetParameter( Position, out paramPosition );
        parameters.TryGetParameter( BeginAtZero, out paramBeginAtZero );
        parameters.TryGetParameter( Min, out paramMin );
        parameters.TryGetParameter( Max, out paramMax );
        parameters.TryGetParameter( TickCount, out paramTickCount );
        parameters.TryGetParameter( Stacked, out paramStacked );
        parameters.TryGetParameter( TickFormatter, out paramTickFormatter );
        parameters.TryGetParameter( GridLines, out paramGridLines );
        parameters.TryGetParameter( LabelsOptions, out paramLabelsOptions );
        parameters.TryGetParameter( Title, out paramTitle );

        return base.SetParametersAsync( parameters );
    }

    protected override void Register()
    {
        Parent?.RegisterValueAxis( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterValueAxis( this );
    }

    internal SvgChartAxisOptions ResolveOptions( SvgChartAxisOptions fallback )
    {
        fallback ??= new();

        return new()
        {
            Id = paramId.GetValueOrDefault( fallback.Id ),
            Position = paramPosition.GetValueOrDefault( fallback.Position ),
            BeginAtZero = paramBeginAtZero.GetValueOrDefault( fallback.BeginAtZero ),
            Min = paramMin.GetValueOrDefault( fallback.Min ),
            Max = paramMax.GetValueOrDefault( fallback.Max ),
            TickCount = paramTickCount.GetValueOrDefault( fallback.TickCount ),
            Stacked = paramStacked.GetValueOrDefault( fallback.Stacked ),
            TickFormatter = paramTickFormatter.GetValueOrDefault( fallback.TickFormatter ),
            GridLines = SvgChartOptionsMapper.CreateGridLinesOptions( fallback.GridLines, paramGridLines ),
            Labels = SvgChartOptionsMapper.CreateLabelsOptions( fallback.Labels, paramLabelsOptions ),
            Title = paramTitle.GetValueOrDefault( fallback.Title )
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the axis identifier used by series to target this value axis.
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// Defines the axis position.
    /// </summary>
    [Parameter] public SvgChartAxisPosition Position { get; set; } = SvgChartAxisPosition.Auto;

    /// <summary>
    /// Defines whether the value axis includes zero.
    /// </summary>
    [Parameter] public bool BeginAtZero { get; set; } = true;

    /// <summary>
    /// Defines a custom minimum value.
    /// </summary>
    [Parameter] public double? Min { get; set; }

    /// <summary>
    /// Defines a custom maximum value.
    /// </summary>
    [Parameter] public double? Max { get; set; }

    /// <summary>
    /// Defines the number of axis ticks.
    /// </summary>
    [Parameter] public int TickCount { get; set; } = 5;

    /// <summary>
    /// Defines whether compatible series are stacked on this axis.
    /// </summary>
    [Parameter] public bool Stacked { get; set; }

    /// <summary>
    /// Defines a callback used to format value axis tick labels.
    /// </summary>
    [Parameter] public Func<SvgChartAxisTickContext, string> TickFormatter { get; set; }

    /// <summary>
    /// Defines grid line options for this value axis.
    /// </summary>
    [Parameter] public SvgChartGridLinesOptions GridLines { get; set; } = new();

    /// <summary>
    /// Defines label options for this value axis.
    /// </summary>
    [Parameter] public SvgChartAxisLabelsOptions LabelsOptions { get; set; }

    /// <summary>
    /// Defines the axis title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    #endregion
}