#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartRadarSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Radar;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => x.Type == SvgChartType.Radar && !x.Hidden ).ToList();

        if ( renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        var plot = chart.PlotArea;
        var centerX = plot.Left + plot.Width / 2;
        var centerY = plot.Top + plot.Height / 2;
        var radius = Math.Max( 1, Math.Min( plot.Width, plot.Height ) * 0.42 );
        var max = Math.Max( chart.ValueMax, 1 );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-radar" );

        for ( var i = 1; i <= 4; i++ )
        {
            builder.OpenElement( sequence++, "polygon" );
            builder.AddAttribute( sequence++, "points", SvgChartSeriesRenderHelpers.BuildRadarPoints( Enumerable.Repeat( max * i / 4, chart.Labels.Count ).Select( x => (double?)x ).ToList(), centerX, centerY, radius, max ) );
            builder.AddAttribute( sequence++, "fill", "none" );
            builder.AddAttribute( sequence++, "stroke", "currentColor" );
            builder.AddAttribute( sequence++, "stroke-opacity", "0.12" );
            builder.CloseElement();
        }

        foreach ( var item in renderSeries )
        {
            var radarPoints = SvgChartSeriesRenderHelpers.BuildRadarPoints( item.Values, centerX, centerY, radius, max );

            builder.OpenElement( sequence++, "polygon" );
            builder.AddAttribute( sequence++, "class", "svg-chart-radar-area" );
            builder.AddAttribute( sequence++, "points", radarPoints );
            builder.AddAttribute( sequence++, "fill", item.Color );
            builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( item.FillOpacity ) );
            builder.AddAttribute( sequence++, "stroke", item.Color );
            builder.AddAttribute( sequence++, "stroke-width", "2" );
            context.AddAnimatedStyleAttribute( builder, ref sequence );
            context.RenderPathFadeAnimation( builder, ref sequence, item, "area", radarPoints, SvgChartRenderHelpers.Format( item.FillOpacity ) );
            builder.CloseElement();

            for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < item.Values.Count; pointIndex++ )
            {
                var value = item.Values[pointIndex];

                if ( !value.HasValue )
                    continue;

                var angle = -Math.PI / 2 + Math.PI * 2 * pointIndex / chart.Labels.Count;
                var renderedRadius = radius * Math.Max( value.Value, 0 ) / max;
                var renderedPoint = SvgChartSeriesRenderHelpers.PolarToCartesian( centerX, centerY, renderedRadius, angle );
                var markerRadius = 4d;
                var bounds = new SvgChartPointBounds
                {
                    X = renderedPoint.X - markerRadius,
                    Y = renderedPoint.Y - markerRadius,
                    Width = markerRadius * 2,
                    Height = markerRadius * 2
                };
                var point = chart.CreatePoint( item, pointIndex, value.Value, bounds );
                var animationKey = context.TrackPointBounds( item, pointIndex, bounds );

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-radar-marker" );
                builder.AddAttribute( sequence++, "cx", SvgChartRenderHelpers.Format( renderedPoint.X ) );
                builder.AddAttribute( sequence++, "cy", SvgChartRenderHelpers.Format( renderedPoint.Y ) );
                builder.AddAttribute( sequence++, "r", SvgChartRenderHelpers.Format( markerRadius ) );
                builder.AddAttribute( sequence++, "fill", item.Color );
                builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
                builder.AddAttribute( sequence++, "stroke-width", "1.5" );
                context.AddAnimatedStyleAttribute( builder, ref sequence );
                context.AddPointInteractionAttributes( builder, ref sequence, point, item.Color );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cx", SvgChartRenderHelpers.Format( renderedPoint.X ), SvgChartRenderHelpers.Format( renderedPoint.X ), bounds => SvgChartRenderHelpers.Format( bounds.X + bounds.Width / 2 ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cy", SvgChartRenderHelpers.Format( renderedPoint.Y ), SvgChartRenderHelpers.Format( renderedPoint.Y ), bounds => SvgChartRenderHelpers.Format( bounds.Y + bounds.Height / 2 ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "r", "0", SvgChartRenderHelpers.Format( markerRadius ), bounds => SvgChartRenderHelpers.Format( bounds.Width / 2 ) );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    #endregion
}