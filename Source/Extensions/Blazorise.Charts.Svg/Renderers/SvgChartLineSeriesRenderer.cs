#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartLineSeriesContent : SvgChartSeriesContentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedLinePoint> points = [];

    private SvgChartLineSeriesState state;

    private SvgChartSeriesProjectionState? pointsProjectionState;

    #endregion

    #region Methods

    protected override bool UpdateRenderState()
    {
        var resolvedState = new SvgChartLineSeriesState(
            Series.Color,
            Series.StrokeWidth,
            Series.OutlineColor,
            Series.OutlineStrokeWidth,
            Series.OutlineOpacity,
            Series.MarkerRadius,
            Series.Interpolation,
            Series.Tension,
            Context.PassThroughSeriesPaths );
        var projectionState = Context.GetProjectionState( Series, false );
        var shouldResolvePoints = Context.Animation.Enabled
            || !pointsProjectionState.HasValue
            || projectionState != pointsProjectionState.Value
            || !SeriesInputEquals( Context, Series, points );

        if ( shouldResolvePoints )
        {
            points = ResolvePoints( Context, Series );
            pointsProjectionState = projectionState;
        }

        var shouldRender = shouldResolvePoints || resolvedState != state;
        state = resolvedState;

        return shouldRender;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        if ( points.Count > 1 )
        {
            var linePoints = new List<(int Index, double X, double Y, double Value)>( points.Count );

            foreach ( var point in points )
                linePoints.Add( (point.PointIndex, point.X, point.Y, point.Value) );

            var linePath = SvgChartSeriesRenderHelpers.BuildLinePath( linePoints, Series.Interpolation, Series.Tension );

            if ( Series.OutlineStrokeWidth > 0 && !string.IsNullOrWhiteSpace( Series.OutlineColor ) )
            {
                builder.OpenElement( sequence++, "path" );
                builder.AddAttribute( sequence++, "class", "svg-chart-line-outline" );
                builder.AddAttribute( sequence++, "d", linePath );
                builder.AddAttribute( sequence++, "fill", "none" );
                builder.AddAttribute( sequence++, "stroke", Series.OutlineColor );
                builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( Series.OutlineStrokeWidth ) );
                builder.AddAttribute( sequence++, "stroke-opacity", SvgChartRenderHelpers.Format( Math.Clamp( Series.OutlineOpacity, 0, 1 ) ) );
                builder.AddAttribute( sequence++, "stroke-linecap", "round" );
                builder.AddAttribute( sequence++, "stroke-linejoin", "round" );
                builder.AddAttribute( sequence++, "pointer-events", "none" );
                Context.RenderPathFadeAnimation( builder, ref sequence, Series, "outline", linePath, "1" );
                builder.CloseElement();
            }

            builder.OpenElement( sequence++, "path" );
            builder.AddAttribute( sequence++, "class", "svg-chart-line" );
            builder.AddAttribute( sequence++, "d", linePath );
            builder.AddAttribute( sequence++, "fill", "none" );
            builder.AddAttribute( sequence++, "stroke", Series.Color );
            builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( Series.StrokeWidth ) );
            builder.AddAttribute( sequence++, "stroke-linecap", "round" );
            builder.AddAttribute( sequence++, "stroke-linejoin", "round" );
            if ( Context.PassThroughSeriesPaths )
                builder.AddAttribute( sequence++, "pointer-events", "none" );
            Context.RenderPathFadeAnimation( builder, ref sequence, Series, "line", linePath, "1" );
            builder.CloseElement();
        }

        if ( Series.MarkerRadius > 0 )
        {
            var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

            foreach ( var renderedPoint in points )
            {
                var bounds = new SvgChartPointBounds
                {
                    X = renderedPoint.X - Series.MarkerRadius,
                    Y = renderedPoint.Y - Series.MarkerRadius,
                    Width = Series.MarkerRadius * 2,
                    Height = Series.MarkerRadius * 2
                };
                var point = new SvgChartPointEventArgs
                {
                    SeriesName = Series.Name,
                    SeriesIndex = seriesIndex,
                    PointIndex = renderedPoint.PointIndex,
                    Category = renderedPoint.Category,
                    Value = renderedPoint.Value,
                    Bounds = bounds
                };
                var animationKey = Context.TrackPointBounds( Series, renderedPoint.PointIndex, bounds );
                var xString = SvgChartRenderHelpers.Format( renderedPoint.X );
                var yString = SvgChartRenderHelpers.Format( renderedPoint.Y );
                var radiusString = SvgChartRenderHelpers.Format( Series.MarkerRadius );

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-marker" );
                builder.AddAttribute( sequence++, "cx", xString );
                builder.AddAttribute( sequence++, "cy", yString );
                builder.AddAttribute( sequence++, "r", radiusString );
                builder.AddAttribute( sequence++, "fill", renderedPoint.Color );
                builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
                builder.AddAttribute( sequence++, "stroke-width", "1.5" );
                Context.AddPointInteractionAttributes( builder, ref sequence, point, renderedPoint.Color );
                Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cx", xString, xString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.X + previousBounds.Width / 2 ) );
                Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cy", yString, yString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Y + previousBounds.Height / 2 ) );
                Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "r", "0", radiusString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Width / 2 ) );
                builder.CloseElement();
            }
        }
    }

    private static List<SvgChartRenderedLinePoint> ResolvePoints( SvgChartSeriesRendererContext context, SvgChartPluginSeries series )
    {
        var chart = context.Chart;
        var hasMarkers = series.MarkerRadius > 0;
        var result = new List<SvgChartRenderedLinePoint>( Math.Min( chart.Labels.Count, series.Values.Count ) );

        for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            result.Add( new(
                pointIndex,
                hasMarkers ? chart.Labels[pointIndex] : null,
                chart.ContinuousCategoryAxis && pointIndex < series.XValues.Count && series.XValues[pointIndex].HasValue
                    ? series.XValues[pointIndex].Value
                    : pointIndex,
                ResolveX( chart, series, pointIndex ),
                chart.ProjectY( value.Value, series.ValueAxisId ),
                value.Value,
                hasMarkers ? series.GetPointColor( pointIndex ) : null ) );
        }

        return result;
    }

    private static bool SeriesInputEquals( SvgChartSeriesRendererContext context, SvgChartPluginSeries series, IReadOnlyList<SvgChartRenderedLinePoint> renderedPoints )
    {
        var chart = context.Chart;
        var hasMarkers = series.MarkerRadius > 0;
        var renderedPointIndex = 0;

        for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            if ( renderedPointIndex >= renderedPoints.Count )
                return false;

            var renderedPoint = renderedPoints[renderedPointIndex++];
            var category = hasMarkers ? chart.Labels[pointIndex] : null;
            var xValue = chart.ContinuousCategoryAxis && pointIndex < series.XValues.Count && series.XValues[pointIndex].HasValue
                ? series.XValues[pointIndex].Value
                : pointIndex;
            var color = hasMarkers ? series.GetPointColor( pointIndex ) : null;

            if ( renderedPoint.PointIndex != pointIndex
                 || !Equals( renderedPoint.Category, category )
                 || renderedPoint.XValue != xValue
                 || renderedPoint.Value != value.Value
                 || !string.Equals( renderedPoint.Color, color, StringComparison.Ordinal ) )
                return false;
        }

        return renderedPointIndex == renderedPoints.Count;
    }

    private static double ResolveX( SvgChartPluginRenderContext chart, SvgChartPluginSeries series, int pointIndex )
    {
        if ( chart.ContinuousCategoryAxis && pointIndex < series.XValues.Count && series.XValues[pointIndex].HasValue )
            return chart.ProjectX( series.XValues[pointIndex].Value, series.CategoryAxisId );

        return chart.ProjectCategory( pointIndex, series.CategoryAxisId );
    }

    #endregion
}

internal readonly record struct SvgChartRenderedLinePoint(
    int PointIndex,
    object Category,
    double XValue,
    double X,
    double Y,
    double Value,
    string Color );

internal readonly record struct SvgChartLineSeriesState(
    string Color,
    double StrokeWidth,
    string OutlineColor,
    double OutlineStrokeWidth,
    double OutlineOpacity,
    double MarkerRadius,
    SvgChartInterpolationMode Interpolation,
    double Tension,
    bool PassThroughSeriesPaths );

internal sealed class SvgChartLineSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Line;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => x.Type == SvgChartType.Line && context.ShouldRenderSeries( x ) ).ToList();

        if ( renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-lines" );

        foreach ( var item in renderSeries )
            context.RenderRetainedSeries<SvgChartLineSeriesContent>( builder, ref sequence, item, "svg-chart-line-series", item.Hidden );

        builder.CloseElement();
    }

    #endregion
}