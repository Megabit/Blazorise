#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartRadialSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type is SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var item = series.FirstOrDefault( x => !x.Hidden && CanRender( x ) );

        if ( item is null )
            return;

        var values = item.Values
            .Select( ( value, index ) => new { Value = value, Index = index } )
            .Where( x => x.Value.HasValue && x.Value.Value >= 0 && !chart.IsDataPointHidden( item.Name, x.Index ) )
            .Select( x => (Value: x.Value.Value, Index: x.Index) )
            .ToList();

        if ( values.Count == 0 )
            return;

        var plot = chart.PlotArea;
        var centerX = plot.Left + plot.Width / 2;
        var centerY = plot.Top + plot.Height / 2;
        var radius = Math.Max( 1, Math.Min( plot.Width, plot.Height ) * 0.42 );
        var total = item.Type == SvgChartType.PolarArea ? values.Count : values.Sum( x => x.Value );
        var startAngle = -Math.PI / 2;
        var max = values.Max( x => x.Value );

        if ( total <= 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-radial" );

        for ( var i = 0; i < values.Count; i++ )
        {
            var value = values[i].Value;
            var pointIndex = values[i].Index;
            var sweep = item.Type == SvgChartType.PolarArea ? ( Math.PI * 2 / values.Count ) : ( value / total * Math.PI * 2 );
            var endAngle = startAngle + sweep;
            var pointRadius = item.Type == SvgChartType.PolarArea ? radius * Math.Sqrt( value / Math.Max( max, 1 ) ) : radius;
            var innerRadius = item.Type == SvgChartType.Doughnut ? radius * 0.58 : 0;
            var color = item.GetPointColor( pointIndex );
            var category = pointIndex < chart.Labels.Count ? chart.Labels[pointIndex] : pointIndex + 1;
            var bounds = new SvgChartPointBounds { X = centerX - pointRadius, Y = centerY - pointRadius, Width = pointRadius * 2, Height = pointRadius * 2 };
            var point = chart.CreatePoint( item, pointIndex, category, value, bounds );

            builder.OpenElement( sequence++, "path" );
            builder.AddAttribute( sequence++, "class", $"svg-chart-point svg-chart-{item.Type.ToString().ToLowerInvariant()}-segment" );
            builder.AddAttribute( sequence++, "d", SvgChartSeriesRenderHelpers.BuildArcPath( centerX, centerY, innerRadius, pointRadius, startAngle, endAngle ) );
            builder.AddAttribute( sequence++, "fill", color );
            builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
            builder.AddAttribute( sequence++, "stroke-width", "1" );
            context.AddAnimatedStyleAttribute( builder, ref sequence );
            context.AddPointInteractionAttributes( builder, ref sequence, point, color );
            context.RenderInitialAttributeAnimation( builder, ref sequence, "opacity", "0", "1" );
            builder.CloseElement();

            startAngle = endAngle;
        }

        builder.CloseElement();
    }

    #endregion
}