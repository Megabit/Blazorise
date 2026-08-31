#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartColumnSeriesContent : SvgChartSeriesContentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedRectangle> columns = [];

    private SvgChartRectangleSeriesState state;

    #endregion

    #region Methods

    protected override bool UpdateRenderState()
    {
        var resolvedState = new SvgChartRectangleSeriesState( Series.BorderRadius, Context.CategoryFormatterKey );
        var resolvedColumns = ResolveColumns( Context, Series );
        var shouldRender = Context.Animation.Enabled
            || resolvedState != state
            || !resolvedColumns.SequenceEqual( columns );
        columns = resolvedColumns;
        state = resolvedState;

        return shouldRender;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

        foreach ( var column in columns )
        {
            var point = column.CreatePoint( seriesIndex, Series.Name );
            var bounds = point.Bounds;
            var animationKey = Context.TrackPointBounds( Series, column.PointIndex, bounds );
            var xString = SvgChartRenderHelpers.Format( column.X );
            var yString = SvgChartRenderHelpers.Format( column.Y );
            var widthString = SvgChartRenderHelpers.Format( column.Width );
            var heightString = SvgChartRenderHelpers.Format( column.Height );

            builder.OpenElement( sequence++, "rect" );
            builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-column" );
            builder.AddAttribute( sequence++, "x", xString );
            builder.AddAttribute( sequence++, "y", yString );
            builder.AddAttribute( sequence++, "width", widthString );
            builder.AddAttribute( sequence++, "height", heightString );
            builder.AddAttribute( sequence++, "rx", SvgChartRenderHelpers.Format( Series.BorderRadius ) );
            builder.AddAttribute( sequence++, "fill", column.Color );
            Context.AddPointInteractionAttributes( builder, ref sequence, point, column.Color );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "x", xString, xString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.X ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "y", SvgChartRenderHelpers.Format( column.Baseline ), yString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Y ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "width", widthString, widthString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Width ) );
            Context.RenderPointBoundsAttributeAnimation( builder, ref sequence, animationKey, "height", "0", heightString, previousBounds => SvgChartRenderHelpers.Format( previousBounds.Height ) );
            builder.CloseElement();
        }
    }

    private static List<SvgChartRenderedRectangle> ResolveColumns( SvgChartSeriesRendererContext context, SvgChartPluginSeries series )
    {
        var chart = context.Chart;
        var visibleSeries = chart.Series.Where( x => x.Type == SvgChartType.Column && !x.Hidden ).ToList();
        var stackGroups = visibleSeries.Select( ResolveStackGroup ).Distinct().ToList();
        var result = new List<SvgChartRenderedRectangle>( Math.Min( chart.Labels.Count, series.Values.Count ) );

        if ( stackGroups.Count == 0 )
            return result;

        var categoryWidth = Math.Abs( chart.ProjectCategoryBoundary( 1 ) - chart.ProjectCategoryBoundary( 0 ) );
        var groupWidth = categoryWidth * 0.72;
        var barWidth = Math.Max( 1, groupWidth / stackGroups.Count );
        var seriesIndex = stackGroups.IndexOf( ResolveStackGroup( series ) );
        var baselineValue = Math.Clamp( 0, chart.GetValueMin( series.ValueAxisId ), chart.GetValueMax( series.ValueAxisId ) );
        var baseline = chart.ProjectY( baselineValue, series.ValueAxisId );

        for ( var pointIndex = 0; pointIndex < chart.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
        {
            var value = series.Values[pointIndex];

            if ( !value.HasValue )
                continue;

            var categoryStart = chart.ProjectCategoryBoundary( pointIndex ) + ( categoryWidth - groupWidth ) / 2;
            var x = categoryStart + barWidth * seriesIndex + barWidth * 0.1;
            var startValue = ResolveStackValue( series.StackBaseValues, pointIndex, baselineValue );
            var endValue = ResolveStackValue( series.StackEndValues, pointIndex, value.Value );
            var startY = chart.ProjectY( startValue, series.ValueAxisId );
            var endY = chart.ProjectY( endValue, series.ValueAxisId );
            var height = Math.Abs( startY - endY );
            var y = Math.Min( startY, endY );
            var width = Math.Max( 1, barWidth * 0.8 );

            result.Add( new(
                pointIndex,
                chart.Labels[pointIndex],
                value.Value,
                x,
                y,
                width,
                height,
                baseline,
                series.GetPointColor( pointIndex ) ) );
        }

        return result;
    }

    private static string ResolveStackGroup( SvgChartPluginSeries series )
    {
        return series.StackEndValues.Count > 0 ? series.Stack ?? string.Empty : series.Name;
    }

    private static double ResolveStackValue( IReadOnlyList<double?> values, int index, double fallback )
    {
        return index >= 0 && index < values.Count && values[index].HasValue ? values[index].Value : fallback;
    }

    #endregion
}

internal readonly record struct SvgChartRectangleSeriesState(
    double BorderRadius,
    object CategoryFormatterKey );

internal readonly record struct SvgChartRenderedRectangle(
    int PointIndex,
    object Category,
    double Value,
    double X,
    double Y,
    double Width,
    double Height,
    double Baseline,
    string Color )
{
    public SvgChartPointEventArgs CreatePoint( int seriesIndex, string seriesName )
    {
        return new()
        {
            SeriesName = seriesName,
            SeriesIndex = seriesIndex,
            PointIndex = this.PointIndex,
            Category = this.Category,
            Value = this.Value,
            Bounds = new()
            {
                X = this.X,
                Y = this.Y,
                Width = this.Width,
                Height = this.Height
            }
        };
    }
}

internal sealed class SvgChartColumnSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type == SvgChartType.Column;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var chart = context.Chart;
        var renderSeries = series.Where( x => x.Type == SvgChartType.Column && context.ShouldRenderSeries( x ) ).ToList();

        if ( renderSeries.Count == 0 || chart.Labels.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-columns" );

        foreach ( var item in renderSeries )
            context.RenderRetainedSeries<SvgChartColumnSeriesContent>( builder, ref sequence, item, "svg-chart-column-series", item.Hidden );

        builder.CloseElement();
    }

    #endregion
}