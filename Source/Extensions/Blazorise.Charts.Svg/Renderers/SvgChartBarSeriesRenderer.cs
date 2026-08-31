#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartBarSeriesContent : SvgChartSeriesContentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedRectangle> bars = [];

    private SvgChartRectangleSeriesState state;

    #endregion

    #region Methods

    protected override bool UpdateRenderState()
    {
        var resolvedState = new SvgChartRectangleSeriesState( Series.BorderRadius, Context.CategoryFormatterKey );
        var resolvedBars = ResolveBars( Context, Series );
        var shouldRender = Context.Animation.Enabled
            || resolvedState != state
            || !resolvedBars.SequenceEqual( bars );
        bars = resolvedBars;
        state = resolvedState;

        return shouldRender;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

        foreach ( var bar in bars )
        {
            var point = bar.CreatePoint( seriesIndex, Series.Name );
            var bounds = point.Bounds;
            var animationKey = Context.TrackPointBounds( Series, bar.PointIndex, bounds );
            var xString = SvgChartRenderHelpers.Format( bar.X );
            var yString = SvgChartRenderHelpers.Format( bar.Y );
            var widthString = SvgChartRenderHelpers.Format( bar.Width );
            var heightString = SvgChartRenderHelpers.Format( bar.Height );

            builder.OpenElement( sequence++, "rect" );
            builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-bar" );
            builder.AddAttribute( sequence++, "x", xString );
            builder.AddAttribute( sequence++, "y", yString );
            builder.AddAttribute( sequence++, "width", widthString );
            builder.AddAttribute( sequence++, "height", heightString );
            builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( Series.BorderRadius ) );
            builder.AddAttribute( sequence++, "fill", bar.Color );
            Context.AddPointInteractionAttributes( builder, ref sequence, point, bar.Color );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "x", SvgChartRenderHelpers.Format( bar.Baseline ), xString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.X ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "y", yString, yString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Y ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "width", "0", widthString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Width ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "height", heightString, heightString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Height ) );
            builder.CloseElement();
        }
    }

    private static List<SvgChartRenderedRectangle> ResolveBars( SvgChartSeriesRendererContext context, SvgChartPluginSeries series )
    {
        var chart = context.Chart;
        var visibleSeries = chart.Series.Where( x => x.Type == SvgChartType.Bar && !x.Hidden ).ToList();
        var stackGroups = visibleSeries.Select( ResolveStackGroup ).Distinct().ToList();
        var result = new List<SvgChartRenderedRectangle>( Math.Min( chart.Labels.Count, series.Values.Count ) );

        if ( stackGroups.Count == 0 )
            return result;

        var categoryHeight = chart.PlotArea.Height / chart.Labels.Count;
        var groupHeight = categoryHeight * 0.72;
        var barHeight = Math.Max( 1, groupHeight / stackGroups.Count );
        var seriesIndex = stackGroups.IndexOf( ResolveStackGroup( series ) );
        var baselineValue = Math.Clamp( 0, chart.GetValueMin( series.ValueAxisId ), chart.GetValueMax( series.ValueAxisId ) );
        var baseline = chart.ProjectValueX( baselineValue, series.ValueAxisId );

        for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            var categoryStart = chart.PlotArea.Top + categoryHeight * pointIndex + ( categoryHeight - groupHeight ) / 2;
            var startValue = ResolveStackValue( series.StackBaseValues, pointIndex, baselineValue );
            var endValue = ResolveStackValue( series.StackEndValues, pointIndex, value.Value );
            var startX = chart.ProjectValueX( startValue, series.ValueAxisId );
            var endX = chart.ProjectValueX( endValue, series.ValueAxisId );
            var width = Math.Abs( endX - startX );
            var x = Math.Min( endX, startX );
            var y = categoryStart + barHeight * seriesIndex + barHeight * 0.1;
            var height = Math.Max( 1, barHeight * 0.8 );

            result.Add( new(
                pointIndex,
                chart.Labels[pointIndex],
                value.Value,
                x,
                y,
                width,
                height,
                baseline,
                series.GetPointColor( pointIndex ) ) );
        }

        return result;
    }

    private static string ResolveStackGroup( SvgChartPluginSeries series )
    {
        return series.StackEndValues.Count > 0 ? series.Stack ?? string.Empty : series.Name;
    }

    private static double ResolveStackValue( IReadOnlyList<double?> values, int index, double fallback )
    {
        return index >= 0 && index < values.Count && values[index].HasValue ? values[index].Value : fallback;
    }

    #endregion
}

internal sealed class SvgChartBarSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Bar;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => x.Type == SvgChartType.Bar && context.ShouldRenderSeries( x ) ).ToList();

        if ( renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-bars" );

        foreach ( var item in renderSeries )
            context.RenderRetainedSeries<SvgChartBarSeriesContent>( builder, ref sequence, item, "svg-chart-bar-series", item.Hidden );

        builder.CloseElement();
    }

    #endregion
}