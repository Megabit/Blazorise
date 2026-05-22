#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartSeriesRenderHelpers
{
    #region Methods

    public static string BuildLinePath( IReadOnlyList<(int Index, double X, double Y, double Value)> points )
    {
        var builder = new StringBuilder();

        for ( var i = 0; i < points.Count; i++ )
        {
            builder.Append( i == 0 ? "M " : " L " );
            builder.Append( SvgChartRenderHelpers.Format( points[i].X ) );
            builder.Append( ' ' );
            builder.Append( SvgChartRenderHelpers.Format( points[i].Y ) );
        }

        return builder.ToString();
    }

    public static string BuildArcPath( double centerX, double centerY, double innerRadius, double outerRadius, double startAngle, double endAngle )
    {
        var sweep = endAngle - startAngle;

        if ( sweep >= Math.PI * 2 )
            endAngle = startAngle + Math.PI * 2 - 0.0001;

        var largeArc = sweep > Math.PI ? 1 : 0;
        var outerStart = PolarToCartesian( centerX, centerY, outerRadius, startAngle );
        var outerEnd = PolarToCartesian( centerX, centerY, outerRadius, endAngle );

        if ( innerRadius <= 0 )
        {
            return $"M {SvgChartRenderHelpers.Format( centerX )} {SvgChartRenderHelpers.Format( centerY )} L {SvgChartRenderHelpers.Format( outerStart.X )} {SvgChartRenderHelpers.Format( outerStart.Y )} A {SvgChartRenderHelpers.Format( outerRadius )} {SvgChartRenderHelpers.Format( outerRadius )} 0 {largeArc} 1 {SvgChartRenderHelpers.Format( outerEnd.X )} {SvgChartRenderHelpers.Format( outerEnd.Y )} Z";
        }

        var innerEnd = PolarToCartesian( centerX, centerY, innerRadius, endAngle );
        var innerStart = PolarToCartesian( centerX, centerY, innerRadius, startAngle );

        return $"M {SvgChartRenderHelpers.Format( outerStart.X )} {SvgChartRenderHelpers.Format( outerStart.Y )} A {SvgChartRenderHelpers.Format( outerRadius )} {SvgChartRenderHelpers.Format( outerRadius )} 0 {largeArc} 1 {SvgChartRenderHelpers.Format( outerEnd.X )} {SvgChartRenderHelpers.Format( outerEnd.Y )} L {SvgChartRenderHelpers.Format( innerEnd.X )} {SvgChartRenderHelpers.Format( innerEnd.Y )} A {SvgChartRenderHelpers.Format( innerRadius )} {SvgChartRenderHelpers.Format( innerRadius )} 0 {largeArc} 0 {SvgChartRenderHelpers.Format( innerStart.X )} {SvgChartRenderHelpers.Format( innerStart.Y )} Z";
    }

    public static string BuildRadarPoints( IReadOnlyList<double?> values, double centerX, double centerY, double radius, double max )
    {
        var builder = new StringBuilder();
        var count = values.Count;

        if ( count == 0 || max <= 0 )
            return string.Empty;

        for ( var i = 0; i < count; i++ )
        {
            var value = Math.Max( values[i] ?? 0, 0 );
            var angle = -Math.PI / 2 + Math.PI * 2 * i / count;
            var point = PolarToCartesian( centerX, centerY, radius * value / max, angle );

            if ( i > 0 )
                builder.Append( ' ' );

            builder.Append( SvgChartRenderHelpers.Format( point.X ) );
            builder.Append( ',' );
            builder.Append( SvgChartRenderHelpers.Format( point.Y ) );
        }

        return builder.ToString();
    }

    public static (double X, double Y) PolarToCartesian( double centerX, double centerY, double radius, double angle )
    {
        return (centerX + radius * Math.Cos( angle ), centerY + radius * Math.Sin( angle ));
    }

    public static int ResolveRenderOrder( SvgChartPluginSeries series )
    {
        if ( series.Order.HasValue )
            return series.Order.Value;

        return series.Type switch
        {
            SvgChartType.Area => 0,
            SvgChartType.Column or SvgChartType.Bar => 10,
            SvgChartType.Line => 20,
            SvgChartType.Scatter or SvgChartType.Bubble => 30,
            _ => 0
        };
    }

    #endregion
}