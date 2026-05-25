#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartZoomController
{
    #region Methods

    public static SvgChartViewport ResolveFullViewport( SvgChartRenderModel model )
    {
        return new()
        {
            XMin = ResolveFullViewportXMin( model ),
            XMax = ResolveFullViewportXMax( model ),
            YMin = model.PrimaryValueAxis.Min,
            YMax = model.PrimaryValueAxis.Max
        };
    }

    public static SvgChartViewport ResolveEffectiveViewport( SvgChartRenderModel model, SvgChartViewport fullViewport )
    {
        var viewport = model.Viewport;

        return new()
        {
            XMin = viewport?.XMin ?? fullViewport.XMin,
            XMax = viewport?.XMax ?? fullViewport.XMax,
            YMin = viewport?.YMin ?? fullViewport.YMin,
            YMax = viewport?.YMax ?? fullViewport.YMax
        };
    }

    public static SvgChartViewport ZoomViewport( SvgChartViewport viewport, SvgChartViewport fullViewport, SvgChartZoomOptions zoom, SvgChartPlotArea plot, double anchorX, double anchorY, double factor )
    {
        var result = SvgChartGeometry.CloneViewport( viewport );

        if ( SvgChartGeometry.SupportsHorizontalZoom( zoom.Mode ) )
        {
            var range = viewport.XMax.Value - viewport.XMin.Value;
            var anchor = viewport.XMin.Value + range * Math.Clamp( ( anchorX - plot.Left ) / plot.Width, 0, 1 );
            var min = anchor - ( anchor - viewport.XMin.Value ) * factor;
            var max = anchor + ( viewport.XMax.Value - anchor ) * factor;
            var clamped = SvgChartGeometry.ClampRange( min, max, fullViewport.XMin.Value, fullViewport.XMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.XMin = clamped.Min;
            result.XMax = clamped.Max;
        }

        if ( SvgChartGeometry.SupportsVerticalZoom( zoom.Mode ) )
        {
            var range = viewport.YMax.Value - viewport.YMin.Value;
            var anchor = viewport.YMax.Value - range * Math.Clamp( ( anchorY - plot.Top ) / plot.Height, 0, 1 );
            var min = anchor - ( anchor - viewport.YMin.Value ) * factor;
            var max = anchor + ( viewport.YMax.Value - anchor ) * factor;
            var clamped = SvgChartGeometry.ClampRange( min, max, fullViewport.YMin.Value, fullViewport.YMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.YMin = clamped.Min;
            result.YMax = clamped.Max;
        }

        return result;
    }

    public static SvgChartViewport PanViewport( SvgChartViewport viewport, SvgChartViewport fullViewport, SvgChartZoomOptions zoom, SvgChartPlotArea plot, double deltaX, double deltaY )
    {
        var result = SvgChartGeometry.CloneViewport( viewport );

        if ( SvgChartGeometry.SupportsHorizontalZoom( zoom.Mode ) )
        {
            var range = viewport.XMax.Value - viewport.XMin.Value;
            var delta = -deltaX / plot.Width * range;
            var clamped = SvgChartGeometry.ClampRange( viewport.XMin.Value + delta, viewport.XMax.Value + delta, fullViewport.XMin.Value, fullViewport.XMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.XMin = clamped.Min;
            result.XMax = clamped.Max;
        }

        if ( SvgChartGeometry.SupportsVerticalZoom( zoom.Mode ) )
        {
            var range = viewport.YMax.Value - viewport.YMin.Value;
            var delta = deltaY / plot.Height * range;
            var clamped = SvgChartGeometry.ClampRange( viewport.YMin.Value + delta, viewport.YMax.Value + delta, fullViewport.YMin.Value, fullViewport.YMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.YMin = clamped.Min;
            result.YMax = clamped.Max;
        }

        return result;
    }

    private static double ResolveFullViewportXMin( SvgChartRenderModel model )
    {
        var pointScale = SvgChartGeometry.ResolvePointXScale( model );

        if ( pointScale is not null )
            return pointScale.Min;

        if ( SvgChartGeometry.IsBarChart( model ) )
            return model.PrimaryValueAxis.Min;

        return -0.5;
    }

    private static double ResolveFullViewportXMax( SvgChartRenderModel model )
    {
        var pointScale = SvgChartGeometry.ResolvePointXScale( model );

        if ( pointScale is not null )
            return pointScale.Max;

        if ( SvgChartGeometry.IsBarChart( model ) )
            return model.PrimaryValueAxis.Max;

        return Math.Max( 0.5, model.Labels.Count - 0.5 );
    }

    #endregion
}