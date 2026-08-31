#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartRadarSeriesContent : SvgChartSeriesContentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedRadarPoint> points = [];

    private SvgChartRadarSeriesState state;

    private SvgChartRadarProjectionState? pointsProjectionState;

    private SvgChartSeriesProjectionTransform transform = SvgChartSeriesProjectionTransform.Identity;

    #endregion

    #region Methods

    protected override bool UpdateRenderState()
    {
        var resolvedState = new SvgChartRadarSeriesState( Series.Color, Series.FillOpacity );
        var projectionState = ResolveProjectionState( Context );
        var canTransformPoints = !Context.Animation.Enabled
            && pointsProjectionState.HasValue
            && projectionState.CanTransformFrom( pointsProjectionState.Value )
            && SeriesInputEquals( Context, Series, points );

        if ( canTransformPoints )
        {
            var resolvedTransform = projectionState.CreateTransformFrom( pointsProjectionState.Value );

            var shouldRender = resolvedTransform != transform || resolvedState != state;
            transform = resolvedTransform;
            state = resolvedState;

            return shouldRender;
        }

        points = ResolvePoints( Context, Series, projectionState );
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
        builder.AddAttribute( sequence++, "class", "svg-chart-radar-geometry" );
        var transformSequence = sequence++;
        var markerScaleStyleSequence = sequence++;

        if ( !transform.IsIdentity )
        {
            builder.AddAttribute( transformSequence, "transform", transform.ToTransformString() );
            builder.AddAttribute( markerScaleStyleSequence, "style", transform.ToMarkerScaleStyleString() );
        }

        builder.OpenComponent<SvgChartRadarSeriesGeometry>( sequence++ );
        builder.AddAttribute( sequence++, nameof( SvgChartRadarSeriesGeometry.Context ), Context );
        builder.AddAttribute( sequence++, nameof( SvgChartRadarSeriesGeometry.Series ), Series );
        builder.AddAttribute( sequence++, nameof( SvgChartRadarSeriesGeometry.Points ), points );
        builder.AddAttribute( sequence++, nameof( SvgChartRadarSeriesGeometry.State ), state );
        builder.AddAttribute( sequence++, nameof( SvgChartRadarSeriesGeometry.Transform ), transform );
        builder.CloseComponent();

        builder.CloseElement();
    }

    private static SvgChartRadarProjectionState ResolveProjectionState( SvgChartSeriesRendererContext context )
    {
        var chart = context.Chart;
        var plot = chart.PlotArea;

        return new(
            chart.Labels.Count,
            plot.Left + plot.Width / 2,
            plot.Top + plot.Height / 2,
            Math.Max( 1, Math.Min( plot.Width, plot.Height ) * 0.42 ),
            Math.Max( chart.ValueMax, 1 ),
            context.CategoryFormatterKey );
    }

    private static List<SvgChartRenderedRadarPoint> ResolvePoints( SvgChartSeriesRendererContext context, SvgChartPluginSeries series, SvgChartRadarProjectionState projection )
    {
        var chart = context.Chart;
        var result = new List<SvgChartRenderedRadarPoint>( series.Values.Count );

        for ( var pointIndex = 0; pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];
            var areaAngle = -Math.PI / 2 + Math.PI * 2 * pointIndex / series.Values.Count;
            var areaRadius = projection.Radius * Math.Max( value ?? 0, 0 ) / projection.Max;
            var areaPoint = SvgChartSeriesRenderHelpers.PolarToCartesian( projection.CenterX, projection.CenterY, areaRadius, areaAngle );
            var hasMarker = value.HasValue && pointIndex < chart.Labels.Count;
            var markerAngle = hasMarker ? -Math.PI / 2 + Math.PI * 2 * pointIndex / chart.Labels.Count : areaAngle;
            var markerPoint = SvgChartSeriesRenderHelpers.PolarToCartesian( projection.CenterX, projection.CenterY, areaRadius, markerAngle );

            result.Add( new(
                pointIndex,
                hasMarker ? chart.Labels[pointIndex] : null,
                value,
                areaPoint.X,
                areaPoint.Y,
                markerPoint.X,
                markerPoint.Y,
                hasMarker,
                hasMarker ? series.GetPointColor( pointIndex ) : null ) );
        }

        return result;
    }

    private static bool SeriesInputEquals( SvgChartSeriesRendererContext context, SvgChartPluginSeries series, IReadOnlyList<SvgChartRenderedRadarPoint> renderedPoints )
    {
        var chart = context.Chart;

        if ( series.Values.Count != renderedPoints.Count )
            return false;

        for ( var pointIndex = 0; pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];
            var renderedPoint = renderedPoints[pointIndex];
            var hasMarker = value.HasValue && pointIndex < chart.Labels.Count;

            if ( renderedPoint.PointIndex != pointIndex
                 || !Equals( renderedPoint.Category, hasMarker ? chart.Labels[pointIndex] : null )
                 || renderedPoint.Value != value
                 || renderedPoint.HasMarker != hasMarker
                 || !string.Equals( renderedPoint.Color, hasMarker ? series.GetPointColor( pointIndex ) : null, StringComparison.Ordinal ) )
                return false;
        }

        return true;
    }

    #endregion
}

internal sealed class SvgChartRadarSeriesGeometry : ComponentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedRadarPoint> renderedPoints = [];

    private SvgChartRadarSeriesState renderedState;

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
            {
                if ( renderedPoint.HasMarker )
                    Context.UpdatePointInteraction( CreatePoint( renderedPoint, Transform, seriesIndex ), renderedPoint.Color );
            }
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
        var radarPoints = BuildRadarPoints( Points );

        builder.OpenElement( sequence++, "polygon" );
        builder.AddAttribute( sequence++, "class", "svg-chart-radar-area" );
        builder.AddAttribute( sequence++, "points", radarPoints );
        builder.AddAttribute( sequence++, "fill", Series.Color );
        builder.AddAttribute( sequence++, "opacity", SvgChartRenderHelpers.Format( Series.FillOpacity ) );
        builder.AddAttribute( sequence++, "stroke", Series.Color );
        builder.AddAttribute( sequence++, "stroke-width", "2" );
        builder.AddAttribute( sequence++, "vector-effect", "non-scaling-stroke" );
        Context.RenderPathFadeAnimation( builder, ref sequence, Series, "area", radarPoints, SvgChartRenderHelpers.Format( Series.FillOpacity ) );
        builder.CloseElement();

        foreach ( var renderedPoint in Points )
        {
            if ( !renderedPoint.HasMarker )
                continue;

            const double markerRadius = 4;
            var point = CreatePoint( renderedPoint, Transform, seriesIndex );
            var bounds = point.Bounds;
            var animationKey = Context.TrackPointBounds( Series, renderedPoint.PointIndex, bounds );
            var xString = SvgChartRenderHelpers.Format( renderedPoint.MarkerX );
            var yString = SvgChartRenderHelpers.Format( renderedPoint.MarkerY );
            var radiusString = SvgChartRenderHelpers.Format( markerRadius );

            builder.OpenElement( sequence++, "circle" );
            builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-radar-marker" );
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

    private static string BuildRadarPoints( IReadOnlyList<SvgChartRenderedRadarPoint> points )
    {
        var builder = new StringBuilder();

        for ( var i = 0; i < points.Count; i++ )
        {
            if ( i > 0 )
                builder.Append( ' ' );

            builder.Append( SvgChartRenderHelpers.Format( points[i].AreaX ) );
            builder.Append( ',' );
            builder.Append( SvgChartRenderHelpers.Format( points[i].AreaY ) );
        }

        return builder.ToString();
    }

    private SvgChartPointEventArgs CreatePoint( SvgChartRenderedRadarPoint renderedPoint, SvgChartSeriesProjectionTransform transform, int seriesIndex )
    {
        const double markerRadius = 4;
        var x = transform.ProjectX( renderedPoint.MarkerX );
        var y = transform.ProjectY( renderedPoint.MarkerY );

        return new()
        {
            SeriesName = Series.Name,
            SeriesIndex = seriesIndex,
            PointIndex = renderedPoint.PointIndex,
            Category = renderedPoint.Category,
            Value = renderedPoint.Value.Value,
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

    [Parameter] public IReadOnlyList<SvgChartRenderedRadarPoint> Points { get; set; } = [];

    [Parameter] public SvgChartRadarSeriesState State { get; set; }

    [Parameter] public SvgChartSeriesProjectionTransform Transform { get; set; }

    #endregion
}

internal readonly record struct SvgChartRenderedRadarPoint(
    int PointIndex,
    object Category,
    double? Value,
    double AreaX,
    double AreaY,
    double MarkerX,
    double MarkerY,
    bool HasMarker,
    string Color );

internal readonly record struct SvgChartRadarSeriesState(
    string Color,
    double FillOpacity );

internal readonly record struct SvgChartRadarProjectionState(
    int LabelCount,
    double CenterX,
    double CenterY,
    double Radius,
    double Max,
    object CategoryFormatterKey )
{
    public bool CanTransformFrom( SvgChartRadarProjectionState source )
    {
        return LabelCount == source.LabelCount
            && Equals( CategoryFormatterKey, source.CategoryFormatterKey );
    }

    public SvgChartSeriesProjectionTransform CreateTransformFrom( SvgChartRadarProjectionState source )
    {
        var scale = Radius / Max / ( source.Radius / source.Max );

        return new(
            scale,
            scale,
            CenterX - scale * source.CenterX,
            CenterY - scale * source.CenterY );
    }
}

internal sealed class SvgChartRadarSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Radar;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => x.Type == SvgChartType.Radar && context.ShouldRenderSeries( x ) ).ToList();

        if ( renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        var plot = chart.PlotArea;
        var centerX = plot.Left + plot.Width / 2;
        var centerY = plot.Top + plot.Height / 2;
        var radius = Math.Max( 1, Math.Min( plot.Width, plot.Height ) * 0.42 );
        var max = Math.Max( chart.ValueMax, 1 );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-radar" );

        for ( var i = 1; i <= 4; i++ )
        {
            builder.OpenElement( sequence++, "polygon" );
            builder.AddAttribute( sequence++, "points", SvgChartSeriesRenderHelpers.BuildRadarPoints( Enumerable.Repeat( max * i / 4, chart.Labels.Count ).Select( x => (double?)x ).ToList(), centerX, centerY, radius, max ) );
            builder.AddAttribute( sequence++, "fill", "none" );
            builder.AddAttribute( sequence++, "stroke", "currentColor" );
            builder.AddAttribute( sequence++, "stroke-opacity", "0.12" );
            builder.CloseElement();
        }

        foreach ( var item in renderSeries )
            context.RenderRetainedSeries<SvgChartRadarSeriesContent>( builder, ref sequence, item, "svg-chart-radar-series", item.Hidden );

        builder.CloseElement();
    }

    #endregion
}