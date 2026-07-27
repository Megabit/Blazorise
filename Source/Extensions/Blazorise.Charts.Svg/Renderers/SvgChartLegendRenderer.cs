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
    #region Members

    private const double LegendItemSwatchWidth = 12;

    private const double LegendItemTextGap = 6;

    private const double LegendItemGap = 16;

    private const double LegendHorizontalPadding = 8;

    private const double LegendFontSize = 12;

    private const double LegendMinRowHeight = 16;

    private const double LegendTopReservedHeight = 28;

    private const double LegendBottomReservedHeight = 38;

    #endregion

    #region Methods

    public static double ResolveReservedHeight( SvgChartRenderModel model, SvgChartOptions options, SvgChartLegendPosition position )
    {
        var legendItems = ResolveItems( model, static _ => Task.CompletedTask, static ( _, _ ) => Task.CompletedTask, static ( _, _ ) => false );

        if ( legendItems.Count == 0 )
            return 0;

        var rows = ResolveRows( legendItems, options );
        var fontSize = options.Font?.Size ?? LegendFontSize;

        return ResolveBaseReservedHeight( position ) + Math.Max( 0, rows.Count - 1 ) * ResolveRowHeight( fontSize );
    }

    public static void Render(
        RenderTreeBuilder builder,
        ref int sequence,
        SvgChartRenderModel model,
        SvgChartOptions options,
        SvgChartLegendPosition position,
        double y,
        object eventReceiver,
        Func<string, Task> toggleSeries,
        Func<string, int, Task> toggleDataPoint,
        Func<string, int, bool> isDataPointHidden )
    {
        var legendItems = ResolveItems( model, toggleSeries, toggleDataPoint, isDataPointHidden );

        if ( legendItems.Count == 0 )
            return;

        var fontSize = options.Font?.Size ?? LegendFontSize;
        var rowHeight = ResolveRowHeight( fontSize );
        var rows = ResolveRows( legendItems, options );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-legend" );

        for ( var rowIndex = 0; rowIndex < rows.Count; rowIndex++ )
        {
            var row = rows[rowIndex];
            var x = Math.Max( LegendHorizontalPadding, ( options.Width - row.Width ) / 2 );
            var rowY = position == SvgChartLegendPosition.Bottom
                ? y - ( rows.Count - 1 - rowIndex ) * rowHeight
                : y + rowIndex * rowHeight;

            foreach ( var rowItem in row.Items )
            {
                var item = rowItem.Item;

                builder.OpenElement( sequence++, "g" );
                builder.AddAttribute( sequence++, "class", "svg-chart-legend-item" );
                builder.AddAttribute( sequence++, "role", "button" );
                builder.AddAttribute( sequence++, "tabindex", "0" );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( eventReceiver, item.Toggle ) );

                builder.OpenElement( sequence++, "rect" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( rowY - 9 ) );
                builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( LegendItemSwatchWidth ) );
                builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( LegendItemSwatchWidth ) );
                builder.AddAttribute( sequence++, "rx", "3" );
                builder.AddAttribute( sequence++, "fill", item.Color );
                builder.AddAttribute( sequence++, "opacity", item.Hidden ? "0.35" : "1" );
                builder.CloseElement();

                builder.OpenElement( sequence++, "text" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x + LegendItemSwatchWidth + LegendItemTextGap ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( rowY + 1 ) );
                SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, options, fallbackSize: LegendFontSize, opacity: item.Hidden ? 0.45 : 0.8 );
                builder.AddContent( sequence++, item.Label );
                builder.CloseElement();

                builder.CloseElement();

                x += rowItem.Width + LegendItemGap;
            }
        }

        builder.CloseElement();
    }

    private static List<SvgChartLegendRow> ResolveRows( IReadOnlyList<SvgChartLegendItem> legendItems, SvgChartOptions options )
    {
        var fontSize = options.Font?.Size ?? LegendFontSize;
        var availableWidth = ResolveAvailableWidth( options );
        var rows = new List<SvgChartLegendRow>();
        var row = new SvgChartLegendRow();

        foreach ( var item in legendItems )
        {
            var itemWidth = ResolveVisibleItemWidth( item, availableWidth, fontSize );
            var rowWidth = row.Items.Count == 0 ? itemWidth : row.Width + LegendItemGap + itemWidth;

            if ( row.Items.Count > 0 && rowWidth > availableWidth )
            {
                rows.Add( row );
                row = new();
            }

            row.Add( item, itemWidth );
        }

        if ( row.Items.Count > 0 )
            rows.Add( row );

        return rows;
    }

    private static double ResolveVisibleItemWidth( SvgChartLegendItem item, double availableWidth, double fontSize )
    {
        var textWidth = SvgChartRenderHelpers.EstimateTextWidth( item.Label, fontSize );

        return Math.Min( availableWidth, LegendItemSwatchWidth + LegendItemTextGap + textWidth );
    }

    private static double ResolveAvailableWidth( SvgChartOptions options )
    {
        return Math.Max( 1, options.Width - LegendHorizontalPadding * 2 );
    }

    private static double ResolveRowHeight( double fontSize )
    {
        return Math.Max( LegendMinRowHeight, fontSize + 4 );
    }

    private static double ResolveBaseReservedHeight( SvgChartLegendPosition position )
    {
        return position == SvgChartLegendPosition.Bottom
            ? LegendBottomReservedHeight
            : LegendTopReservedHeight;
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
        return pointIndex >= 0 && pointIndex < ( series.PointColors?.Count ?? 0 ) && !string.IsNullOrWhiteSpace( series.PointColors[pointIndex] )
            ? series.PointColors[pointIndex]
            : series.RenderColor;
    }

    #endregion

    #region Classes

    private sealed class SvgChartLegendRow
    {
        #region Properties

        public List<SvgChartLegendRowItem> Items { get; } = [];

        public double Width { get; private set; }

        #endregion

        #region Methods

        public void Add( SvgChartLegendItem item, double width )
        {
            if ( Items.Count > 0 )
                Width += LegendItemGap;

            Items.Add( new( item, width ) );
            Width += width;
        }

        #endregion
    }

    private sealed record SvgChartLegendRowItem( SvgChartLegendItem Item, double Width );

    #endregion
}