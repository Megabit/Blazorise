#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

internal abstract class SvgChartSeriesContentBase : ComponentBase
{
    #region Members

    private bool shouldRender;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected sealed override void OnParametersSet()
    {
        shouldRender = !Hidden
            && Context is not null
            && Series is not null
            && UpdateRenderState();
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
    {
        return shouldRender;
    }

    protected abstract bool UpdateRenderState();

    #endregion

    #region Properties

    [Parameter] public SvgChartSeriesRendererContext Context { get; set; }

    [Parameter] public SvgChartPluginSeries Series { get; set; }

    [Parameter] public bool Hidden { get; set; }

    #endregion
}

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
        Dictionary<(int SeriesIndex, string SeriesName, int PointIndex), SvgChartPointInteraction> pointInteractions,
        HashSet<(int SeriesIndex, SvgChartType Type, string Name)> materializedSeries,
        bool passThroughSeriesPaths,
        object categoryFormatterKey,
        Func<object, int, string> categoryFormatter )
    {
        Chart = chart;
        Animation = animation ?? new();
        this.previousPointBounds = previousPointBounds ?? new Dictionary<string, SvgChartPointBounds>();
        this.currentPointBounds = currentPointBounds ?? [];
        this.previousPathValues = previousPathValues ?? new Dictionary<string, string>();
        this.currentPathValues = currentPathValues ?? [];
        this.pointInteractions = pointInteractions ?? [];
        this.materializedSeries = materializedSeries ?? [];
        PassThroughSeriesPaths = passThroughSeriesPaths;
        CategoryFormatterKey = categoryFormatterKey;
        this.categoryFormatter = categoryFormatter;
    }

    #endregion

    #region Members

    private readonly IReadOnlyDictionary<string, SvgChartPointBounds> previousPointBounds;

    private readonly Dictionary<string, SvgChartPointBounds> currentPointBounds;

    private readonly IReadOnlyDictionary<string, string> previousPathValues;

    private readonly Dictionary<string, string> currentPathValues;

    private readonly Dictionary<(int SeriesIndex, string SeriesName, int PointIndex), SvgChartPointInteraction> pointInteractions;

    private readonly HashSet<(int SeriesIndex, SvgChartType Type, string Name)> materializedSeries;

    private readonly Func<object, int, string> categoryFormatter;

    #endregion

    #region Methods

    public string TrackPointBounds( SvgChartPluginSeries series, int pointIndex, SvgChartPointBounds bounds )
    {
        if ( !Animation.Geometry.Enabled )
            return string.Empty;

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

        if ( !Animation.InitialRender
             && target.AnimateUpdates
             && previousPointBounds.TryGetValue( key, out var previousBounds ) )
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

    public void AddPointInteractionAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartPointEventArgs point, string color, string label = null )
    {
        var interaction = UpdatePointInteraction( point, color );

        builder.AddAttribute( sequence++, "tabindex", "0" );
        builder.AddAttribute( sequence++, "role", "img" );
        builder.AddAttribute( sequence++, "aria-label", label ?? GetPointLabel( point ) );
        builder.AddAttribute( sequence++, "onclick", interaction.Clicked );
        builder.AddAttribute( sequence++, "onmouseenter", interaction.MouseEntered );
        builder.AddAttribute( sequence++, "onmouseleave", interaction.MouseLeft );
        builder.AddAttribute( sequence++, "onfocus", interaction.Focused );
        builder.AddAttribute( sequence++, "onblur", interaction.Blurred );
    }

    public SvgChartPointInteraction UpdatePointInteraction( SvgChartPointEventArgs point, string color )
    {
        var interactionKey = (point.SeriesIndex, point.SeriesName, point.PointIndex);

        if ( !pointInteractions.TryGetValue( interactionKey, out var interaction ) )
        {
            interaction = new( Chart );
            pointInteractions.Add( interactionKey, interaction );
        }

        interaction.Update( point, color );

        return interaction;
    }

    public bool ShouldRenderSeries( SvgChartPluginSeries series )
    {
        var key = GetSeriesKey( series );

        if ( !series.Hidden )
            materializedSeries.Add( key );

        return !series.Hidden || materializedSeries.Contains( key );
    }

    public void RenderRetainedSeries<TContent>( RenderTreeBuilder builder, ref int sequence, SvgChartPluginSeries series, string className, bool hidden )
        where TContent : SvgChartSeriesContentBase
    {
        var seriesKey = GetSeriesKey( series );

        builder.OpenElement( sequence++, "g" );
        builder.SetKey( seriesKey );
        builder.AddAttribute( sequence++, "class", className );
        var visibilitySequence = sequence++;
        var ariaHiddenSequence = sequence++;

        if ( hidden )
        {
            builder.AddAttribute( visibilitySequence, "visibility", "hidden" );
            builder.AddAttribute( ariaHiddenSequence, "aria-hidden", "true" );
        }

        builder.OpenComponent<TContent>( sequence++ );
        builder.AddAttribute( sequence++, nameof( SvgChartSeriesContentBase.Context ), this );
        builder.AddAttribute( sequence++, nameof( SvgChartSeriesContentBase.Series ), series );
        builder.AddAttribute( sequence++, nameof( SvgChartSeriesContentBase.Hidden ), hidden );
        builder.CloseComponent();

        builder.CloseElement();
    }

    public (int SeriesIndex, SvgChartType Type, string Name) GetSeriesKey( SvgChartPluginSeries series )
    {
        for ( var seriesIndex = 0; seriesIndex < Chart.Series.Count; seriesIndex++ )
        {
            if ( ReferenceEquals( Chart.Series[seriesIndex], series ) )
                return (seriesIndex, series.Type, series.Name);
        }

        return (-1, series.Type, series.Name);
    }

    public string GetPointLabel( SvgChartPointEventArgs point )
    {
        return GetPointLabel( point.Category, point.Value, point.SeriesName, point.PointIndex );
    }

    public string GetPointLabel( object category, object value, string seriesName, int pointIndex )
    {
        var categoryString = categoryFormatter?.Invoke( category, pointIndex ) ?? category?.ToString();

        return $"{categoryString}, {value}. {seriesName}.";
    }

    public SvgChartSeriesProjectionState GetProjectionState( SvgChartPluginSeries series, bool useValueCategoryProjection )
    {
        var chart = Chart;
        var categoryStart = useValueCategoryProjection || chart.ContinuousCategoryAxis
            ? chart.ProjectX( 0, series.CategoryAxisId )
            : chart.ProjectCategory( 0, series.CategoryAxisId );
        var categoryEnd = useValueCategoryProjection || chart.ContinuousCategoryAxis
            ? chart.ProjectX( 1, series.CategoryAxisId )
            : chart.ProjectCategory( 1, series.CategoryAxisId );

        return new(
            chart.ContinuousCategoryAxis,
            categoryStart,
            categoryEnd,
            chart.ProjectY( 0, series.ValueAxisId ),
            chart.ProjectY( 1, series.ValueAxisId ),
            CategoryFormatterKey );
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

    public bool PassThroughSeriesPaths { get; }

    public object CategoryFormatterKey { get; }

    #endregion
}

internal readonly record struct SvgChartSeriesProjectionState(
    bool ContinuousCategoryAxis,
    double CategoryStart,
    double CategoryEnd,
    double ValueStart,
    double ValueEnd,
    object CategoryFormatterKey )
{
    public bool CanTransformFrom( SvgChartSeriesProjectionState source )
    {
        return ContinuousCategoryAxis == source.ContinuousCategoryAxis
            && Equals( CategoryFormatterKey, source.CategoryFormatterKey );
    }
}

internal readonly record struct SvgChartSeriesProjectionTransform(
    double ScaleX,
    double ScaleY,
    double TranslateX,
    double TranslateY )
{
    private const double MinimumScale = 0.0000001;

    public const string MarkerTransformStyleString = "transform-box:fill-box;transform-origin:center;transform:scale(var(--svg-chart-marker-scale-x,1),var(--svg-chart-marker-scale-y,1));";

    public static SvgChartSeriesProjectionTransform Identity { get; } = new( 1, 1, 0, 0 );

    public static SvgChartSeriesProjectionTransform Create( SvgChartSeriesProjectionState source, SvgChartSeriesProjectionState target )
    {
        var scaleX = ResolveScale( source.CategoryStart, source.CategoryEnd, target.CategoryStart, target.CategoryEnd );
        var scaleY = ResolveScale( source.ValueStart, source.ValueEnd, target.ValueStart, target.ValueEnd );

        return new(
            scaleX,
            scaleY,
            target.CategoryStart - scaleX * source.CategoryStart,
            target.ValueStart - scaleY * source.ValueStart );
    }

    public double ProjectX( double value )
    {
        return ScaleX * value + TranslateX;
    }

    public double ProjectY( double value )
    {
        return ScaleY * value + TranslateY;
    }

    public string ToTransformString()
    {
        return $"matrix({SvgChartRenderHelpers.Format( ScaleX )} 0 0 {SvgChartRenderHelpers.Format( ScaleY )} {SvgChartRenderHelpers.Format( TranslateX )} {SvgChartRenderHelpers.Format( TranslateY )})";
    }

    public string ToMarkerScaleStyleString()
    {
        return $"--svg-chart-marker-scale-x:{SvgChartRenderHelpers.Format( 1 / ScaleX )};--svg-chart-marker-scale-y:{SvgChartRenderHelpers.Format( 1 / ScaleY )};";
    }

    public bool IsIdentity => ScaleX == 1
        && ScaleY == 1
        && TranslateX == 0
        && TranslateY == 0;

    private static double ResolveScale( double sourceStart, double sourceEnd, double targetStart, double targetEnd )
    {
        var sourceSize = sourceEnd - sourceStart;

        if ( Math.Abs( sourceSize ) < MinimumScale )
            return 1;

        var scale = ( targetEnd - targetStart ) / sourceSize;

        return Math.Abs( scale ) < MinimumScale ? 1 : scale;
    }
}