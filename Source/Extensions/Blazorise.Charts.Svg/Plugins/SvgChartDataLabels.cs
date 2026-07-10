#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines data labels for a native SVG chart.
/// </summary>
public class SvgChartDataLabels : SvgChartPluginBase
{
    #region Members

    private ComponentParameterInfo<bool> paramVisible;

    private ComponentParameterInfo<bool> paramInteractive;

    private ComponentParameterInfo<SvgChartDataLabelPosition> paramPosition;

    private ComponentParameterInfo<double> paramOffset;

    private ComponentParameterInfo<double> paramOpacity;

    private ComponentParameterInfo<SvgChartFontOptions> paramFont;

    private ComponentParameterInfo<SvgChartSpacing> paramPadding;

    private ComponentParameterInfo<Color> paramBackgroundColor;

    private ComponentParameterInfo<SvgChartBorderOptions> paramBorder;

    private ComponentParameterInfo<Func<SvgChartDataLabelContext, string>> paramFormatter;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Visible, out paramVisible );
        parameters.TryGetParameter( Interactive, out paramInteractive );
        parameters.TryGetParameter( Position, out paramPosition );
        parameters.TryGetParameter( Offset, out paramOffset );
        parameters.TryGetParameter( Opacity, out paramOpacity );
        parameters.TryGetParameter( Font, out paramFont );
        parameters.TryGetParameter( Padding, out paramPadding );
        parameters.TryGetParameter( BackgroundColor, out paramBackgroundColor );
        parameters.TryGetParameter( Border, out paramBorder );
        parameters.TryGetParameter( Formatter, out paramFormatter );

        return base.SetParametersAsync( parameters );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether data labels are visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines whether data labels react to pointer and keyboard interactions.
    /// </summary>
    [Parameter] public bool Interactive { get; set; } = true;

    /// <summary>
    /// Defines the data label position.
    /// </summary>
    [Parameter] public SvgChartDataLabelPosition Position { get; set; } = SvgChartDataLabelPosition.Auto;

    /// <summary>
    /// Defines the label offset from the point anchor.
    /// </summary>
    [Parameter] public double Offset { get; set; } = 6;

    /// <summary>
    /// Defines the label opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 1;

    /// <summary>
    /// Defines data label font options.
    /// </summary>
    [Parameter] public SvgChartFontOptions Font { get; set; }

    /// <summary>
    /// Defines the label padding.
    /// </summary>
    [Parameter] public SvgChartSpacing Padding { get; set; }

    /// <summary>
    /// Defines the label background color.
    /// </summary>
    [Parameter] public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the label border options.
    /// </summary>
    [Parameter] public SvgChartBorderOptions Border { get; set; }

    /// <summary>
    /// Defines a callback used to format label text.
    /// </summary>
    [Parameter] public Func<SvgChartDataLabelContext, string> Formatter { get; set; }

    /// <summary>
    /// Occurs when a data label is clicked.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when a data label is hovered.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Hovered { get; set; }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override void Render( SvgChartPluginRenderContext context, RenderTreeBuilder builder, ref int sequence )
    {
        if ( !Visible )
            return;

        var points = context.IsRadial
            ? ResolveRadialDataLabelPoints( context )
            : ResolveCartesianDataLabelPoints( context );

        if ( points.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-data-labels" );

        foreach ( var item in points )
            RenderDataLabel( builder, ref sequence, context, item.Point, item.Color, item.ChartType );

        builder.CloseElement();
    }

    internal static SvgChartDataLabels Create( SvgChartDataLabelsOptions options )
    {
        if ( options is null )
            return new() { Visible = false };

        return new()
        {
            Visible = options.Visible,
            Interactive = options.Interactive,
            Position = options.Position,
            Offset = options.Offset,
            Opacity = options.Opacity,
            Font = SvgChartAnnotationRenderHelpers.CreateFontOptions( options.Font ),
            Padding = CreateSpacing( options.Padding ),
            BackgroundColor = options.BackgroundColor,
            Border = SvgChartAnnotationRenderHelpers.CreateBorderOptions( options.Border ),
            Formatter = options.Formatter
        };
    }

    internal static SvgChartDataLabels Create( SvgChartDataLabelsOptions options, SvgChartDataLabels component )
    {
        if ( component is null )
            return Create( options );

        options ??= new();

        return new()
        {
            Visible = component.paramVisible.GetValueOrDefault( options.Visible ),
            Interactive = component.paramInteractive.GetValueOrDefault( options.Interactive ),
            Position = component.paramPosition.GetValueOrDefault( options.Position ),
            Offset = component.paramOffset.GetValueOrDefault( options.Offset ),
            Opacity = component.paramOpacity.GetValueOrDefault( options.Opacity ),
            Font = SvgChartAnnotationRenderHelpers.CreateFontOptions( component.paramFont.GetValueOrDefault( options.Font ) ),
            Padding = CreateSpacing( component.paramPadding.GetValueOrDefault( options.Padding ) ),
            BackgroundColor = component.paramBackgroundColor.GetValueOrDefault( options.BackgroundColor ),
            Border = SvgChartAnnotationRenderHelpers.CreateBorderOptions( component.paramBorder.GetValueOrDefault( options.Border ) ),
            Formatter = component.paramFormatter.GetValueOrDefault( options.Formatter ),
            Clicked = component.Clicked,
            Hovered = component.Hovered
        };
    }

    private List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> ResolveCartesianDataLabelPoints( SvgChartPluginRenderContext context )
    {
        var points = new List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)>();

        foreach ( var series in context.Series.Where( x => !x.Hidden && !IsRadialChart( x.Type ) ) )
        {
            if ( series.Type == SvgChartType.Column )
            {
                AddColumnDataLabelPoints( points, context, series );
            }
            else if ( series.Type == SvgChartType.Bar )
            {
                AddBarDataLabelPoints( points, context, series );
            }
            else if ( series.Type is SvgChartType.Line or SvgChartType.Area )
            {
                AddLineDataLabelPoints( points, context, series );
            }
            else if ( IsPointChart( series.Type ) )
            {
                AddPointChartDataLabelPoints( points, context, series );
            }
        }

        return points;
    }

    private List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> ResolveRadialDataLabelPoints( SvgChartPluginRenderContext context )
    {
        var points = new List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)>();
        var series = context.Series.FirstOrDefault( x => !x.Hidden && IsRadialChart( x.Type ) );

        if ( series is null )
            return points;

        if ( series.Type == SvgChartType.Radar )
        {
            AddRadarDataLabelPoints( points, context, series );
            return points;
        }

        AddArcDataLabelPoints( points, context, series );

        return points;
    }

    private static void AddColumnDataLabelPoints( List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> points, SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        var visibleColumnSeries = context.Series.Where( x => x.Type == SvgChartType.Column && !x.Hidden ).ToList();
        var seriesIndex = visibleColumnSeries.IndexOf( series );

        if ( seriesIndex < 0 )
            return;

        var categoryWidth = GetCategoryWidth( context );
        var groupWidth = categoryWidth * 0.72;
        var barWidth = Math.Max( 1, groupWidth / visibleColumnSeries.Count );
        var baselineValue = Math.Clamp( 0, context.GetValueMin( series.ValueAxisId ), context.GetValueMax( series.ValueAxisId ) );
        var baseline = context.ProjectY( baselineValue, series.ValueAxisId );

        for ( var pointIndex = 0; pointIndex < context.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            var categoryStart = context.ProjectCategoryBoundary( pointIndex ) + ( categoryWidth - groupWidth ) / 2;
            var x = categoryStart + barWidth * seriesIndex + barWidth * 0.1;
            var y = context.ProjectY( value.Value, series.ValueAxisId );
            var height = Math.Abs( baseline - y );
            var rectY = Math.Min( y, baseline );
            var rectWidth = Math.Max( 1, barWidth * 0.8 );
            var bounds = new SvgChartPointBounds { X = x, Y = rectY, Width = rectWidth, Height = height };

            points.Add( (context.CreatePoint( series, pointIndex, value.Value, bounds ), series.GetPointColor( pointIndex ), series.Type) );
        }
    }

    private static void AddBarDataLabelPoints( List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> points, SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        var visibleBarSeries = context.Series.Where( x => x.Type == SvgChartType.Bar && !x.Hidden ).ToList();
        var seriesIndex = visibleBarSeries.IndexOf( series );

        if ( seriesIndex < 0 || context.Labels.Count == 0 )
            return;

        var categoryHeight = context.PlotArea.Height / context.Labels.Count;
        var groupHeight = categoryHeight * 0.72;
        var barHeight = Math.Max( 1, groupHeight / visibleBarSeries.Count );
        var baselineValue = Math.Clamp( 0, context.GetValueMin( series.ValueAxisId ), context.GetValueMax( series.ValueAxisId ) );
        var baseline = context.ProjectX( baselineValue, series.ValueAxisId );

        for ( var pointIndex = 0; pointIndex < context.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            var categoryStart = context.PlotArea.Top + categoryHeight * pointIndex + ( categoryHeight - groupHeight ) / 2;
            var x = context.ProjectX( value.Value, series.ValueAxisId );
            var width = Math.Abs( x - baseline );
            var rectX = Math.Min( x, baseline );
            var y = categoryStart + barHeight * seriesIndex + barHeight * 0.1;
            var rectHeight = Math.Max( 1, barHeight * 0.8 );
            var bounds = new SvgChartPointBounds { X = rectX, Y = y, Width = width, Height = rectHeight };

            points.Add( (context.CreatePoint( series, pointIndex, value.Value, bounds ), series.GetPointColor( pointIndex ), series.Type) );
        }
    }

    private static void AddLineDataLabelPoints( List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> points, SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        if ( context.Labels.Count == 0 )
            return;

        var markerRadius = series.Type == SvgChartType.Area ? Math.Max( 3, series.StrokeWidth + 1 ) : series.MarkerRadius;

        for ( var pointIndex = 0; pointIndex < context.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            var x = ResolveLineX( context, series, pointIndex );
            var y = context.ProjectY( value.Value, series.ValueAxisId );
            var bounds = new SvgChartPointBounds { X = x - markerRadius, Y = y - markerRadius, Width = markerRadius * 2, Height = markerRadius * 2 };

            points.Add( (context.CreatePoint( series, pointIndex, value.Value, bounds ), series.GetPointColor( pointIndex ), series.Type) );
        }
    }

    private static void AddPointChartDataLabelPoints( List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> points, SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        for ( var pointIndex = 0; pointIndex < series.YValues.Count; pointIndex++ )
        {
            var yValue = series.YValues[pointIndex];
            var xValue = pointIndex < series.XValues.Count ? series.XValues[pointIndex] : pointIndex;

            if ( !xValue.HasValue || !yValue.HasValue )
                continue;

            var x = context.ProjectX( xValue.Value );
            var y = context.ProjectY( yValue.Value, series.ValueAxisId );
            var radius = series.Type == SvgChartType.Bubble
                ? Math.Max( 2, pointIndex < series.RadiusValues.Count && series.RadiusValues[pointIndex].HasValue ? series.RadiusValues[pointIndex].Value : series.MarkerRadius )
                : series.MarkerRadius;
            var bounds = new SvgChartPointBounds { X = x - radius, Y = y - radius, Width = radius * 2, Height = radius * 2 };
            var category = context.ContinuousCategoryAxis && pointIndex < context.Labels.Count
                ? context.Labels[pointIndex]
                : xValue.Value;
            var point = new SvgChartPointEventArgs
            {
                SeriesName = series.Name,
                SeriesIndex = context.Series.ToList().IndexOf( series ),
                PointIndex = pointIndex,
                Category = category,
                Value = yValue.Value,
                Bounds = bounds
            };

            points.Add( (point, series.GetPointColor( pointIndex ), series.Type) );
        }
    }

    private static double ResolveLineX( SvgChartPluginRenderContext context, SvgChartPluginSeries series, int pointIndex )
    {
        if ( context.ContinuousCategoryAxis && pointIndex < series.XValues.Count && series.XValues[pointIndex].HasValue )
            return context.ProjectX( series.XValues[pointIndex].Value );

        return context.ProjectCategory( pointIndex );
    }

    private static void AddArcDataLabelPoints( List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> points, SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        var values = series.Values
            .Select( ( value, index ) => new { Value = value, Index = index } )
            .Where( x => x.Value.HasValue && x.Value.Value >= 0 && !context.IsDataPointHidden( series.Name, x.Index ) )
            .Select( x => (Value: x.Value.Value, Index: x.Index) )
            .ToList();

        if ( values.Count == 0 )
            return;

        var centerX = context.PlotArea.Left + context.PlotArea.Width / 2;
        var centerY = context.PlotArea.Top + context.PlotArea.Height / 2;
        var radius = Math.Max( 1, Math.Min( context.PlotArea.Width, context.PlotArea.Height ) * 0.42 );
        var total = series.Type == SvgChartType.PolarArea ? values.Count : values.Sum( x => x.Value );
        var startAngle = -Math.PI / 2;
        var max = values.Max( x => x.Value );

        if ( total <= 0 )
            return;

        for ( var i = 0; i < values.Count; i++ )
        {
            var value = values[i].Value;
            var pointIndex = values[i].Index;
            var sweep = series.Type == SvgChartType.PolarArea ? Math.PI * 2 / values.Count : value / total * Math.PI * 2;
            var pointRadius = series.Type == SvgChartType.PolarArea ? radius * Math.Sqrt( value / Math.Max( max, 1 ) ) : radius;
            var innerRadius = series.Type == SvgChartType.Doughnut ? radius * 0.58 : 0;
            var labelRadius = ( innerRadius + pointRadius ) / 2;
            var labelCenter = PolarToCartesian( centerX, centerY, labelRadius, startAngle + sweep / 2 );
            var labelBounds = new SvgChartPointBounds { X = labelCenter.X, Y = labelCenter.Y, Width = 0, Height = 0 };
            var category = pointIndex < context.Labels.Count ? context.Labels[pointIndex] : pointIndex + 1;
            var point = new SvgChartPointEventArgs
            {
                SeriesName = series.Name,
                SeriesIndex = context.Series.ToList().IndexOf( series ),
                PointIndex = pointIndex,
                Category = category,
                Value = value,
                Bounds = labelBounds
            };

            points.Add( (point, series.GetPointColor( pointIndex ), series.Type) );

            startAngle += sweep;
        }
    }

    private static void AddRadarDataLabelPoints( List<(SvgChartPointEventArgs Point, string Color, SvgChartType ChartType)> points, SvgChartPluginRenderContext context, SvgChartPluginSeries series )
    {
        if ( context.Labels.Count == 0 )
            return;

        var centerX = context.PlotArea.Left + context.PlotArea.Width / 2;
        var centerY = context.PlotArea.Top + context.PlotArea.Height / 2;
        var radius = Math.Max( 1, Math.Min( context.PlotArea.Width, context.PlotArea.Height ) * 0.42 );
        var max = Math.Max( context.ValueMax, 1 );

        for ( var pointIndex = 0; pointIndex < context.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            var angle = -Math.PI / 2 + Math.PI * 2 * pointIndex / context.Labels.Count;
            var renderedRadius = radius * Math.Max( value.Value, 0 ) / max;
            var renderedPoint = PolarToCartesian( centerX, centerY, renderedRadius, angle );
            var markerRadius = 4d;
            var bounds = new SvgChartPointBounds
            {
                X = renderedPoint.X - markerRadius,
                Y = renderedPoint.Y - markerRadius,
                Width = markerRadius * 2,
                Height = markerRadius * 2
            };

            points.Add( (context.CreatePoint( series, pointIndex, value.Value, bounds ), series.GetPointColor( pointIndex ), series.Type) );
        }
    }

    private void RenderDataLabel( RenderTreeBuilder builder, ref int sequence, SvgChartPluginRenderContext context, SvgChartPointEventArgs point, string color, SvgChartType chartType )
    {
        var labelContext = new SvgChartDataLabelContext
        {
            SeriesName = point.SeriesName,
            SeriesIndex = point.SeriesIndex,
            PointIndex = point.PointIndex,
            Category = point.Category,
            Value = point.Value,
            Bounds = point.Bounds,
            Color = color,
            Point = point
        };
        var defaultOptions = context.Options.DataLabels ?? new SvgChartDataLabelsOptions();
        var text = ( Formatter ?? defaultOptions.Formatter )?.Invoke( labelContext ) ?? SvgChartRenderHelpers.FormatDataLabelValue( point.Value );

        if ( string.IsNullOrWhiteSpace( text ) )
            return;

        var font = CreateFontOptions( defaultOptions.Font, Font ) ?? new();
        var fontSize = font.Size ?? 11;
        var padding = Padding ?? defaultOptions.Padding ?? new();
        var textWidth = SvgChartRenderHelpers.EstimateTextWidth( text, fontSize );
        var width = textWidth + padding.Start + padding.End;
        var height = fontSize + padding.Top + padding.Bottom;
        var anchor = ResolveDataLabelAnchor( point.Bounds, ResolveDataLabelPosition( Position, chartType ), Offset, width, height );
        var x = anchor.X;
        var y = anchor.Y;
        var fill = SvgChartRenderHelpers.ResolveFontColor( font );
        var backgroundColor = SvgChartRenderHelpers.IsDefaultColor( BackgroundColor ) ? defaultOptions.BackgroundColor : BackgroundColor;
        var background = ResolveDataLabelBackgroundColor( backgroundColor );
        var border = Border ?? defaultOptions.Border;
        var hasBackground = !SvgChartRenderHelpers.IsDefaultColor( backgroundColor );
        var hasBorder = border?.Width > 0;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-data-label" );
        builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Math.Clamp( Opacity, 0, 1 ) ) );

        if ( Interactive )
        {
            builder.AddAttribute( sequence++, "tabindex", "0" );
            builder.AddAttribute( sequence++, "role", "button" );
            builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
            builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( context.EventReceiver, () => context.NotifyDataLabelClicked( point, color ) ) );
            builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( context.EventReceiver, () => context.NotifyDataLabelHovered( point, color ) ) );
            builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( context.EventReceiver, _ => context.NotifyPointLeft() ) );
            builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( context.EventReceiver, () => context.ShowTooltip( point, color, false ) ) );
            builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( context.EventReceiver, _ => context.NotifyPointLeft() ) );
        }
        else
        {
            builder.AddAttribute( sequence++, "pointer-events", "none" );
        }

        if ( hasBackground || hasBorder )
        {
            builder.OpenElement( sequence++, "rect" );
            builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
            builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y ) );
            builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( width ) );
            builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( height ) );
            builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( Math.Max( 0, border?.Radius ?? 0 ) ) );
            builder.AddAttribute( sequence++, "fill", background );

            if ( hasBorder )
            {
                builder.AddAttribute( sequence++, "stroke", SvgChartRenderHelpers.ResolveFontColor( border.Color ) );
                builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( border.Width ) );
            }

            builder.CloseElement();
        }

        builder.OpenElement( sequence++, "text" );
        builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x + padding.Start + textWidth / 2 ) );
        builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y + padding.Top + fontSize * 0.82 ) );
        builder.AddAttribute( sequence++, "text-anchor", "middle" );
        builder.AddAttribute( sequence++, "font-size", SvgChartRenderHelpers.Format( fontSize ) );
        SvgChartRenderHelpers.AddFontFamilyAttribute( builder, ref sequence, font.Family );
        builder.AddAttribute( sequence++, "fill", fill );

        if ( !string.IsNullOrWhiteSpace( font.Weight ) )
            builder.AddAttribute( sequence++, "font-weight", font.Weight );

        builder.AddContent( sequence++, text );
        builder.CloseElement();
        builder.CloseElement();
    }

    private static SvgChartFontOptions CreateFontOptions( SvgChartFontOptions options, SvgChartFontOptions overrides )
    {
        if ( overrides is null )
            return SvgChartAnnotationRenderHelpers.CreateFontOptions( options );

        return new()
        {
            Family = overrides.Family ?? options?.Family,
            Size = overrides.Size ?? options?.Size,
            Weight = overrides.Weight ?? options?.Weight,
            Color = SvgChartRenderHelpers.IsDefaultColor( overrides.Color ) ? options?.Color : overrides.Color
        };
    }

    private static SvgChartSpacing CreateSpacing( SvgChartSpacing spacing )
    {
        if ( spacing is null )
            return new();

        return new()
        {
            Top = spacing.Top,
            End = spacing.End,
            Bottom = spacing.Bottom,
            Start = spacing.Start
        };
    }

    private static string ResolveDataLabelBackgroundColor( Color backgroundColor )
    {
        return SvgChartRenderHelpers.IsDefaultColor( backgroundColor )
            ? "var(--bs-body-bg, #fff)"
            : SvgChartRenderHelpers.ResolveFontColor( backgroundColor );
    }

    private static SvgChartDataLabelPosition ResolveDataLabelPosition( SvgChartDataLabelPosition position, SvgChartType chartType )
    {
        if ( position != SvgChartDataLabelPosition.Auto )
            return position;

        return chartType switch
        {
            SvgChartType.Bar => SvgChartDataLabelPosition.End,
            SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea => SvgChartDataLabelPosition.Center,
            _ => SvgChartDataLabelPosition.Top
        };
    }

    private static (double X, double Y) ResolveDataLabelAnchor( SvgChartPointBounds bounds, SvgChartDataLabelPosition position, double offset, double width, double height )
    {
        var centerX = bounds.X + bounds.Width / 2;
        var centerY = bounds.Y + bounds.Height / 2;

        return position switch
        {
            SvgChartDataLabelPosition.Center => (centerX - width / 2, centerY - height / 2),
            SvgChartDataLabelPosition.End => (bounds.X + bounds.Width + offset, centerY - height / 2),
            SvgChartDataLabelPosition.Bottom => (centerX - width / 2, bounds.Y + bounds.Height + offset),
            SvgChartDataLabelPosition.Start => (bounds.X - width - offset, centerY - height / 2),
            _ => (centerX - width / 2, bounds.Y - height - offset)
        };
    }

    private static double GetCategoryWidth( SvgChartPluginRenderContext context )
    {
        var range = Math.Max( context.CategorySlotCount, 1 );

        return context.PlotArea.Width / range;
    }

    private static (double X, double Y) PolarToCartesian( double centerX, double centerY, double radius, double angle )
    {
        return (centerX + radius * Math.Cos( angle ), centerY + radius * Math.Sin( angle ));
    }

    private static bool IsPointChart( SvgChartType chartType )
    {
        return chartType == SvgChartType.Scatter || chartType == SvgChartType.Bubble;
    }

    private static bool IsRadialChart( SvgChartType chartType )
    {
        return chartType is SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea or SvgChartType.Radar;
    }

    private static string GetPointLabel( SvgChartPointEventArgs point )
    {
        return $"{point.Category}, {point.Value}. {point.SeriesName}.";
    }

    #endregion
}