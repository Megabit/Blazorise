#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartRadialSeriesContent : SvgChartSeriesContentBase
{
    #region Members

    private IReadOnlyList<SvgChartRenderedRadialSegment> segments = [];

    private object categoryFormatterKey;

    #endregion

    #region Methods

    protected override bool UpdateRenderState()
    {
        var resolvedSegments = ResolveSegments( Context, Series, segments );
        var shouldRender = Context.Animation.Enabled
            || !Equals( Context.CategoryFormatterKey, categoryFormatterKey )
            || !resolvedSegments.SequenceEqual( segments );
        segments = resolvedSegments;
        categoryFormatterKey = Context.CategoryFormatterKey;

        return shouldRender;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;

        if ( Context is null || Series is null )
            return;

        var seriesIndex = Context.GetSeriesKey( Series ).SeriesIndex;

        foreach ( var segment in segments )
        {
            var point = CreatePoint( segment, seriesIndex, Series.Name );

            builder.OpenElement( sequence++, "path" );
            builder.SetKey( segment.PointIndex );
            builder.AddAttribute( sequence++, "class", $"svg-chart-point svg-chart-{Series.Type.ToString().ToLowerInvariant()}-segment" );
            var visibilitySequence = sequence++;
            var ariaHiddenSequence = sequence++;

            if ( segment.Hidden )
            {
                builder.AddAttribute( visibilitySequence, "visibility", "hidden" );
                builder.AddAttribute( ariaHiddenSequence, "aria-hidden", "true" );
            }

            builder.AddAttribute( sequence++, "d", segment.Path );
            builder.AddAttribute( sequence++, "fill", segment.Color );
            builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
            builder.AddAttribute( sequence++, "stroke-width", "1" );
            Context.AddPointInteractionAttributes( builder, ref sequence, point, segment.Color );
            Context.RenderInitialAttributeAnimation( builder, ref sequence, "opacity", "0", "1" );
            builder.CloseElement();
        }
    }

    private static List<SvgChartRenderedRadialSegment> ResolveSegments(
        SvgChartSeriesRendererContext context,
        SvgChartPluginSeries series,
        IReadOnlyList<SvgChartRenderedRadialSegment> renderedSegments )
    {
        var chart = context.Chart;
        var values = series.Values
            .Select( ( value, index ) => new
            {
                Value = value,
                Index = index,
                Hidden = chart.IsDataPointHidden( series.Name, index )
            } )
            .Where( x => x.Value.HasValue && x.Value.Value >= 0 )
            .ToList();
        var visibleValues = values.Where( x => !x.Hidden ).ToList();

        var renderedSegmentsByIndex = renderedSegments.ToDictionary( x => x.PointIndex );
        var result = new List<SvgChartRenderedRadialSegment>( values.Count );
        var plot = chart.PlotArea;
        var centerX = plot.Left + plot.Width / 2;
        var centerY = plot.Top + plot.Height / 2;
        var radius = Math.Max( 1, Math.Min( plot.Width, plot.Height ) * 0.42 );
        var total = series.Type == SvgChartType.PolarArea ? visibleValues.Count : visibleValues.Sum( x => x.Value.Value );
        var max = visibleValues.Count > 0 ? visibleValues.Max( x => x.Value.Value ) : 0;
        var startAngle = -Math.PI / 2;
        foreach ( var value in values )
        {
            if ( value.Hidden || total <= 0 )
            {
                if ( renderedSegmentsByIndex.TryGetValue( value.Index, out var renderedSegment ) )
                    result.Add( renderedSegment with { Hidden = true } );

                continue;
            }

            var sweep = series.Type == SvgChartType.PolarArea ? Math.PI * 2 / visibleValues.Count : value.Value.Value / total * Math.PI * 2;
            var endAngle = startAngle + sweep;
            var pointRadius = series.Type == SvgChartType.PolarArea ? radius * Math.Sqrt( value.Value.Value / Math.Max( max, 1 ) ) : radius;
            var innerRadius = series.Type == SvgChartType.Doughnut ? radius * 0.58 : 0;
            var category = value.Index < chart.Labels.Count ? chart.Labels[value.Index] : value.Index + 1;

            result.Add( new(
                value.Index,
                category,
                value.Value.Value,
                centerX,
                centerY,
                pointRadius,
                SvgChartSeriesRenderHelpers.BuildArcPath( centerX, centerY, innerRadius, pointRadius, startAngle, endAngle ),
                series.GetPointColor( value.Index ),
                false ) );

            startAngle = endAngle;
        }

        return result;
    }

    private static SvgChartPointEventArgs CreatePoint( SvgChartRenderedRadialSegment segment, int seriesIndex, string seriesName )
    {
        return new()
        {
            SeriesName = seriesName,
            SeriesIndex = seriesIndex,
            PointIndex = segment.PointIndex,
            Category = segment.Category,
            Value = segment.Value,
            Bounds = new()
            {
                X = segment.CenterX - segment.Radius,
                Y = segment.CenterY - segment.Radius,
                Width = segment.Radius * 2,
                Height = segment.Radius * 2
            }
        };
    }

    #endregion
}

internal readonly record struct SvgChartRenderedRadialSegment(
    int PointIndex,
    object Category,
    double Value,
    double CenterX,
    double CenterY,
    double Radius,
    string Path,
    string Color,
    bool Hidden );

internal sealed class SvgChartRadialSeriesRenderer : ISvgChartSeriesRenderer
{
    #region Methods

    public bool CanRender( SvgChartPluginSeries series )
    {
        return series.Type is SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea;
    }

    public int GetRenderOrder( SvgChartPluginSeries series )
    {
        return SvgChartSeriesRenderHelpers.ResolveRenderOrder( series );
    }

    public void Render( SvgChartSeriesRendererContext context, IReadOnlyList<SvgChartPluginSeries> series, RenderTreeBuilder builder, ref int sequence )
    {
        var activeSeries = series.FirstOrDefault( x => !x.Hidden && CanRender( x ) );
        var renderSeries = series.Where( x => CanRender( x ) && context.ShouldRenderSeries( x ) ).ToList();

        if ( renderSeries.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-radial" );

        foreach ( var item in renderSeries )
        {
            var hidden = item.Hidden || !ReferenceEquals( item, activeSeries );

            context.RenderRetainedSeries<SvgChartRadialSeriesContent>( builder, ref sequence, item, "svg-chart-radial-series", hidden );
        }

        builder.CloseElement();
    }

    #endregion
}