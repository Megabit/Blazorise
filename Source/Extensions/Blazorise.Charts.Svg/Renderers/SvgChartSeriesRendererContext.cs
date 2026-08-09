#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartSeriesRendererContext
{
    #region Constructors

    public SvgChartSeriesRendererContext(
        SvgChartPluginRenderContext chart,
        SvgChartResolvedAnimation animation,
        IReadOnlyDictionary<string, SvgChartPointBounds> previousPointBounds,
        Dictionary<string, SvgChartPointBounds> currentPointBounds,
        IReadOnlyDictionary<string, string> previousPathValues,
        Dictionary<string, string> currentPathValues,
        Func<object, int, string> categoryFormatter )
    {
        Chart = chart;
        Animation = animation ?? new();
        this.previousPointBounds = previousPointBounds ?? new Dictionary<string, SvgChartPointBounds>();
        this.currentPointBounds = currentPointBounds ?? [];
        this.previousPathValues = previousPathValues ?? new Dictionary<string, string>();
        this.currentPathValues = currentPathValues ?? [];
        this.categoryFormatter = categoryFormatter;
    }

    #endregion

    #region Members

    private readonly IReadOnlyDictionary<string, SvgChartPointBounds> previousPointBounds;

    private readonly Dictionary<string, SvgChartPointBounds> currentPointBounds;

    private readonly IReadOnlyDictionary<string, string> previousPathValues;

    private readonly Dictionary<string, string> currentPathValues;

    private readonly Func<object, int, string> categoryFormatter;

    #endregion

    #region Methods

    public void AddAnimatedStyleAttribute( RenderTreeBuilder builder, ref int sequence, string style = null )
    {
        if ( !string.IsNullOrWhiteSpace( style ) )
        {
            builder.AddAttribute( sequence++, "style", style );
        }
    }

    public string TrackPointBounds( SvgChartPluginSeries series, int pointIndex, SvgChartPointBounds bounds )
    {
        var key = CreatePointKey( series, pointIndex );

        currentPointBounds[key] = bounds;

        return key;
    }

    public void RenderPathFadeAnimation( RenderTreeBuilder builder, ref int sequence, SvgChartPluginSeries series, string keySuffix, string pathValue, string opacity )
    {
        if ( !Animation.Opacity.Enabled )
            return;

        var key = $"{series.Type}|{series.Name}|{keySuffix}";

        if ( !ResolvePathAnimationFrom( key, pathValue, Animation.Opacity, out var from ) )
            return;

        RenderAttributeAnimation( builder, ref sequence, Animation.Opacity, "opacity", from, opacity );
    }

    public void RenderPointBoundsAttributeAnimation( RenderTreeBuilder builder, ref int sequence, string key, string attributeName, string initialFrom, string to, Func<SvgChartPointBounds, string> previousValue )
    {
        if ( !Animation.Geometry.Enabled || !IsGeometryTargetEnabled( attributeName ) )
            return;

        if ( !ResolvePointBoundsAnimationFrom( key, Animation.Geometry, initialFrom, previousValue, out var from ) )
            return;

        RenderAttributeAnimation( builder, ref sequence, Animation.Geometry, attributeName, from, to );
    }

    public void RenderInitialAttributeAnimation( RenderTreeBuilder builder, ref int sequence, string attributeName, string from, string to )
    {
        var target = ResolveAnimationTarget( attributeName );

        if ( target is null || !target.Enabled || !target.AnimateInitial )
            return;

        RenderAttributeAnimation( builder, ref sequence, target, attributeName, from, to );
    }

    private void RenderAttributeAnimation( RenderTreeBuilder builder, ref int sequence, SvgChartResolvedAnimationTarget target, string attributeName, string from, string to )
    {
        if ( string.Equals( from, to, StringComparison.Ordinal ) )
            return;

        var name = attributeName.ToLowerInvariant();

        if ( Animation.InitialRender && target.AnimateInitial )
            builder.AddAttribute( sequence++, "data-svg-chart-animation-initial", "true" );

        builder.AddAttribute( sequence++, $"data-svg-chart-animation-{name}", "true" );
        builder.AddAttribute( sequence++, $"data-svg-chart-animation-{name}-from", from );
        builder.AddAttribute( sequence++, $"data-svg-chart-animation-{name}-to", to );
        builder.AddAttribute( sequence++, $"data-svg-chart-animation-{name}-duration", target.Duration );
        builder.AddAttribute( sequence++, $"data-svg-chart-animation-{name}-delay", target.Delay );
        builder.AddAttribute( sequence++, $"data-svg-chart-animation-{name}-key-splines", target.KeySplines );
        builder.AddAttribute( sequence++, $"data-svg-chart-animation-{name}-version", Animation.Version );
    }

    private bool ResolvePointBoundsAnimationFrom( string key, SvgChartResolvedAnimationTarget target, string initialFrom, Func<SvgChartPointBounds, string> previousValue, out string from )
    {
        from = null;

        if ( target.AnimateUpdates && previousPointBounds.TryGetValue( key, out var previousBounds ) )
        {
            from = previousValue( previousBounds );

            return true;
        }

        if ( target.AnimateInitial )
        {
            from = initialFrom;

            return true;
        }

        return false;
    }

    private bool ResolvePathAnimationFrom( string key, string pathValue, SvgChartResolvedOpacityAnimation target, out string from )
    {
        from = null;

        currentPathValues[key] = pathValue;

        if ( !Animation.InitialRender
             && target.AnimateUpdates
             && ( !previousPathValues.TryGetValue( key, out var previousPathValue )
                  || !string.Equals( previousPathValue, pathValue, StringComparison.Ordinal ) ) )
        {
            from = target.From;

            return true;
        }

        if ( target.AnimateInitial )
        {
            from = target.From;

            return true;
        }

        return false;
    }

    public void AddPointInteractionAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartPointEventArgs point, string color )
    {
        builder.AddAttribute( sequence++, "tabindex", "0" );
        builder.AddAttribute( sequence++, "role", "img" );
        builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
        builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( Chart.EventReceiver, () => Chart.NotifyPointClicked( point, color ) ) );
        builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( Chart.EventReceiver, () => Chart.NotifyPointHovered( point, color ) ) );
        builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( Chart.EventReceiver, Chart.NotifyPointLeft ) );
        builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( Chart.EventReceiver, () => Chart.ShowTooltip( point, color, false ) ) );
        builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( Chart.EventReceiver, Chart.NotifyPointLeft ) );
    }

    public string GetPointLabel( SvgChartPointEventArgs point )
    {
        var category = categoryFormatter?.Invoke( point.Category, point.PointIndex ) ?? point.Category?.ToString();

        return $"{category}, {point.Value}. {point.SeriesName}.";
    }

    public string ResolveColor( int index )
    {
        return SvgChartRenderHelpers.ResolveColor( null, index );
    }

    private static string CreatePointKey( SvgChartPluginSeries series, int pointIndex )
    {
        return $"{series.Type}|{series.Name}|{pointIndex}";
    }

    private static bool IsOpacityAttribute( string attributeName )
    {
        return string.Equals( attributeName, "opacity", StringComparison.OrdinalIgnoreCase );
    }

    private static bool IsGeometryAttribute( string attributeName )
    {
        return attributeName is "x" or "y" or "width" or "height" or "cx" or "cy" or "r";
    }

    private bool IsGeometryTargetEnabled( string attributeName )
    {
        if ( IsPositionAttribute( attributeName ) )
            return Animation.Geometry.AnimatePosition;

        if ( IsSizeAttribute( attributeName ) )
            return Animation.Geometry.AnimateSize;

        return true;
    }

    private SvgChartResolvedAnimationTarget ResolveAnimationTarget( string attributeName )
    {
        if ( IsOpacityAttribute( attributeName ) )
            return Animation.Opacity;

        if ( IsGeometryAttribute( attributeName ) && IsGeometryTargetEnabled( attributeName ) )
            return Animation.Geometry;

        return null;
    }

    private static bool IsPositionAttribute( string attributeName )
    {
        return attributeName is "x" or "y" or "cx" or "cy";
    }

    private static bool IsSizeAttribute( string attributeName )
    {
        return attributeName is "width" or "height" or "r";
    }

    #endregion

    #region Properties

    public SvgChartPluginRenderContext Chart { get; }

    public SvgChartResolvedAnimation Animation { get; }

    #endregion
}