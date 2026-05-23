#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartLineSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Line;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => x.Type == SvgChartType.Line && !x.Hidden ).ToList();

        if ( renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-lines" );

        foreach ( var item in renderSeries )
        {
            var points = new List<(int Index, double X, double Y, double Value)>();

            for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < item.Values.Count; pointIndex++ )
            {
                var value = item.Values[pointIndex];

                if ( value.HasValue )
                    points.Add( (pointIndex, chart.ProjectCategory( pointIndex ), chart.ProjectY( value.Value, item.ValueAxisId ), value.Value) );
            }

            if ( points.Count > 1 )
            {
                var linePath = SvgChartSeriesRenderHelpers.BuildLinePath( points );

                builder.OpenElement( sequence++, "path" );
                builder.AddAttribute( sequence++, "class", "svg-chart-line" );
                builder.AddAttribute( sequence++, "d", linePath );
                builder.AddAttribute( sequence++, "fill", "none" );
                builder.AddAttribute( sequence++, "stroke", item.Color );
                builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( item.StrokeWidth ) );
                builder.AddAttribute( sequence++, "stroke-linecap", "round" );
                builder.AddAttribute( sequence++, "stroke-linejoin", "round" );
                context.AddAnimatedStyleAttribute( builder, ref sequence );
                context.RenderPathFadeAnimation( builder, ref sequence, item, "line", linePath, "1" );
                builder.CloseElement();
            }

            foreach ( var renderedPoint in points )
            {
                var bounds = new SvgChartPointBounds
                {
                    X = renderedPoint.X - item.MarkerRadius,
                    Y = renderedPoint.Y - item.MarkerRadius,
                    Width = item.MarkerRadius * 2,
                    Height = item.MarkerRadius * 2
                };
                var point = chart.CreatePoint( item, renderedPoint.Index, renderedPoint.Value, bounds );
                var animationKey = context.TrackPointBounds( item, renderedPoint.Index, bounds );

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-marker" );
                builder.AddAttribute( sequence++, "cx", SvgChartRenderHelpers.Format( renderedPoint.X ) );
                builder.AddAttribute( sequence++, "cy", SvgChartRenderHelpers.Format( renderedPoint.Y ) );
                builder.AddAttribute( sequence++, "r", SvgChartRenderHelpers.Format( item.MarkerRadius ) );
                builder.AddAttribute( sequence++, "fill", item.Color );
                builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
                builder.AddAttribute( sequence++, "stroke-width", "1.5" );
                context.AddAnimatedStyleAttribute( builder, ref sequence );
                context.AddPointInteractionAttributes( builder, ref sequence, point, item.Color );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cx", SvgChartRenderHelpers.Format( renderedPoint.X ), SvgChartRenderHelpers.Format( renderedPoint.X ), bounds => SvgChartRenderHelpers.Format( bounds.X + bounds.Width / 2 ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cy", SvgChartRenderHelpers.Format( renderedPoint.Y ), SvgChartRenderHelpers.Format( renderedPoint.Y ), bounds => SvgChartRenderHelpers.Format( bounds.Y + bounds.Height / 2 ) );
                context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "r", "0", SvgChartRenderHelpers.Format( item.MarkerRadius ), bounds => SvgChartRenderHelpers.Format( bounds.Width / 2 ) );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    #endregion
}