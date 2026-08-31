#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartPointSeriesContent : SvgChartSeriesContentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedPoint> points = [];

    private SvgChartSeriesProjectionState? pointsProjectionState;

    private SvgChartSeriesProjectionTransform transform = SvgChartSeriesProjectionTransform.Identity;

    #endregion

    #region Methods

    protected override bool UpdateRenderState()
    {
        var projectionState = Context.GetProjectionState( Series, true );
        var canTransformPoints = !Context.Animation.Enabled
            && pointsProjectionState.HasValue
            && projectionState.CanTransformFrom( pointsProjectionState.Value )
            && SeriesInputEquals( Context, Series, points );

        if ( canTransformPoints )
        {
            var resolvedTransform = SvgChartSeriesProjectionTransform.Create( pointsProjectionState.Value, projectionState );

            var shouldRender = resolvedTransform != transform;
            transform = resolvedTransform;

            return shouldRender;
        }

        points = ResolvePoints( Context, Series );
        pointsProjectionState = projectionState;
        transform = SvgChartSeriesProjectionTransform.Identity;

        return true;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-point-geometry" );
        var transformSequence = sequence++;
        var markerScaleStyleSequence = sequence++;

        if ( !transform.IsIdentity )
        {
            builder.AddAttribute( transformSequence, "transform", transform.ToTransformString() );
            builder.AddAttribute( markerScaleStyleSequence, "style", transform.ToMarkerScaleStyleString() );
        }

        builder.OpenComponent<SvgChartPointSeriesGeometry>( sequence++ );
        builder.AddAttribute( sequence++, nameof( SvgChartPointSeriesGeometry.Context ), Context );
        builder.AddAttribute( sequence++, nameof( SvgChartPointSeriesGeometry.Series ), Series );
        builder.AddAttribute( sequence++, nameof( SvgChartPointSeriesGeometry.Points ), points );
        builder.AddAttribute( sequence++, nameof( SvgChartPointSeriesGeometry.Transform ), transform );
        builder.CloseComponent();

        builder.CloseElement();
    }

    private static List<SvgChartRenderedPoint> ResolvePoints( SvgChartSeriesRendererContext context, SvgChartPluginSeries series )
    {
        var chart = context.Chart;
        var result = new List<SvgChartRenderedPoint>( series.YValues.Count );

        for ( var pointIndex = 0; pointIndex < series.YValues.Count; pointIndex++ )
        {
            var yValue = series.YValues[pointIndex];
            var xValue = pointIndex < series.XValues.Count ? series.XValues[pointIndex] : pointIndex;

            if ( !xValue.HasValue || !yValue.HasValue )
                continue;

            var x = chart.ProjectX( xValue.Value );
            var y = chart.ProjectY( yValue.Value, series.ValueAxisId );
            var radius = series.Type == SvgChartType.Bubble
                ? Math.Max( 2, pointIndex < series.RadiusValues.Count && series.RadiusValues[pointIndex].HasValue ? series.RadiusValues[pointIndex].Value : series.MarkerRadius )
                : series.MarkerRadius;
            var category = chart.ContinuousCategoryAxis && pointIndex < chart.Labels.Count
                ? chart.Labels[pointIndex]
                : xValue.Value;

            result.Add( new(
                pointIndex,
                category,
                yValue.Value,
                xValue.Value,
                x,
                y,
                radius,
                series.GetPointColor( pointIndex ),
                context.GetPointLabel( category, yValue.Value, series.Name, pointIndex ) ) );
        }

        return result;
    }

    private static bool SeriesInputEquals( SvgChartSeriesRendererContext context, SvgChartPluginSeries series, IReadOnlyList<SvgChartRenderedPoint> renderedPoints )
    {
        var chart = context.Chart;
        var renderedPointIndex = 0;

        for ( var pointIndex = 0; pointIndex < series.YValues.Count; pointIndex++ )
        {
            var yValue = series.YValues[pointIndex];
            var xValue = pointIndex < series.XValues.Count ? series.XValues[pointIndex] : pointIndex;

            if ( !xValue.HasValue || !yValue.HasValue )
                continue;

            if ( renderedPointIndex >= renderedPoints.Count )
                return false;

            var renderedPoint = renderedPoints[renderedPointIndex++];
            var radius = series.Type == SvgChartType.Bubble
                ? Math.Max( 2, pointIndex < series.RadiusValues.Count && series.RadiusValues[pointIndex].HasValue ? series.RadiusValues[pointIndex].Value : series.MarkerRadius )
                : series.MarkerRadius;
            var category = chart.ContinuousCategoryAxis && pointIndex < chart.Labels.Count
                ? chart.Labels[pointIndex]
                : xValue.Value;

            if ( renderedPoint.PointIndex != pointIndex
                 || !Equals( renderedPoint.Category, category )
                 || renderedPoint.Value != yValue.Value
                 || renderedPoint.XValue != xValue.Value
                 || renderedPoint.Radius != radius
                 || !string.Equals( renderedPoint.Color, series.GetPointColor( pointIndex ), StringComparison.Ordinal ) )
                return false;
        }

        return renderedPointIndex == renderedPoints.Count;
    }

    #endregion
}

internal sealed class SvgChartPointSeriesGeometry : ComponentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedPoint> renderedPoints = [];

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var shouldRender = Context?.Animation.Enabled == true || !ReferenceEquals( Points, renderedPoints );

        if ( !shouldRender && Context is not null && Series is not null )
        {
            var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

            foreach ( var renderedPoint in Points )
            {
                var point = CreatePoint( renderedPoint, Transform, seriesIndex, Series.Name );

                Context.UpdatePointInteraction( point, renderedPoint.Color );
            }
        }
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        return Context?.Animation.Enabled == true || !ReferenceEquals( Points, renderedPoints );
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

        foreach ( var renderedPoint in Points )
        {
            var point = CreatePoint( renderedPoint, Transform, seriesIndex, Series.Name );
            var bounds = point.Bounds;
            var animationKey = Context.TrackPointBounds( Series, renderedPoint.PointIndex, bounds );
            var xString = SvgChartRenderHelpers.Format( renderedPoint.X );
            var yString = SvgChartRenderHelpers.Format( renderedPoint.Y );
            var radiusString = SvgChartRenderHelpers.Format( renderedPoint.Radius );
            var opacityString = Series.Type == SvgChartType.Bubble ? "0.72" : "1";

            builder.OpenElement( sequence++, "circle" );
            builder.AddAttribute( sequence++, "class", $"svg-chart-point svg-chart-{Series.Type.ToString().ToLowerInvariant()}" );
            builder.AddAttribute( sequence++, "cx", xString );
            builder.AddAttribute( sequence++, "cy", yString );
            builder.AddAttribute( sequence++, "r", radiusString );
            builder.AddAttribute( sequence++, "fill", renderedPoint.Color );
            builder.AddAttribute( sequence++, "opacity", opacityString );
            builder.AddAttribute( sequence++, "style", SvgChartSeriesProjectionTransform.MarkerTransformStyleString );
            Context.AddPointInteractionAttributes( builder, ref sequence, point, renderedPoint.Color, renderedPoint.Label );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cx", xString, xString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.X + previousBounds.Width / 2 ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "cy", yString, yString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Y + previousBounds.Height / 2 ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "r", "0", radiusString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Width / 2 ) );
            Context.RenderInitialAttributeAnimation( builder, ref sequence, "opacity", "0", opacityString );
            builder.CloseElement();
        }

        renderedPoints = Points;
    }

    private static SvgChartPointEventArgs CreatePoint( SvgChartRenderedPoint renderedPoint, SvgChartSeriesProjectionTransform transform, int seriesIndex, string seriesName )
    {
        var x = transform.ProjectX( renderedPoint.X );
        var y = transform.ProjectY( renderedPoint.Y );

        return new()
        {
            SeriesName = seriesName,
            SeriesIndex = seriesIndex,
            PointIndex = renderedPoint.PointIndex,
            Category = renderedPoint.Category,
            Value = renderedPoint.Value,
            Bounds = new()
            {
                X = x - renderedPoint.Radius,
                Y = y - renderedPoint.Radius,
                Width = renderedPoint.Radius * 2,
                Height = renderedPoint.Radius * 2
            }
        };
    }

    #endregion

    #region Properties

    [Parameter] public SvgChartSeriesRendererContext Context { get; set; }

    [Parameter] public SvgChartPluginSeries Series { get; set; }

    [Parameter] public IReadOnlyList<SvgChartRenderedPoint> Points { get; set; } = [];

    [Parameter] public SvgChartSeriesProjectionTransform Transform { get; set; }

    #endregion
}

internal readonly record struct SvgChartRenderedPoint(
    int PointIndex,
    object Category,
    double Value,
    double XValue,
    double X,
    double Y,
    double Radius,
    string Color,
    string Label );

internal sealed class SvgChartPointSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type is SvgChartType.Scatter or SvgChartType.Bubble;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var renderSeries = series.Where( x => CanRender( x ) && context.ShouldRenderSeries( x ) ).ToList();

        if ( renderSeries.Count == 0 )
            return;

        foreach ( var item in renderSeries )
            context.RenderRetainedSeries<SvgChartPointSeriesContent>( builder, ref sequence, item, "svg-chart-points", item.Hidden );
    }

    #endregion
}