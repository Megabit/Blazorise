#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

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
        var visibleSeries = chart.Series.Where( x => x.Type == SvgChartType.Bar && !x.Hidden ).ToList();
        var renderSeries = series.Where( x => x.Type == SvgChartType.Bar && !x.Hidden ).ToList();

        if ( visibleSeries.Count == 0 || renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        var categoryHeight = chart.PlotArea.Height / chart.Labels.Count;
        var groupHeight = categoryHeight * 0.72;
        var barHeight = Math.Max( 1, groupHeight / visibleSeries.Count );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-bars" );

        foreach ( var item in renderSeries )
        {
            var seriesIndex = visibleSeries.IndexOf( item );
            var baseline = chart.ProjectValueX( 0, item.ValueAxisId );

            for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < item.Values.Count; pointIndex++ )
            {
                var value = item.Values[pointIndex];

                if ( !value.HasValue )
                    continue;

                var categoryStart = chart.PlotArea.Top + categoryHeight * pointIndex + ( categoryHeight - groupHeight ) / 2;
                var x = chart.ProjectValueX( value.Value, item.ValueAxisId );
                var width = Math.Abs( x - baseline );
                var rectX = Math.Min( x, baseline );
                var y = categoryStart + barHeight * seriesIndex + barHeight * 0.1;
                var rectHeight = Math.Max( 1, barHeight * 0.8 );
                var bounds = new SvgChartPointBounds { X = rectX, Y = y, Width = width, Height = rectHeight };
                var point = chart.CreatePoint( item, pointIndex, value.Value, bounds );
                var animationKey = context.TrackPointBounds( item, pointIndex, bounds );

                builder.OpenElement( sequence++, "rect" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-bar" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( rectX ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y ) );
                builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( width ) );
                builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( rectHeight ) );
                builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( item.BorderRadius ) );
                builder.AddAttribute( sequence++, "fill", item.Color );
                context.AddAnimatedStyleAttribute( builder, ref sequence );
                context.AddPointInteractionAttributes( builder, ref sequence, point, item.Color );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "x", SvgChartRenderHelpers.Format( baseline ), SvgChartRenderHelpers.Format( rectX ), bounds => SvgChartRenderHelpers.Format( bounds.X ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "y", SvgChartRenderHelpers.Format( y ), SvgChartRenderHelpers.Format( y ), bounds => SvgChartRenderHelpers.Format( bounds.Y ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "width", "0", SvgChartRenderHelpers.Format( width ), bounds => SvgChartRenderHelpers.Format( bounds.Width ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "height", SvgChartRenderHelpers.Format( rectHeight ), SvgChartRenderHelpers.Format( rectHeight ), bounds => SvgChartRenderHelpers.Format( bounds.Height ) );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    #endregion
}