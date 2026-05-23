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
        var barWidth = Math.Max( 1, groupWidth / visibleSeries.Count );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-columns" );

        foreach ( var item in renderSeries )
        {
            var seriesIndex = visibleSeries.IndexOf( item );
            var baseline = chart.ProjectY( 0, item.ValueAxisId );

            for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < item.Values.Count; pointIndex++ )
            {
                var value = item.Values[pointIndex];

                if ( !value.HasValue )
                    continue;

                var categoryStart = chart.ProjectCategoryBoundary( pointIndex ) + ( categoryWidth - groupWidth ) / 2;
                var x = categoryStart + barWidth * seriesIndex + barWidth * 0.1;
                var y = chart.ProjectY( value.Value, item.ValueAxisId );
                var height = Math.Abs( baseline - y );
                var rectY = Math.Min( y, baseline );
                var rectWidth = Math.Max( 1, barWidth * 0.8 );
                var bounds = new SvgChartPointBounds { X = x, Y = rectY, Width = rectWidth, Height = height };
                var point = chart.CreatePoint( item, pointIndex, value.Value, bounds );
                var animationKey = context.TrackPointBounds( item, pointIndex, bounds );

                builder.OpenElement( sequence++, "rect" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-column" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( rectY ) );
                builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( rectWidth ) );
                builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( height ) );
                builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( item.BorderRadius ) );
                builder.AddAttribute( sequence++, "fill", item.Color );
                context.AddAnimatedStyleAttribute( builder, ref sequence );
                context.AddPointInteractionAttributes( builder, ref sequence, point, item.Color );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "x", SvgChartRenderHelpers.Format( x ), SvgChartRenderHelpers.Format( x ), bounds => SvgChartRenderHelpers.Format( bounds.X ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "y", SvgChartRenderHelpers.Format( baseline ), SvgChartRenderHelpers.Format( rectY ), bounds => SvgChartRenderHelpers.Format( bounds.Y ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "width", SvgChartRenderHelpers.Format( rectWidth ), SvgChartRenderHelpers.Format( rectWidth ), bounds => SvgChartRenderHelpers.Format( bounds.Width ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "height", "0", SvgChartRenderHelpers.Format( height ), bounds => SvgChartRenderHelpers.Format( bounds.Height ) );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    #endregion
}