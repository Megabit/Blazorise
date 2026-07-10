#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the category axis of a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
public class SvgChartCategoryAxis<TItem> : SvgChartComponentBase
{
    #region Members

    private ComponentParameterInfo<string> paramId;

    private ComponentParameterInfo<SvgChartAxisPosition> paramPosition;

    private ComponentParameterInfo<string> paramTitle;

    private ComponentParameterInfo<Func<SvgChartAxisTickContext, string>> paramTickFormatter;

    private ComponentParameterInfo<SvgChartGridLinesOptions> paramGridLines;

    private ComponentParameterInfo<SvgChartAxisLabelsOptions> paramLabelsOptions;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Id, out paramId );
        parameters.TryGetParameter( Position, out paramPosition );
        parameters.TryGetParameter( Title, out paramTitle );
        parameters.TryGetParameter( TickFormatter, out paramTickFormatter );
        parameters.TryGetParameter( GridLines, out paramGridLines );
        parameters.TryGetParameter( LabelsOptions, out paramLabelsOptions );

        return base.SetParametersAsync( parameters );
    }

    protected override void Register()
    {
        Parent?.RegisterCategoryAxis( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterCategoryAxis( this );
    }

    internal SvgChartAxisOptions ResolveOptions( SvgChartAxisOptions fallback )
    {
        fallback ??= new();

        return new()
        {
            Id = paramId.GetValueOrDefault( fallback.Id ),
            Position = paramPosition.GetValueOrDefault( fallback.Position ),
            BeginAtZero = fallback.BeginAtZero,
            Min = fallback.Min,
            Max = fallback.Max,
            TickCount = fallback.TickCount,
            Stacked = fallback.Stacked,
            TickFormatter = paramTickFormatter.GetValueOrDefault( fallback.TickFormatter ),
            GridLines = SvgChartOptionsMapper.CreateGridLinesOptions( fallback.GridLines, paramGridLines ),
            Labels = SvgChartOptionsMapper.CreateLabelsOptions( fallback.Labels, paramLabelsOptions ),
            Title = paramTitle.GetValueOrDefault( fallback.Title )
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the axis identifier used by series to target this category axis.
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// Defines the axis position.
    /// </summary>
    [Parameter] public SvgChartAxisPosition Position { get; set; } = SvgChartAxisPosition.Auto;

    /// <summary>
    /// Defines explicit category labels.
    /// </summary>
    [Parameter] public IReadOnlyList<object> Labels { get; set; }

    /// <summary>
    /// Defines a selector used to read category labels from chart items.
    /// </summary>
    [Parameter] public Func<TItem, object> Value { get; set; }

    /// <summary>
    /// Defines the axis title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Defines a callback used to format category axis labels.
    /// </summary>
    [Parameter] public Func<SvgChartAxisTickContext, string> TickFormatter { get; set; }

    /// <summary>
    /// Defines grid line options for this category axis.
    /// </summary>
    [Parameter] public SvgChartGridLinesOptions GridLines { get; set; }

    /// <summary>
    /// Defines label options for this category axis.
    /// </summary>
    [Parameter] public SvgChartAxisLabelsOptions LabelsOptions { get; set; }

    #endregion
}