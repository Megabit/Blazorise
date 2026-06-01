#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartLegendRenderer
{
    #region Methods

    public static void Render(
        RenderTreeBuilder builder,
        ref int sequence,
        SvgChartRenderModel model,
        SvgChartOptions options,
        double y,
        object eventReceiver,
        Func<string, Task> toggleSeries,
        Func<string, int, Task> toggleDataPoint,
        Func<string, int, bool> isDataPointHidden )
    {
        var legendItems = ResolveItems( model, toggleSeries, toggleDataPoint, isDataPointHidden );

        if ( legendItems.Count == 0 )
            return;

        var itemWidth = Math.Min( 140, options.Width / Math.Max( legendItems.Count, 1 ) );
        var totalWidth = itemWidth * legendItems.Count;
        var startX = Math.Max( 8, ( options.Width - totalWidth ) / 2 );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-legend" );

        for ( var i = 0; i < legendItems.Count; i++ )
        {
            var item = legendItems[i];
            var x = startX + itemWidth * i;

            builder.OpenElement( sequence++, "g" );
            builder.AddAttribute( sequence++, "class", "svg-chart-legend-item" );
            builder.AddAttribute( sequence++, "role", "button" );
            builder.AddAttribute( sequence++, "tabindex", "0" );
            builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( eventReceiver, item.Toggle ) );

            builder.OpenElement( sequence++, "rect" );
            builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
            builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y - 9 ) );
            builder.AddAttribute( sequence++, "width", "12" );
            builder.AddAttribute( sequence++, "height", "12" );
            builder.AddAttribute( sequence++, "rx", "3" );
            builder.AddAttribute( sequence++, "fill", item.Color );
            builder.AddAttribute( sequence++, "opacity", item.Hidden ? "0.35" : "1" );
            builder.CloseElement();

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x + 18 ) );
            builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y + 1 ) );
            SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, options, fallbackSize: 12, opacity: item.Hidden ? 0.45 : 0.8 );
            builder.AddContent( sequence++, item.Label );
            builder.CloseElement();

            builder.CloseElement();
        }

        builder.CloseElement();
    }

    private static List<SvgChartLegendItem> ResolveItems( SvgChartRenderModel model, Func<string, Task> toggleSeries, Func<string, int, Task> toggleDataPoint, Func<string, int, bool> isDataPointHidden )
    {
        if ( model.Series.Count == 0 )
            return [];

        if ( SvgChartGeometry.IsRadialCategoryLegendChart( model ) )
            return ResolveRadialItems( model, toggleDataPoint, isDataPointHidden );

        return model.Series.Select( series => new SvgChartLegendItem
        {
            Label = series.Name,
            Color = series.RenderColor,
            Hidden = series.Hidden,
            Toggle = () => toggleSeries( series.Name )
        } ).ToList();
    }

    private static List<SvgChartLegendItem> ResolveRadialItems( SvgChartRenderModel model, Func<string, int, Task> toggleDataPoint, Func<string, int, bool> isDataPointHidden )
    {
        var series = model.Series.FirstOrDefault();

        if ( series is null )
            return [];

        var count = Math.Max( model.Labels.Count, series.Values.Count );
        var result = new List<SvgChartLegendItem>();

        for ( var i = 0; i < count; i++ )
        {
            var pointIndex = i;
            var category = i < model.Labels.Count ? model.Labels[i] : i + 1;

            result.Add( new()
            {
                Label = category?.ToString() ?? string.Empty,
                Color = ResolvePointColor( series, i ),
                Hidden = series.Hidden || isDataPointHidden( series.Name, pointIndex ),
                Toggle = () => toggleDataPoint( series.Name, pointIndex )
            } );
        }

        return result;
    }

    private static string ResolvePointColor( SvgChartRenderSeries series, int pointIndex )
    {
        return pointIndex >= 0 && pointIndex < series.PointColors.Count && !string.IsNullOrWhiteSpace( series.PointColors[pointIndex] )
            ? series.PointColors[pointIndex]
            : series.RenderColor;
    }

    #endregion
}