#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartGeometry
{
    #region Methods

    public static (double Min, double Max) ResolveCategoryRange( int labelCount, SvgChartZoomOptions zoom, SvgChartViewport viewport )
    {
        var min = -0.5;
        var max = Math.Max( 0.5, labelCount - 0.5 );

        if ( zoom?.Enabled != true || !SupportsHorizontalZoom( zoom.Mode ) )
            return (min, max);

        return NormalizeViewportRange( viewport?.XMin, viewport?.XMax, min, max, zoom.MinZoom, zoom.MaxZoom );
    }

    public static SvgChartAxisOptions ApplyValueAxisViewport( SvgChartAxisOptions axis, SvgChartZoomOptions zoom, SvgChartViewport viewport, bool horizontalValueAxis )
    {
        if ( zoom?.Enabled != true || viewport is null )
            return axis;

        var supportsAxis = horizontalValueAxis
            ? SupportsHorizontalZoom( zoom.Mode )
            : SupportsVerticalZoom( zoom.Mode );

        if ( !supportsAxis )
            return axis;

        var viewportMin = horizontalValueAxis ? viewport.XMin : viewport.YMin;
        var viewportMax = horizontalValueAxis ? viewport.XMax : viewport.YMax;

        if ( !viewportMin.HasValue || !viewportMax.HasValue || viewportMax <= viewportMin )
            return axis;

        var result = SvgChartOptionsMapper.CreateValueAxisOptions( axis );
        result.Min = viewportMin;
        result.Max = viewportMax;

        return result;
    }

    public static SvgChartAxisOptions ApplyPointXAxisViewport( SvgChartAxisOptions axis, SvgChartZoomOptions zoom, SvgChartViewport viewport )
    {
        if ( zoom?.Enabled != true || !SupportsHorizontalZoom( zoom.Mode ) || viewport?.XMin is null || viewport.XMax is null || viewport.XMax <= viewport.XMin )
            return axis;

        var result = SvgChartOptionsMapper.CreateValueAxisOptions( axis );
        result.Min = viewport.XMin;
        result.Max = viewport.XMax;

        return result;
    }

    public static SvgChartScale BuildScale( List<double> values, SvgChartAxisOptions axis )
    {
        var min = axis.Min ?? ( values.Count == 0 ? 0 : values.Min() );
        var max = axis.Max ?? ( values.Count == 0 ? 1 : values.Max() );

        if ( axis.BeginAtZero )
        {
            min = Math.Min( min, 0 );
            max = Math.Max( max, 0 );
        }

        if ( Math.Abs( max - min ) < double.Epsilon )
        {
            max += 1;
            min -= 1;
        }

        var tickCount = Math.Max( 2, axis.TickCount );
        var step = NiceNumber( ( max - min ) / ( tickCount - 1 ), true );
        var niceMin = axis.Min ?? Math.Floor( min / step ) * step;
        var niceMax = axis.Max ?? Math.Ceiling( max / step ) * step;
        var ticks = new List<double>();

        for ( var tick = niceMin; tick <= niceMax + step / 2; tick += step )
            ticks.Add( Math.Abs( tick ) < step / 1000000 ? 0 : tick );

        return new()
        {
            Min = niceMin,
            Max = niceMax,
            Ticks = ticks
        };
    }

    public static (double Min, double Max) NormalizeViewportRange( double? requestedMin, double? requestedMax, double fullMin, double fullMax, double minZoom, double maxZoom )
    {
        if ( !requestedMin.HasValue || !requestedMax.HasValue || requestedMax <= requestedMin )
            return (fullMin, fullMax);

        return ClampRange( requestedMin.Value, requestedMax.Value, fullMin, fullMax, minZoom, maxZoom );
    }

    public static (double Min, double Max) ClampRange( double min, double max, double fullMin, double fullMax, double minZoom, double maxZoom )
    {
        var fullRange = fullMax - fullMin;

        if ( fullRange <= 0 )
            return (fullMin, fullMax);

        var requestedRange = Math.Max( double.Epsilon, max - min );
        var minimumZoom = Math.Max( 1, minZoom );
        var maximumZoom = Math.Max( minimumZoom, maxZoom );
        var maxRange = fullRange / minimumZoom;
        var minRange = fullRange / maximumZoom;
        var range = Math.Clamp( requestedRange, minRange, maxRange );
        var center = min + requestedRange / 2;
        var resultMin = center - range / 2;
        var resultMax = center + range / 2;

        if ( resultMin < fullMin )
        {
            resultMax += fullMin - resultMin;
            resultMin = fullMin;
        }

        if ( resultMax > fullMax )
        {
            resultMin -= resultMax - fullMax;
            resultMax = fullMax;
        }

        return (Math.Max( fullMin, resultMin ), Math.Min( fullMax, resultMax ));
    }

    public static bool SupportsHorizontalZoom( SvgChartZoomMode mode )
    {
        return mode is SvgChartZoomMode.X or SvgChartZoomMode.XY;
    }

    public static bool SupportsVerticalZoom( SvgChartZoomMode mode )
    {
        return mode is SvgChartZoomMode.Y or SvgChartZoomMode.XY;
    }

    public static SvgChartViewport CloneViewport( SvgChartViewport viewport )
    {
        if ( viewport is null )
            return null;

        return new()
        {
            XMin = viewport.XMin,
            XMax = viewport.XMax,
            YMin = viewport.YMin,
            YMax = viewport.YMax
        };
    }

    public static double NiceNumber( double range, bool round )
    {
        if ( range <= 0 || double.IsNaN( range ) || double.IsInfinity( range ) )
            return 1;

        var exponent = Math.Floor( Math.Log10( range ) );
        var fraction = range / Math.Pow( 10, exponent );
        double niceFraction;

        if ( round )
        {
            niceFraction = fraction switch
            {
                < 1.5 => 1,
                < 3 => 2,
                < 7 => 5,
                _ => 10
            };
        }
        else
        {
            niceFraction = fraction switch
            {
                <= 1 => 1,
                <= 2 => 2,
                <= 5 => 5,
                _ => 10
            };
        }

        return niceFraction * Math.Pow( 10, exponent );
    }

    public static double GetY( double value, SvgChartPlotArea plot, SvgChartRenderModel model )
    {
        return GetY( value, plot, model.PrimaryValueAxis );
    }

    public static double GetY( double value, SvgChartPlotArea plot, SvgChartRenderModel model, SvgChartRenderSeries series )
    {
        return GetY( value, plot, ResolveValueAxis( model, series ) );
    }

    public static double GetY( double value, SvgChartPlotArea plot, SvgChartRenderValueAxis axis )
    {
        var range = axis.Max - axis.Min;

        if ( range <= 0 )
            return plot.Bottom;

        return plot.Bottom - ( value - axis.Min ) / range * plot.Height;
    }

    public static double GetX( double value, SvgChartPlotArea plot, SvgChartRenderModel model )
    {
        return GetX( value, plot, model.PrimaryValueAxis.Min, model.PrimaryValueAxis.Max );
    }

    public static double GetX( double value, SvgChartPlotArea plot, SvgChartRenderModel model, SvgChartRenderSeries series )
    {
        var axis = ResolveValueAxis( model, series );

        return GetX( value, plot, axis.Min, axis.Max );
    }

    public static double GetX( double value, SvgChartPlotArea plot, double min, double max )
    {
        var range = max - min;

        if ( range <= 0 )
            return plot.Left;

        return plot.Left + ( value - min ) / range * plot.Width;
    }

    public static double GetCategoryX( int index, SvgChartPlotArea plot, SvgChartRenderModel model )
    {
        return GetX( index, plot, model.CategoryMin, model.CategoryMax );
    }

    public static double GetCategoryBoundaryX( double index, SvgChartPlotArea plot, SvgChartRenderModel model )
    {
        return GetX( index - 0.5, plot, model.CategoryMin, model.CategoryMax );
    }

    public static double GetCategoryWidth( SvgChartPlotArea plot, SvgChartRenderModel model )
    {
        var range = model.CategoryMax - model.CategoryMin;

        if ( range <= 0 )
            return plot.Width;

        return plot.Width / range;
    }

    public static SvgChartScale ResolvePointXScale( SvgChartRenderModel model, IEnumerable<SvgChartRenderSeries> pointSeries = null )
    {
        if ( model.CategoryScale is not null )
            return model.CategoryScale;

        var series = pointSeries?.ToList()
            ?? model.Series.Where( x => !x.Hidden && IsPointChart( x.Type ) ).ToList();

        if ( series.Count == 0 )
            return null;

        var axis = ApplyPointXAxisViewport( model.Options.XAxis ?? new(), model.Zoom, model.Viewport );

        return BuildScale( series.SelectMany( x => x.XValues ).Where( x => x.HasValue ).Select( x => x.Value ).ToList(), axis );
    }

    public static SvgChartRenderValueAxis ResolveValueAxis( SvgChartRenderModel model, SvgChartRenderSeries series )
    {
        if ( string.IsNullOrWhiteSpace( series.ValueAxisId ) )
            return model.PrimaryValueAxis;

        return model.ValueAxes.LastOrDefault( x => string.Equals( x.Id, series.ValueAxisId, StringComparison.Ordinal ) ) ?? model.PrimaryValueAxis;
    }

    public static SvgChartRenderValueAxis ResolveValueAxis( SvgChartRenderModel model, string valueAxisId )
    {
        if ( string.IsNullOrWhiteSpace( valueAxisId ) )
            return model.PrimaryValueAxis;

        return model.ValueAxes.LastOrDefault( x => string.Equals( x.Id, valueAxisId, StringComparison.Ordinal ) ) ?? model.PrimaryValueAxis;
    }

    public static double ResolveAnnotationX( SvgChartRenderModel model, SvgChartPlotArea plot, double? value, SvgChartScale pointChartScale, double fallback )
    {
        if ( !value.HasValue )
            return fallback;

        if ( pointChartScale is not null )
            return GetX( value.Value, plot, pointChartScale.Min, pointChartScale.Max );

        return GetX( value.Value, plot, model.CategoryMin, model.CategoryMax );
    }

    public static double ResolveAnnotationY( SvgChartRenderModel model, SvgChartPlotArea plot, double? value, string valueAxisId, double fallback )
    {
        if ( !value.HasValue )
            return fallback;

        return GetY( value.Value, plot, ResolveValueAxis( model, valueAxisId ) );
    }

    public static bool IsBarChart( SvgChartRenderModel model )
    {
        return model.Series.Any( x => x.Type == SvgChartType.Bar );
    }

    public static bool IsRadialChart( SvgChartRenderModel model )
    {
        return model.Series.Any( x => IsRadialChart( x.Type ) );
    }

    public static bool IsRadialChart( SvgChartType chartType )
    {
        return chartType is SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea or SvgChartType.Radar;
    }

    public static bool IsRadialCategoryLegendChart( SvgChartRenderModel model )
    {
        return model.Series.Any( x => IsRadialCategoryLegendChart( x.Type ) );
    }

    public static bool IsRadialCategoryLegendChart( SvgChartType chartType )
    {
        return chartType is SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea;
    }

    public static bool IsPointChart( SvgChartType chartType )
    {
        return chartType == SvgChartType.Scatter || chartType == SvgChartType.Bubble;
    }

    #endregion
}