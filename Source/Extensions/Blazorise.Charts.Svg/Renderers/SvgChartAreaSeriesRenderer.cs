#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartAreaSeriesContent : SvgChartSeriesContentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedAreaPoint> points = [];

    private SvgChartAreaSeriesState state;

    private SvgChartSeriesProjectionState? pointsProjectionState;

    private SvgChartSeriesProjectionTransform transform = SvgChartSeriesProjectionTransform.Identity;

    #endregion

    #region Methods

    protected override bool UpdateRenderState()
    {
        var resolvedState = new SvgChartAreaSeriesState(
            Series.Color,
            Series.StrokeWidth,
            Series.FillOpacity,
            Series.Interpolation,
            Series.Tension,
            Context.PassThroughSeriesPaths );
        var projectionState = Context.GetProjectionState( Series, false );
        var canTransformPoints = !Context.Animation.Enabled
            && pointsProjectionState.HasValue
            && projectionState.CanTransformFrom( pointsProjectionState.Value )
            && SeriesInputEquals( Context, Series, points );

        if ( canTransformPoints )
        {
            var resolvedTransform = SvgChartSeriesProjectionTransform.Create( pointsProjectionState.Value, projectionState );

            var shouldRender = resolvedTransform != transform || resolvedState != state;
            transform = resolvedTransform;
            state = resolvedState;

            return shouldRender;
        }

        points = ResolvePoints( Context, Series );
        pointsProjectionState = projectionState;
        transform = SvgChartSeriesProjectionTransform.Identity;
        state = resolvedState;

        return true;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-area-geometry" );
        var transformSequence = sequence++;
        var markerScaleStyleSequence = sequence++;

        if ( !transform.IsIdentity )
        {
            builder.AddAttribute( transformSequence, "transform", transform.ToTransformString() );
            builder.AddAttribute( markerScaleStyleSequence, "style", transform.ToMarkerScaleStyleString() );
        }

        builder.OpenComponent<SvgChartAreaSeriesGeometry>( sequence++ );
        builder.AddAttribute( sequence++, nameof( SvgChartAreaSeriesGeometry.Context ), Context );
        builder.AddAttribute( sequence++, nameof( SvgChartAreaSeriesGeometry.Series ), Series );
        builder.AddAttribute( sequence++, nameof( SvgChartAreaSeriesGeometry.Points ), points );
        builder.AddAttribute( sequence++, nameof( SvgChartAreaSeriesGeometry.State ), state );
        builder.AddAttribute( sequence++, nameof( SvgChartAreaSeriesGeometry.Transform ), transform );
        builder.CloseComponent();

        builder.CloseElement();
    }

    private static List<SvgChartRenderedAreaPoint> ResolvePoints( SvgChartSeriesRendererContext context, SvgChartPluginSeries series )
    {
        var chart = context.Chart;
        var baseline = chart.ProjectY( 0, series.ValueAxisId );
        var result = new List<SvgChartRenderedAreaPoint>( Math.Min( chart.Labels.Count, series.Values.Count ) );

        for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            var startValue = ResolveStackValue( series.StackBaseValues, pointIndex, 0 );
            var endValue = ResolveStackValue( series.StackEndValues, pointIndex, value.Value );
            var xValue = chart.ContinuousCategoryAxis && pointIndex < series.XValues.Count && series.XValues[pointIndex].HasValue
                ? series.XValues[pointIndex].Value
                : pointIndex;
            var x = ResolveX( chart, series, pointIndex );

            result.Add( new(
                pointIndex,
                chart.Labels[pointIndex],
                xValue,
                x,
                chart.ProjectY( endValue, series.ValueAxisId ),
                series.StackEndValues.Count > 0 ? chart.ProjectY( startValue, series.ValueAxisId ) : baseline,
                value.Value,
                startValue,
                endValue,
                series.GetPointColor( pointIndex ) ) );
        }

        return result;
    }

    private static bool SeriesInputEquals( SvgChartSeriesRendererContext context, SvgChartPluginSeries series, IReadOnlyList<SvgChartRenderedAreaPoint> renderedPoints )
    {
        var chart = context.Chart;
        var renderedPointIndex = 0;

        for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            if ( renderedPointIndex >= renderedPoints.Count )
                return false;

            var renderedPoint = renderedPoints[renderedPointIndex++];
            var startValue = ResolveStackValue( series.StackBaseValues, pointIndex, 0 );
            var endValue = ResolveStackValue( series.StackEndValues, pointIndex, value.Value );
            var xValue = chart.ContinuousCategoryAxis && pointIndex < series.XValues.Count && series.XValues[pointIndex].HasValue
                ? series.XValues[pointIndex].Value
                : pointIndex;

            if ( renderedPoint.PointIndex != pointIndex
                 || !Equals( renderedPoint.Category, chart.Labels[pointIndex] )
                 || renderedPoint.XValue != xValue
                 || renderedPoint.Value != value.Value
                 || renderedPoint.StartValue != startValue
                 || renderedPoint.EndValue != endValue
                 || !string.Equals( renderedPoint.Color, series.GetPointColor( pointIndex ), StringComparison.Ordinal ) )
                return false;
        }

        return renderedPointIndex == renderedPoints.Count;
    }

    private static double ResolveStackValue( IReadOnlyList<double?> values, int index, double fallback )
    {
        return index >= 0 && index < values.Count && values[index].HasValue ? values[index].Value : fallback;
    }

    private static double ResolveX( SvgChartPluginRenderContext chart, SvgChartPluginSeries series, int pointIndex )
    {
        if ( chart.ContinuousCategoryAxis && pointIndex < series.XValues.Count && series.XValues[pointIndex].HasValue )
            return chart.ProjectX( series.XValues[pointIndex].Value, series.CategoryAxisId );

        return chart.ProjectCategory( pointIndex, series.CategoryAxisId );
    }

    #endregion
}

internal sealed class SvgChartAreaSeriesGeometry : ComponentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedAreaPoint> renderedPoints = [];

    private SvgChartAreaSeriesState renderedState;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var shouldRender = Context?.Animation.Enabled == true
            || !ReferenceEquals( Points, renderedPoints )
            || State != renderedState;

        if ( !shouldRender && Context is not null && Series is not null )
        {
            var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

            foreach ( var renderedPoint in Points )
                Context.UpdatePointInteraction( CreatePoint( renderedPoint, Transform, seriesIndex ), renderedPoint.Color );
        }
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        return Context?.Animation.Enabled == true
            || !ReferenceEquals( Points, renderedPoints )
            || State != renderedState;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

        if ( Points.Count > 1 )
        {
            var linePoints = new List<(int Index, double X, double Y, double Value)>( Points.Count );
            var basePoints = new List<(int Index, double X, double Y, double Value)>( Points.Count );

            foreach ( var point in Points )
            {
                linePoints.Add( (point.PointIndex, point.X, point.Y, point.Value) );
                basePoints.Add( (point.PointIndex, point.X, point.BaseY, point.StartValue) );
            }

            var linePath = SvgChartSeriesRenderHelpers.BuildLinePath( linePoints, Series.Interpolation, Series.Tension );
            var areaPath = SvgChartSeriesRenderHelpers.BuildAreaPath( linePoints, basePoints, Series.Interpolation, Series.Tension );

            builder.OpenElement( sequence++, "path" );
            builder.AddAttribute( sequence++, "class", "svg-chart-area" );
            builder.AddAttribute( sequence++, "d", areaPath );
            builder.AddAttribute( sequence++, "fill", Series.Color );
            builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Series.FillOpacity ) );
            if ( Context.PassThroughSeriesPaths )
                builder.AddAttribute( sequence++, "pointer-events", "none" );
            Context.RenderPathFadeAnimation( builder, ref sequence, Series, "area", areaPath, SvgChartRenderHelpers.Format( Series.FillOpacity ) );
            builder.CloseElement();

            builder.OpenElement( sequence++, "path" );
            builder.AddAttribute( sequence++, "class", "svg-chart-area-line" );
            builder.AddAttribute( sequence++, "d", linePath );
            builder.AddAttribute( sequence++, "fill", "none" );
            builder.AddAttribute( sequence++, "stroke", Series.Color );
            builder.AddAttribute( sequence++, "stroke-width", SvgChartRenderHelpers.Format( Series.StrokeWidth ) );
            builder.AddAttribute( sequence++, "stroke-linecap", "round" );
            builder.AddAttribute( sequence++, "stroke-linejoin", "round" );
            builder.AddAttribute( sequence++, "vector-effect", "non-scaling-stroke" );
            if ( Context.PassThroughSeriesPaths )
                builder.AddAttribute( sequence++, "pointer-events", "none" );
            Context.RenderPathFadeAnimation( builder, ref sequence, Series, "line", linePath, "1" );
            builder.CloseElement();
        }

        foreach ( var renderedPoint in Points )
        {
            var markerRadius = Math.Max( 3, Series.StrokeWidth + 1 );
            var point = CreatePoint( renderedPoint, Transform, seriesIndex );
            var bounds = point.Bounds;
            var animationKey = Context.TrackPointBounds( Series, renderedPoint.PointIndex, bounds );
            var xString = SvgChartRenderHelpers.Format( renderedPoint.X );
            var yString = SvgChartRenderHelpers.Format( renderedPoint.Y );
            var radiusString = SvgChartRenderHelpers.Format( markerRadius );

            builder.OpenElement( sequence++, "circle" );
            builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-area-marker" );
            builder.AddAttribute( sequence++, "cx", xString );
            builder.AddAttribute( sequence++, "cy", yString );
            builder.AddAttribute( sequence++, "r", radiusString );
            builder.AddAttribute( sequence++, "fill", renderedPoint.Color );
            builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
            builder.AddAttribute( sequence++, "stroke-width", "1.5" );
            builder.AddAttribute( sequence++, "style", SvgChartSeriesProjectionTransform.MarkerTransformStyleString );
            Context.AddPointInteractionAttributes( builder, ref sequence, point, renderedPoint.Color );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cx", xString, xString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.X + previousBounds.Width / 2 ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cy", yString, yString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Y + previousBounds.Height / 2 ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "r", "0", radiusString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Width / 2 ) );
            builder.CloseElement();
        }

        renderedPoints = Points;
        renderedState = State;
    }

    private SvgChartPointEventArgs CreatePoint( SvgChartRenderedAreaPoint renderedPoint, SvgChartSeriesProjectionTransform transform, int seriesIndex )
    {
        var markerRadius = Math.Max( 3, Series.StrokeWidth + 1 );
        var x = transform.ProjectX( renderedPoint.X );
        var y = transform.ProjectY( renderedPoint.Y );

        return new()
        {
            SeriesName = Series.Name,
            SeriesIndex = seriesIndex,
            PointIndex = renderedPoint.PointIndex,
            Category = renderedPoint.Category,
            Value = renderedPoint.Value,
            Bounds = new()
            {
                X = x - markerRadius,
                Y = y - markerRadius,
                Width = markerRadius * 2,
                Height = markerRadius * 2
            }
        };
    }

    #endregion

    #region Properties

    [Parameter] public SvgChartSeriesRendererContext Context { get; set; }

    [Parameter] public SvgChartPluginSeries Series { get; set; }

    [Parameter] public IReadOnlyList<SvgChartRenderedAreaPoint> Points { get; set; } = [];

    [Parameter] public SvgChartAreaSeriesState State { get; set; }

    [Parameter] public SvgChartSeriesProjectionTransform Transform { get; set; }

    #endregion
}

internal readonly record struct SvgChartRenderedAreaPoint(
    int PointIndex,
    object Category,
    double XValue,
    double X,
    double Y,
    double BaseY,
    double Value,
    double StartValue,
    double EndValue,
    string Color );

internal readonly record struct SvgChartAreaSeriesState(
    string Color,
    double StrokeWidth,
    double FillOpacity,
    SvgChartInterpolationMode Interpolation,
    double Tension,
    bool PassThroughSeriesPaths );

internal sealed class SvgChartAreaSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Area;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => x.Type == SvgChartType.Area && context.ShouldRenderSeries( x ) ).ToList();

        if ( renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-areas" );

        foreach ( var item in renderSeries )
            context.RenderRetainedSeries<SvgChartAreaSeriesContent>( builder, ref sequence, item, "svg-chart-area-series", item.Hidden );

        builder.CloseElement();
    }

    #endregion
}