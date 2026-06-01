#region Using directives
using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal static class SvgChartAxesRenderer
{
    #region Methods

    public static void RenderGridAndAxes( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model, SvgChartPlotArea plot, SvgChartStreamingAnimation streamingAnimation, string plotClipPathId, string categoryAxisLabelsClipPathId )
    {
        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid" );

        var primaryAxis = model.PrimaryValueAxis;
        var primaryGridLines = primaryAxis.GridLines;

        if ( primaryGridLines?.Visible == true )
        {
            foreach ( var tick in primaryAxis.Ticks )
            {
                var y = SvgChartGeometry.GetY( tick, plot, primaryAxis );

                builder.OpenElement( sequence++, "line" );
                builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( plot.Left ) );
                builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( plot.Right ) );
                builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( y ) );
                builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( y ) );
                AddGridLineAttributes( builder, ref sequence, primaryGridLines );
                builder.CloseElement();
            }
        }

        if ( model.CategoryScaleKind == SvgChartAxisScaleKind.Continuous || model.Series.Any( x => !x.Hidden && SvgChartGeometry.IsPointChart( x.Type ) ) )
            RenderPointXAxisGridAndLabels( builder, ref sequence, model, plot, plotClipPathId, categoryAxisLabelsClipPathId );
        else
            RenderCategoryAxisGridLines( builder, ref sequence, model, plot, streamingAnimation, plotClipPathId );

        var primaryLabels = primaryAxis.Labels ?? new();

        if ( primaryLabels.Visible )
        {
            for ( var i = 0; i < primaryAxis.Ticks.Count; i++ )
            {
                var tick = primaryAxis.Ticks[i];
                var y = SvgChartGeometry.GetY( tick, plot, primaryAxis );

                builder.OpenElement( sequence++, "text" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( plot.Left - 10 ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y + 4 ) );
                builder.AddAttribute( sequence++, "text-anchor", "end" );
                SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.68 );
                builder.AddContent( sequence++, FormatAxisLabel( FormatValueTick( primaryAxis, tick, i ), primaryLabels, model.Options ) );
                builder.CloseElement();
            }
        }

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( plot.Left ) );
        builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( plot.Left ) );
        builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( plot.Top ) );
        builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "stroke", "currentColor" );
        builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
        builder.CloseElement();

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( plot.Left ) );
        builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( plot.Right ) );
        builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "stroke", "currentColor" );
        builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
        builder.CloseElement();

        if ( !streamingAnimation.Enabled && model.CategoryScaleKind != SvgChartAxisScaleKind.Continuous && !model.Series.Any( x => !x.Hidden && SvgChartGeometry.IsPointChart( x.Type ) ) )
            RenderCategoryAxisLabels( builder, ref sequence, model, plot, streamingAnimation, categoryAxisLabelsClipPathId );

        RenderRightValueAxes( builder, ref sequence, model, plot, primaryAxis );

        builder.CloseElement();
    }

    public static void RenderCategoryAxisLabels( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model, SvgChartPlotArea plot, SvgChartStreamingAnimation streamingAnimation, string categoryAxisLabelsClipPathId )
    {
        var labels = model.CategoryAxis.Labels ?? new();
        var labelStep = Math.Max( 1, labels.Step );

        if ( !labels.Visible )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-axis-labels svg-chart-category-axis-labels" );

        if ( streamingAnimation.Enabled || model.Zoom?.Enabled == true )
        {
            builder.AddAttribute( sequence++, "clip-path", $"url(#{categoryAxisLabelsClipPathId})" );
        }

        if ( streamingAnimation.Enabled )
        {
            builder.OpenElement( sequence++, "g" );
            builder.SetKey( $"streaming-axis-labels-{streamingAnimation.Version}" );
            builder.AddAttribute( sequence++, "style", streamingAnimation.Style );
            AddStreamingAnimationAttributes( builder, ref sequence, streamingAnimation );
        }

        var labelCount = Math.Max( model.Labels.Count, model.CategorySlotCount );

        for ( var i = 0; i < labelCount; i++ )
        {
            var labelIndex = i < model.CategoryLabelIndexes.Count ? model.CategoryLabelIndexes[i] : i;

            if ( labelIndex < 0 || labelIndex % labelStep != 0 )
                continue;

            var x = SvgChartGeometry.GetCategoryX( i, plot, model );
            var label = i < model.Labels.Count ? model.Labels[i] : i + 1;

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
            builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( plot.Bottom + labels.Offset ) );
            builder.AddAttribute( sequence++, "text-anchor", "middle" );
            SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.72 );
            builder.AddContent( sequence++, FormatAxisLabel( FormatCategoryTick( model, label, labelIndex ), labels, model.Options ) );
            builder.CloseElement();
        }

        if ( streamingAnimation.Enabled )
            builder.CloseElement();

        builder.CloseElement();
    }

    public static void RenderHorizontalGridAndAxes( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model, SvgChartPlotArea plot )
    {
        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid" );

        var valueLabels = model.PrimaryValueAxis.Labels ?? new();

        for ( var i = 0; i < model.Ticks.Count; i++ )
        {
            var tick = model.Ticks[i];
            var x = SvgChartGeometry.GetX( tick, plot, model );
            var gridLines = model.PrimaryValueAxis.GridLines;

            if ( gridLines?.Visible == true )
            {
                builder.OpenElement( sequence++, "line" );
                builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( plot.Top ) );
                builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( plot.Bottom ) );
                AddGridLineAttributes( builder, ref sequence, gridLines );
                builder.CloseElement();
            }

            if ( valueLabels.Visible )
            {
                builder.OpenElement( sequence++, "text" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( plot.Bottom + valueLabels.Offset ) );
                builder.AddAttribute( sequence++, "text-anchor", "middle" );
                SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.68 );
                builder.AddContent( sequence++, FormatAxisLabel( FormatValueTick( model.PrimaryValueAxis, tick, i ), valueLabels, model.Options ) );
                builder.CloseElement();
            }
        }

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( plot.Left ) );
        builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( plot.Right ) );
        builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "stroke", "currentColor" );
        builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
        builder.CloseElement();

        var categoryLabels = model.CategoryAxis?.Labels ?? new();

        if ( categoryLabels.Visible )
        {
            for ( var i = 0; i < model.Labels.Count; i++ )
            {
                var y = plot.Top + plot.Height * ( i + 0.5 ) / Math.Max( model.Labels.Count, 1 );

                builder.OpenElement( sequence++, "text" );
                builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( plot.Left - 10 ) );
                builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y + 4 ) );
                builder.AddAttribute( sequence++, "text-anchor", "end" );
                SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.72 );
                builder.AddContent( sequence++, FormatAxisLabel( FormatCategoryTick( model, model.Labels[i], i ), categoryLabels, model.Options, Math.Max( 1, plot.Left - 14 ) ) );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    private static void RenderCategoryAxisGridLines( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model, SvgChartPlotArea plot, SvgChartStreamingAnimation streamingAnimation, string plotClipPathId )
    {
        var gridLines = model.CategoryAxis.GridLines;

        if ( gridLines?.Visible != true )
            return;

        var labels = model.CategoryAxis.Labels ?? new();
        var labelStep = Math.Max( 1, labels.Step );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid svg-chart-category-grid" );
        builder.AddAttribute( sequence++, "clip-path", $"url(#{plotClipPathId})" );

        if ( streamingAnimation.Enabled )
        {
            builder.OpenElement( sequence++, "g" );
            builder.SetKey( $"streaming-category-grid-{streamingAnimation.Version}" );
            builder.AddAttribute( sequence++, "style", streamingAnimation.Style );
            AddStreamingAnimationAttributes( builder, ref sequence, streamingAnimation );
        }

        for ( var i = 0; i < model.Labels.Count; i++ )
        {
            var labelIndex = i < model.CategoryLabelIndexes.Count ? model.CategoryLabelIndexes[i] : i;

            if ( labelIndex % labelStep != 0 )
                continue;

            var x = SvgChartGeometry.GetCategoryX( i, plot, model );

            builder.OpenElement( sequence++, "line" );
            builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( x ) );
            builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( x ) );
            builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( plot.Top ) );
            builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( plot.Bottom ) );
            AddGridLineAttributes( builder, ref sequence, gridLines );
            builder.CloseElement();
        }

        if ( streamingAnimation.Enabled )
            builder.CloseElement();

        builder.CloseElement();
    }

    private static void RenderPointXAxisGridAndLabels( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model, SvgChartPlotArea plot, string plotClipPathId, string categoryAxisLabelsClipPathId )
    {
        var scale = SvgChartGeometry.ResolvePointXScale( model );

        if ( scale is null )
            return;

        var gridLines = model.CategoryAxis.GridLines;
        var labels = model.CategoryAxis.Labels ?? new();

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid svg-chart-point-xaxis-grid" );
        builder.AddAttribute( sequence++, "clip-path", $"url(#{plotClipPathId})" );

        foreach ( var tick in scale.Ticks )
        {
            var x = SvgChartGeometry.GetX( tick, plot, scale.Min, scale.Max );

            if ( gridLines?.Visible == true )
            {
                builder.OpenElement( sequence++, "line" );
                builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( x ) );
                builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( plot.Top ) );
                builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( plot.Bottom ) );
                AddGridLineAttributes( builder, ref sequence, gridLines );
                builder.CloseElement();
            }
        }

        builder.CloseElement();

        if ( !labels.Visible )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-axis-labels svg-chart-point-xaxis-labels" );
        builder.AddAttribute( sequence++, "clip-path", $"url(#{categoryAxisLabelsClipPathId})" );

        for ( var i = 0; i < scale.Ticks.Count; i += Math.Max( 1, labels.Step ) )
        {
            var tick = scale.Ticks[i];
            var x = SvgChartGeometry.GetX( tick, plot, scale.Min, scale.Max );

            if ( x < plot.Left - 0.5 || x > plot.Right + 0.5 )
                continue;

            var labelPlacement = ResolvePointXAxisLabelPlacement( x, plot );

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( labelPlacement.X ) );
            builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( plot.Bottom + labels.Offset ) );
            builder.AddAttribute( sequence++, "text-anchor", labelPlacement.TextAnchor );
            SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.72 );
            builder.AddContent( sequence++, FormatAxisLabel( FormatCategoryTick( model, tick, i ), labels, model.Options ) );
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    private static (double X, string TextAnchor) ResolvePointXAxisLabelPlacement( double x, SvgChartPlotArea plot )
    {
        const double edgeTolerance = 0.5;

        if ( x <= plot.Left + edgeTolerance )
            return (plot.Left, "start");

        if ( x >= plot.Right - edgeTolerance )
            return (plot.Right, "end");

        return (x, "middle");
    }

    private static void RenderRightValueAxes( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model, SvgChartPlotArea plot, SvgChartRenderValueAxis primaryAxis )
    {
        foreach ( var axis in model.ValueAxes.Where( x => x != primaryAxis && x.Position == SvgChartAxisPosition.Right ) )
        {
            var labels = axis.Labels ?? new();

            builder.OpenElement( sequence++, "g" );
            builder.AddAttribute( sequence++, "class", "svg-chart-axis svg-chart-value-axis-right" );

            builder.OpenElement( sequence++, "line" );
            builder.AddAttribute( sequence++, "x1", SvgChartRenderHelpers.Format( plot.Right ) );
            builder.AddAttribute( sequence++, "x2", SvgChartRenderHelpers.Format( plot.Right ) );
            builder.AddAttribute( sequence++, "y1", SvgChartRenderHelpers.Format( plot.Top ) );
            builder.AddAttribute( sequence++, "y2", SvgChartRenderHelpers.Format( plot.Bottom ) );
            builder.AddAttribute( sequence++, "stroke", "currentColor" );
            builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
            builder.CloseElement();

            if ( labels.Visible )
            {
                for ( var i = 0; i < axis.Ticks.Count; i++ )
                {
                    var tick = axis.Ticks[i];
                    var y = SvgChartGeometry.GetY( tick, plot, axis );

                    builder.OpenElement( sequence++, "text" );
                    builder.AddAttribute( sequence++, "x", SvgChartRenderHelpers.Format( plot.Right + 10 ) );
                    builder.AddAttribute( sequence++, "y", SvgChartRenderHelpers.Format( y + 4 ) );
                    builder.AddAttribute( sequence++, "text-anchor", "start" );
                    SvgChartTextRenderer.AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.68 );
                    builder.AddContent( sequence++, FormatAxisLabel( FormatValueTick( axis, tick, i ), labels, model.Options ) );
                    builder.CloseElement();
                }
            }

            builder.CloseElement();
        }
    }

    private static void AddGridLineAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartGridLinesOptions gridLines )
    {
        builder.AddAttribute( sequence++, "stroke", ResolveGridLineColor( gridLines ) );
        builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( Math.Max( 0, gridLines?.Width ?? 1 ) ) );
        builder.AddAttribute( sequence++, "stroke-opacity", SvgChartRenderHelpers.Format( Math.Clamp( gridLines?.Opacity ?? 0.14, 0, 1 ) ) );

        if ( !string.IsNullOrWhiteSpace( gridLines?.DashPattern ) )
            builder.AddAttribute( sequence++, "stroke-dasharray", gridLines.DashPattern );
    }

    private static string ResolveGridLineColor( SvgChartGridLinesOptions gridLines )
    {
        var color = gridLines?.Color;

        if ( SvgChartRenderHelpers.IsDefaultColor( color ) )
            return "currentColor";

        return SvgChartRenderHelpers.ResolveColor( color, 0 );
    }

    private static void AddStreamingAnimationAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartStreamingAnimation animation )
    {
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-animation", "true" );
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-version", animation.Version.ToString( System.Globalization.CultureInfo.InvariantCulture ) );
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-offset", SvgChartRenderHelpers.Format( animation.OffsetX ) );
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-duration", SvgChartRenderHelpers.FormatDuration( animation.Duration ) );
    }

    private static string FormatCategoryTick( SvgChartRenderModel model, object value, int index )
    {
        return model.CategoryTickFormatter?.Invoke( new()
        {
            Value = value,
            Index = index,
            CategoryAxis = true,
            AxisId = model.CategoryAxis?.Id
        } ) ?? SvgChartRenderHelpers.FormatDataLabelValue( value );
    }

    private static string FormatValueTick( SvgChartRenderValueAxis axis, double value, int index )
    {
        return axis.TickFormatter?.Invoke( new()
        {
            Value = value,
            Index = index,
            CategoryAxis = false,
            AxisId = axis.Id
        } ) ?? SvgChartRenderHelpers.FormatTick( value );
    }

    private static string FormatAxisLabel( string label, SvgChartAxisLabelsOptions labels, SvgChartOptions options, double? autoMaxWidth = null )
    {
        var maxWidth = labels?.MaxWidth ?? autoMaxWidth;

        if ( string.IsNullOrEmpty( label ) || !maxWidth.HasValue )
            return label;

        var fontSize = options?.Font?.Size ?? 11;

        if ( SvgChartRenderHelpers.EstimateTextWidth( label, fontSize ) <= maxWidth.Value )
            return label;

        var maxCharacters = Math.Max( 1, (int)Math.Floor( maxWidth.Value / ( fontSize * 0.58 ) ) );

        if ( label.Length <= maxCharacters )
            return label;

        return maxCharacters <= 3
            ? label[..maxCharacters]
            : label[..( maxCharacters - 3 )] + "...";
    }

    #endregion
}