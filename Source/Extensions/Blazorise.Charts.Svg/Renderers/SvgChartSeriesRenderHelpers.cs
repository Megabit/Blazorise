#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartSeriesRenderHelpers
{
    #region Methods

    public static string BuildLinePath( IReadOnlyList<(int Index, double X, double Y, double Value)> points, SvgChartInterpolationMode interpolation = SvgChartInterpolationMode.Linear, double tension = 0.4 )
    {
        var builder = new StringBuilder();

        AppendInterpolatedPath( builder, points, interpolation, tension, true );

        return builder.ToString();
    }

    public static string BuildAreaPath( IReadOnlyList<(int Index, double X, double Y, double Value)> points, IReadOnlyList<(int Index, double X, double Y, double Value)> basePoints, SvgChartInterpolationMode interpolation = SvgChartInterpolationMode.Linear, double tension = 0.4 )
    {
        var builder = new StringBuilder();

        AppendInterpolatedPath( builder, points, interpolation, tension, true );

        var reversedBasePoints = new List<(int Index, double X, double Y, double Value)>();

        for ( var i = basePoints.Count - 1; i >= 0; i-- )
            reversedBasePoints.Add( basePoints[i] );

        AppendInterpolatedPath( builder, reversedBasePoints, interpolation, tension, false );

        builder.Append( " Z" );

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

    private static void AppendInterpolatedPath( StringBuilder builder, IReadOnlyList<(int Index, double X, double Y, double Value)> points, SvgChartInterpolationMode interpolation, double tension, bool startWithMove )
    {
        if ( points is null || points.Count == 0 )
            return;

        AppendPointCommand( builder, startWithMove ? "M " : " L ", points[0] );

        if ( points.Count == 1 )
            return;

        switch ( interpolation )
        {
            case SvgChartInterpolationMode.Monotone:
                AppendMonotonePath( builder, points );
                break;
            case SvgChartInterpolationMode.Cubic:
                AppendCubicPath( builder, points, tension );
                break;
            case SvgChartInterpolationMode.StepBefore:
            case SvgChartInterpolationMode.StepAfter:
            case SvgChartInterpolationMode.StepMiddle:
                AppendStepPath( builder, points, interpolation );
                break;
            default:
                AppendLinearPath( builder, points );
                break;
        }
    }

    private static void AppendLinearPath( StringBuilder builder, IReadOnlyList<(int Index, double X, double Y, double Value)> points )
    {
        for ( var i = 1; i < points.Count; i++ )
            AppendPointCommand( builder, " L ", points[i] );
    }

    private static void AppendStepPath( StringBuilder builder, IReadOnlyList<(int Index, double X, double Y, double Value)> points, SvgChartInterpolationMode interpolation )
    {
        for ( var i = 1; i < points.Count; i++ )
        {
            var previous = points[i - 1];
            var current = points[i];

            switch ( interpolation )
            {
                case SvgChartInterpolationMode.StepBefore:
                    AppendLineCommand( builder, previous.X, current.Y );
                    AppendLineCommand( builder, current.X, current.Y );
                    break;
                case SvgChartInterpolationMode.StepAfter:
                    AppendLineCommand( builder, current.X, previous.Y );
                    AppendLineCommand( builder, current.X, current.Y );
                    break;
                default:
                    var middleX = previous.X + ( current.X - previous.X ) / 2d;
                    AppendLineCommand( builder, middleX, previous.Y );
                    AppendLineCommand( builder, middleX, current.Y );
                    AppendLineCommand( builder, current.X, current.Y );
                    break;
            }
        }
    }

    private static void AppendCubicPath( StringBuilder builder, IReadOnlyList<(int Index, double X, double Y, double Value)> points, double tension )
    {
        var resolvedTension = Math.Clamp( tension, 0d, 1d );

        if ( resolvedTension <= 0 )
        {
            AppendLinearPath( builder, points );
            return;
        }

        for ( var i = 1; i < points.Count; i++ )
        {
            var previousPrevious = points[Math.Max( 0, i - 2 )];
            var previous = points[i - 1];
            var current = points[i];
            var next = points[Math.Min( points.Count - 1, i + 1 )];
            var control1X = previous.X + ( current.X - previousPrevious.X ) * resolvedTension / 6d;
            var control1Y = previous.Y + ( current.Y - previousPrevious.Y ) * resolvedTension / 6d;
            var control2X = current.X - ( next.X - previous.X ) * resolvedTension / 6d;
            var control2Y = current.Y - ( next.Y - previous.Y ) * resolvedTension / 6d;

            AppendCubicCommand( builder, control1X, control1Y, control2X, control2Y, current.X, current.Y );
        }
    }

    private static void AppendMonotonePath( StringBuilder builder, IReadOnlyList<(int Index, double X, double Y, double Value)> points )
    {
        if ( !CanUseMonotoneInterpolation( points ) )
        {
            AppendLinearPath( builder, points );
            return;
        }

        var count = points.Count;
        var slopes = new double[count - 1];
        var tangents = new double[count];

        for ( var i = 0; i < count - 1; i++ )
            slopes[i] = ( points[i + 1].Y - points[i].Y ) / ( points[i + 1].X - points[i].X );

        tangents[0] = slopes[0];
        tangents[count - 1] = slopes[count - 2];

        for ( var i = 1; i < count - 1; i++ )
        {
            tangents[i] = slopes[i - 1] * slopes[i] <= 0
                ? 0
                : ( slopes[i - 1] + slopes[i] ) / 2d;
        }

        for ( var i = 0; i < count - 1; i++ )
        {
            if ( slopes[i] == 0 )
            {
                tangents[i] = 0;
                tangents[i + 1] = 0;
                continue;
            }

            var alpha = tangents[i] / slopes[i];
            var beta = tangents[i + 1] / slopes[i];
            var distance = Math.Sqrt( alpha * alpha + beta * beta );

            if ( distance > 3 )
            {
                var scale = 3 / distance;
                tangents[i] = scale * alpha * slopes[i];
                tangents[i + 1] = scale * beta * slopes[i];
            }
        }

        for ( var i = 0; i < count - 1; i++ )
        {
            var current = points[i];
            var next = points[i + 1];
            var deltaX = next.X - current.X;
            var control1X = current.X + deltaX / 3d;
            var control1Y = current.Y + tangents[i] * deltaX / 3d;
            var control2X = next.X - deltaX / 3d;
            var control2Y = next.Y - tangents[i + 1] * deltaX / 3d;

            AppendCubicCommand( builder, control1X, control1Y, control2X, control2Y, next.X, next.Y );
        }
    }

    private static bool CanUseMonotoneInterpolation( IReadOnlyList<(int Index, double X, double Y, double Value)> points )
    {
        var increasing = points[points.Count - 1].X > points[0].X;

        for ( var i = 1; i < points.Count; i++ )
        {
            if ( increasing && points[i].X <= points[i - 1].X )
                return false;

            if ( !increasing && points[i].X >= points[i - 1].X )
                return false;
        }

        return true;
    }

    private static void AppendPointCommand( StringBuilder builder, string command, (int Index, double X, double Y, double Value) point )
    {
        builder.Append( command );
        builder.Append( SvgChartRenderHelpers.Format( point.X ) );
        builder.Append( ' ' );
        builder.Append( SvgChartRenderHelpers.Format( point.Y ) );
    }

    private static void AppendLineCommand( StringBuilder builder, double x, double y )
    {
        builder.Append( " L " );
        builder.Append( SvgChartRenderHelpers.Format( x ) );
        builder.Append( ' ' );
        builder.Append( SvgChartRenderHelpers.Format( y ) );
    }

    private static void AppendCubicCommand( StringBuilder builder, double control1X, double control1Y, double control2X, double control2Y, double x, double y )
    {
        builder.Append( " C " );
        builder.Append( SvgChartRenderHelpers.Format( control1X ) );
        builder.Append( ' ' );
        builder.Append( SvgChartRenderHelpers.Format( control1Y ) );
        builder.Append( ' ' );
        builder.Append( SvgChartRenderHelpers.Format( control2X ) );
        builder.Append( ' ' );
        builder.Append( SvgChartRenderHelpers.Format( control2Y ) );
        builder.Append( ' ' );
        builder.Append( SvgChartRenderHelpers.Format( x ) );
        builder.Append( ' ' );
        builder.Append( SvgChartRenderHelpers.Format( y ) );
    }

    #endregion
}