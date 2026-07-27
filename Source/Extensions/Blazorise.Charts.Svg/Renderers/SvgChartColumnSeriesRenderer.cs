#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartColumnSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Column;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var visibleSeries = chart.Series.Where( x => x.Type == SvgChartType.Column && !x.Hidden ).ToList();
        var renderSeries = series.Where( x => x.Type == SvgChartType.Column && !x.Hidden ).ToList();

        if ( visibleSeries.Count == 0 || renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        var categoryWidth = Math.Abs( chart.ProjectCategoryBoundary( 1 ) - chart.ProjectCategoryBoundary( 0 ) );
        var groupWidth = categoryWidth * 0.72;
        var stackGroups = visibleSeries.Select( ResolveStackGroup ).Distinct().ToList();
        var barWidth = Math.Max( 1, groupWidth / stackGroups.Count );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-columns" );

        foreach ( var item in renderSeries )
        {
            var seriesIndex = stackGroups.IndexOf( ResolveStackGroup( item ) );
            var baselineValue = Math.Clamp( 0, chart.GetValueMin( item.ValueAxisId ), chart.GetValueMax( item.ValueAxisId ) );
            var baseline = chart.ProjectY( baselineValue, item.ValueAxisId );

            for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < item.Values.Count; pointIndex++ )
            {
                var value = item.Values[pointIndex];

                if ( !value.HasValue )
                    continue;

                var categoryStart = chart.ProjectCategoryBoundary( pointIndex ) + ( categoryWidth - groupWidth ) / 2;
                var x = categoryStart + barWidth * seriesIndex + barWidth * 0.1;
                var startValue = ResolveStackValue( item.StackBaseValues, pointIndex, baselineValue );
                var endValue = ResolveStackValue( item.StackEndValues, pointIndex, value.Value );
                var startY = chart.ProjectY( startValue, item.ValueAxisId );
                var endY = chart.ProjectY( endValue, item.ValueAxisId );
                var height = Math.Abs( startY - endY );
                var rectY = Math.Min( startY, endY );
                var rectWidth = Math.Max( 1, barWidth * 0.8 );
                var bounds = new SvgChartPointBounds { X = x, Y = rectY, Width = rectWidth, Height = height };
                var point = chart.CreatePoint( item, pointIndex, value.Value, bounds );
                var animationKey = context.TrackPointBounds( item, pointIndex, bounds );
                var color = item.GetPointColor( pointIndex );

                builder.OpenElement( sequence++, "rect" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-column" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( rectY ) );
                builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( rectWidth ) );
                builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( height ) );
                builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( item.BorderRadius ) );
                builder.AddAttribute( sequence++, "fill", color );
                context.AddAnimatedStyleAttribute( builder, ref sequence );
                context.AddPointInteractionAttributes( builder, ref sequence, point, color );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "x", SvgChartRenderHelpers.Format( x ), SvgChartRenderHelpers.Format( x ), bounds => SvgChartRenderHelpers.Format( bounds.X ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "y", SvgChartRenderHelpers.Format( baseline ), SvgChartRenderHelpers.Format( rectY ), bounds => SvgChartRenderHelpers.Format( bounds.Y ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "width", SvgChartRenderHelpers.Format( rectWidth ), SvgChartRenderHelpers.Format( rectWidth ), bounds => SvgChartRenderHelpers.Format( bounds.Width ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "height", "0", SvgChartRenderHelpers.Format( height ), bounds => SvgChartRenderHelpers.Format( bounds.Height ) );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
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