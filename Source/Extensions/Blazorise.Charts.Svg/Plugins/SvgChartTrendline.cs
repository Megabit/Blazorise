#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a trendline for a native SVG chart.
/// </summary>
public class SvgChartTrendline : SvgChartPluginBase
{
    #region Properties

    /// <summary>
    /// Defines whether the trendline is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the source series name used to calculate the trendline.
    /// </summary>
    [Parameter] public string SeriesName { get; set; }

    /// <summary>
    /// Defines the trendline name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Defines the trendline calculation type.
    /// </summary>
    [Parameter] public SvgChartTrendlineType Type { get; set; } = SvgChartTrendlineType.Linear;

    /// <summary>
    /// Defines the trendline color. Use a Blazorise theme color, or pass a CSS color value such as <c>#4c6ef5</c>, <c>rgb(76, 110, 245)</c>, <c>hsl(228 88% 60%)</c>, or <c>var(--chart-color)</c>.
    /// </summary>
    [Parameter] public Color Color { get; set; }

    /// <summary>
    /// Defines the trendline stroke width.
    /// </summary>
    [Parameter] public double StrokeWidth { get; set; } = 2;

    /// <summary>
    /// Defines the trendline stroke dash pattern.
    /// </summary>
    [Parameter] public string DashPattern { get; set; } = "6 4";

    /// <summary>
    /// Defines the trendline opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 0.85;

    /// <summary>
    /// Defines the trendline rendering order among other trendlines. Lower values are rendered first, behind higher values.
    /// </summary>
    [Parameter] public int? Order { get; set; }

    /// <inheritdoc/>
    public override void Render( SvgChartPluginRenderContext context, RenderTreeBuilder builder, ref int sequence )
    {
        if ( !Visible || context.IsRadial || Type != SvgChartTrendlineType.Linear || string.IsNullOrWhiteSpace( SeriesName ) )
            return;

        var series = context.Series.LastOrDefault( x => !x.Hidden && string.Equals( x.Name, SeriesName, StringComparison.Ordinal ) );

        if ( series is null )
            return;

        var line = IsPointChart( series.Type )
            ? CalculatePointTrendline( context, series )
            : CalculateCategoryTrendline( context, series );

        if ( line is null )
            return;

        var color = SvgChartRenderHelpers.IsDefaultColor( Color )
            ? series.Color
            : SvgChartRenderHelpers.ResolveColor( Color, 0 );

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "class", "svg-chart-trendline" );
        builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( line.Value.X1 ) );
        builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( line.Value.Y1 ) );
        builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( line.Value.X2 ) );
        builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( line.Value.Y2 ) );
        builder.AddAttribute( sequence++, "stroke", color );
        builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( StrokeWidth ) );
        builder.AddAttribute( sequence++, "stroke-linecap", "round" );
        builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Opacity ) );

        if ( !string.IsNullOrWhiteSpace( DashPattern ) )
            builder.AddAttribute( sequence++, "stroke-dasharray", DashPattern );

        builder.CloseElement();
    }

    internal static SvgChartTrendline Create( SvgChartTrendlineOptions options )
    {
        if ( options is null )
            return new();

        return new()
        {
            Visible = options.Visible,
            SeriesName = options.SeriesName,
            Name = options.Name,
            Type = options.Type,
            Color = options.Color,
            StrokeWidth = options.StrokeWidth,
            DashPattern = options.DashPattern,
            Opacity = options.Opacity,
            Order = options.Order
        };
    }

    private static (double X1, double Y1, double X2, double Y2)? CalculateCategoryTrendline( SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        if ( series.Type == SvgChartType.Bar )
            return null;

        var samples = new List<(double X, double Y)>();

        for ( var i = 0; i < context.Labels.Count && i < series.Values.Count; i++ )
        {
            var value = series.Values[i];

            if ( value.HasValue )
                samples.Add( (i, value.Value) );
        }

        if ( !TryCalculateLinearRegression( samples, out var slope, out var intercept ) )
            return null;

        var firstIndex = samples.Min( x => x.X );
        var lastIndex = samples.Max( x => x.X );
        var firstValue = slope * firstIndex + intercept;
        var lastValue = slope * lastIndex + intercept;

        return (
            context.ProjectCategory( firstIndex ),
            context.ProjectY( firstValue, series.ValueAxisId ),
            context.ProjectCategory( lastIndex ),
            context.ProjectY( lastValue, series.ValueAxisId )
        );
    }

    private static (double X1, double Y1, double X2, double Y2)? CalculatePointTrendline( SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        var samples = new List<(double X, double Y)>();
        var count = Math.Min( series.XValues.Count, series.YValues.Count );

        for ( var i = 0; i < count; i++ )
        {
            var xValue = series.XValues[i];
            var yValue = series.YValues[i];

            if ( xValue.HasValue && yValue.HasValue )
                samples.Add( (xValue.Value, yValue.Value) );
        }

        if ( !TryCalculateLinearRegression( samples, out var slope, out var intercept ) )
            return null;

        var firstX = samples.Min( x => x.X );
        var lastX = samples.Max( x => x.X );
        var firstY = slope * firstX + intercept;
        var lastY = slope * lastX + intercept;

        return (
            context.ProjectX( firstX ),
            context.ProjectY( firstY, series.ValueAxisId ),
            context.ProjectX( lastX ),
            context.ProjectY( lastY, series.ValueAxisId )
        );
    }

    private static bool TryCalculateLinearRegression( List<(double X, double Y)> samples, out double slope, out double intercept )
    {
        slope = 0;
        intercept = 0;

        if ( samples.Count < 2 )
            return false;

        var count = samples.Count;
        var sumX = samples.Sum( x => x.X );
        var sumY = samples.Sum( x => x.Y );
        var sumXY = samples.Sum( x => x.X * x.Y );
        var sumXX = samples.Sum( x => x.X * x.X );
        var denominator = count * sumXX - sumX * sumX;

        if ( Math.Abs( denominator ) < double.Epsilon )
            return false;

        slope = ( count * sumXY - sumX * sumY ) / denominator;
        intercept = ( sumY - slope * sumX ) / count;

        return !double.IsNaN( slope )
               && !double.IsInfinity( slope )
               && !double.IsNaN( intercept )
               && !double.IsInfinity( intercept );
    }

    private static bool IsPointChart( SvgChartType chartType )
    {
        return chartType == SvgChartType.Scatter || chartType == SvgChartType.Bubble;
    }

    #endregion
}