#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartPointSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type is SvgChartType.Scatter or SvgChartType.Bubble;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => !x.Hidden && CanRender( x ) ).ToList();

        if ( renderSeries.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-points" );

        foreach ( var item in renderSeries )
        {
            for ( var pointIndex = 0; pointIndex < item.YValues.Count; pointIndex++ )
            {
                var yValue = item.YValues[pointIndex];
                var xValue = pointIndex < item.XValues.Count ? item.XValues[pointIndex] : pointIndex;

                if ( !xValue.HasValue || !yValue.HasValue )
                    continue;

                var x = chart.ProjectX( xValue.Value );
                var y = chart.ProjectY( yValue.Value, item.ValueAxisId );
                var radius = item.Type == SvgChartType.Bubble
                    ? Math.Max( 2, pointIndex < item.RadiusValues.Count && item.RadiusValues[pointIndex].HasValue ? item.RadiusValues[pointIndex].Value : item.MarkerRadius )
                    : item.MarkerRadius;
                var bounds = new SvgChartPointBounds { X = x - radius, Y = y - radius, Width = radius * 2, Height = radius * 2 };
                var category = chart.ContinuousCategoryAxis && pointIndex < chart.Labels.Count
                    ? chart.Labels[pointIndex]
                    : xValue.Value;
                var point = chart.CreatePoint( item, pointIndex, category, yValue.Value, bounds );
                var animationKey = context.TrackPointBounds( item, pointIndex, bounds );
                var color = item.GetPointColor( pointIndex );

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", $"svg-chart-point svg-chart-{item.Type.ToString().ToLowerInvariant()}" );
                builder.AddAttribute( sequence++, "cx", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "cy", SvgChartRenderHelpers.Format( y ) );
                builder.AddAttribute( sequence++, "r", SvgChartRenderHelpers.Format( radius ) );
                builder.AddAttribute( sequence++, "fill", color );
                builder.AddAttribute( sequence++, "opacity", item.Type == SvgChartType.Bubble ? "0.72" : "1" );
                context.AddAnimatedStyleAttribute( builder, ref sequence );
                context.AddPointInteractionAttributes( builder, ref sequence, point, color );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cx", SvgChartRenderHelpers.Format( x ), SvgChartRenderHelpers.Format( x ), bounds => SvgChartRenderHelpers.Format( bounds.X + bounds.Width / 2 ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cy", SvgChartRenderHelpers.Format( y ), SvgChartRenderHelpers.Format( y ), bounds => SvgChartRenderHelpers.Format( bounds.Y + bounds.Height / 2 ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "r", "0", SvgChartRenderHelpers.Format( radius ), bounds => SvgChartRenderHelpers.Format( bounds.Width / 2 ) );
                context.RenderInitialAttributeAnimation( builder, ref sequence, "opacity", "0", item.Type == SvgChartType.Bubble ? "0.72" : "1" );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    #endregion
}