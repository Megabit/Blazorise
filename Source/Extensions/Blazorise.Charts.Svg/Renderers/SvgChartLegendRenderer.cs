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

    public static double ResolveReservedHeight( SvgChartRenderModel model, SvgChartOptions options, SvgChartLegendOptions legend )
    {
        var legendItems = ResolveItems( model, static _ => Task.CompletedTask, static ( _, _ ) => Task.CompletedTask, static ( _, _ ) => false );

        if ( legendItems.Count == 0 )
        {
            return 0;
        }

        var rows = ResolveRows( legendItems, options, legend );
        var fontSize = SvgChartTextRenderer.ResolveFontSize( options, legend.Labels?.Font, 12 );

        var padding = legend.Padding ?? new();

        return Math.Max( 0, padding.Top ) + Math.Max( 0, padding.Bottom ) + rows.Count * ResolveRowHeight( fontSize, legend ) + Math.Max( 0, rows.Count - 1 ) * Math.Max( 0, legend.Labels?.Padding ?? 10 );
    }

    public static void Render(
        RenderTreeBuilder builder,
        ref int sequence,
        SvgChartRenderModel model,
        SvgChartOptions options,
        SvgChartLegendOptions legend,
        SvgChartTextOptions title,
        SvgChartTextOptions subtitle,
        object eventReceiver,
        Func<string, Task> toggleSeries,
        Func<string, int, Task> toggleDataPoint,
        Func<string, int, bool> isDataPointHidden )
    {
        var legendItems = ResolveItems( model, toggleSeries, toggleDataPoint, isDataPointHidden );

        if ( legendItems.Count == 0 )
        {
            return;
        }

        var fontSize = SvgChartTextRenderer.ResolveFontSize( options, legend.Labels?.Font, 12 );
        var rowHeight = ResolveRowHeight( fontSize, legend );
        var rows = ResolveRows( legendItems, options, legend );
        var padding = legend.Padding ?? new();
        var y = legend.Position == SvgChartLegendPosition.Top
            ? SvgChartTextRenderer.GetTopTextHeight( title ) + SvgChartTextRenderer.GetTopTextHeight( subtitle )
            : options.Height - SvgChartTextRenderer.GetBottomTextHeight( title ) - SvgChartTextRenderer.GetBottomTextHeight( subtitle ) - ResolveReservedHeight( model, options, legend );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-legend" );

        for ( var rowIndex = 0; rowIndex < rows.Count; rowIndex++ )
        {
            var row = rows[rowIndex];
            var x = Math.Max( 0, padding.Start ) + Math.Max( 0, ( ResolveAvailableWidth( options, legend ) - row.Width ) / 2 );
            var rowY = y + Math.Max( 0, padding.Top ) + rowHeight / 2 + rowIndex * ( rowHeight + Math.Max( 0, legend.Labels?.Padding ?? 10 ) );

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
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( rowY - Math.Max( 0, legend.Labels?.BoxHeight ?? fontSize ) / 2 ) );
                builder.AddAttribute( sequence++, "width", SvgChartRenderHelpers.Format( Math.Max( 0, legend.Labels?.BoxWidth ?? 12 ) ) );
                builder.AddAttribute( sequence++, "height", SvgChartRenderHelpers.Format( Math.Max( 0, legend.Labels?.BoxHeight ?? fontSize ) ) );
                builder.AddAttribute( sequence++, "rx", "3" );
                builder.AddAttribute( sequence++, "fill", item.Color );
                builder.AddAttribute( sequence++, "opacity", item.Hidden ? "0.35" : "1" );
                builder.CloseElement();

                builder.OpenElement( sequence++, "text" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x + Math.Max( 0, legend.Labels?.BoxWidth ?? 12 ) + Math.Max( 0, legend.Labels?.LabelGap ?? fontSize / 2 ) ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( rowY ) );
                SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, options, legend.Labels?.Font, fallbackSize: 12, opacity: item.Hidden ? 0.45 : 0.8 );
                builder.AddAttribute( sequence++, "dominant-baseline", "central" );
                builder.AddContent( sequence++, SvgChartAxesRenderer.TruncateLabel( item.Label, Math.Max( 0, ResolveAvailableWidth( options, legend ) - Math.Max( 0, legend.Labels?.BoxWidth ?? 12 ) - Math.Max( 0, legend.Labels?.LabelGap ?? fontSize / 2 ) ), fontSize, options, legend.Labels?.Font ) );
                builder.CloseElement();

                builder.CloseElement();

                x += rowItem.Width + Math.Max( 0, legend.Labels?.Padding ?? 10 );
            }
        }

        builder.CloseElement();
    }

    private static List<SvgChartLegendRow> ResolveRows( IReadOnlyList<SvgChartLegendItem> legendItems, SvgChartOptions options, SvgChartLegendOptions legend )
    {
        var fontSize = SvgChartTextRenderer.ResolveFontSize( options, legend.Labels?.Font, 12 );
        var availableWidth = ResolveAvailableWidth( options, legend );
        var rows = new List<SvgChartLegendRow>();
        var row = new SvgChartLegendRow();

        foreach ( var item in legendItems )
        {
            var itemWidth = ResolveVisibleItemWidth( item, availableWidth, fontSize, options, legend );
            var rowWidth = row.Items.Count == 0 ? itemWidth : row.Width + Math.Max( 0, legend.Labels?.Padding ?? 10 ) + itemWidth;

            if ( row.Items.Count > 0 && rowWidth > availableWidth )
            {
                rows.Add( row );
                row = new();
            }

            row.Add( item, itemWidth, Math.Max( 0, legend.Labels?.Padding ?? 10 ) );
        }

        if ( row.Items.Count > 0 )
        {
            rows.Add( row );
        }

        return rows;
    }

    private static double ResolveVisibleItemWidth( SvgChartLegendItem item, double availableWidth, double fontSize, SvgChartOptions options, SvgChartLegendOptions legend )
    {
        var textWidth = SvgChartRenderHelpers.EstimateTextWidth( item.Label, fontSize, options, legend.Labels?.Font );

        return Math.Min( availableWidth, Math.Max( 0, legend.Labels?.BoxWidth ?? 12 ) + Math.Max( 0, legend.Labels?.LabelGap ?? fontSize / 2 ) + textWidth );
    }

    private static double ResolveAvailableWidth( SvgChartOptions options, SvgChartLegendOptions legend )
    {
        return Math.Max( 1, options.Width - Math.Max( 0, legend.Padding?.Start ?? 0 ) - Math.Max( 0, legend.Padding?.End ?? 0 ) );
    }

    private static double ResolveRowHeight( double fontSize, SvgChartLegendOptions legend )
    {
        return Math.Max( Math.Max( 0, legend.Labels?.BoxHeight ?? fontSize ), fontSize );
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

        public void Add( SvgChartLegendItem item, double width, double gap )
        {
            if ( Items.Count > 0 )
            {
                Width += gap;
            }

            Items.Add( new( item, width ) );
            Width += width;
        }

        #endregion
    }

    private sealed record SvgChartLegendRowItem( SvgChartLegendItem Item, double Width );

    #endregion
}