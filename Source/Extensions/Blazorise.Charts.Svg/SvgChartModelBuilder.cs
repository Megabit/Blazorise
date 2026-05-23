#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using static Blazorise.Charts.Svg.SvgChartGeometry;
using static Blazorise.Charts.Svg.SvgChartOptionsMapper;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartModelBuilder<TItem>
{
    #region Members

    private readonly SvgChartType Type;

    private readonly IEnumerable<TItem> Items;

    private readonly SvgChartData<double?> Data;

    private readonly SvgChartOptions Options;

    private readonly SvgChartStreamingOptions Streaming;

    private readonly SvgChartViewport internalViewport;

    private readonly SvgChartData<double?> internalChartData;

    private readonly IReadOnlyList<object> seriesComponents;

    private readonly IReadOnlyList<object> categoryAxisComponents;

    private readonly IReadOnlyList<SvgChartValueAxis> valueAxisComponents;

    private readonly IReadOnlyList<ISvgChartPlugin> pluginComponents;

    private readonly IReadOnlyList<SvgChartTooltip> tooltipComponents;

    private readonly IReadOnlyCollection<string> hiddenSeries;

    #endregion

    #region Constructors

    public SvgChartModelBuilder(
        SvgChartType type,
        IEnumerable<TItem> items,
        SvgChartData<double?> data,
        SvgChartOptions options,
        SvgChartStreamingOptions streaming,
        SvgChartViewport internalViewport,
        SvgChartData<double?> internalChartData,
        IReadOnlyList<object> seriesComponents,
        IReadOnlyList<object> categoryAxisComponents,
        IReadOnlyList<SvgChartValueAxis> valueAxisComponents,
        IReadOnlyList<ISvgChartPlugin> pluginComponents,
        IReadOnlyList<SvgChartTooltip> tooltipComponents,
        IReadOnlyCollection<string> hiddenSeries )
    {
        Type = type;
        Items = items;
        Data = data;
        Options = options;
        Streaming = streaming;
        this.internalViewport = internalViewport;
        this.internalChartData = internalChartData;
        this.seriesComponents = seriesComponents;
        this.categoryAxisComponents = categoryAxisComponents;
        this.valueAxisComponents = valueAxisComponents;
        this.pluginComponents = pluginComponents;
        this.tooltipComponents = tooltipComponents;
        this.hiddenSeries = hiddenSeries;
    }

    #endregion

    #region Methods
    public SvgChartRenderModel Build( bool applyStreamingViewport = true, bool applyZoomViewport = true )
    {
        var chartData = internalChartData ?? Data;
        var options = ResolveOptions();
        var childSeries = seriesComponents.OfType<SvgChartSeries<TItem>>().ToList();
        var categoryAxes = categoryAxisComponents.OfType<SvgChartCategoryAxis<TItem>>().ToList();
        var categoryAxis = ResolveCategoryAxis( chartData, childSeries, categoryAxes );
        var valueAxisId = ResolveValueAxisId( chartData, childSeries );
        var items = Items?.ToList() ?? [];
        var labels = ResolveLabels( chartData, categoryAxis, items );
        var series = ResolveSeries( chartData, childSeries, items, labels.Count );
        var zoom = ResolveZoom( options );
        var viewport = applyZoomViewport ? ResolveViewport( zoom ) : null;
        var categorySlotCount = labels.Count;
        var categoryLabelIndexes = Enumerable.Range( 0, labels.Count ).ToList();
        var categoryAxisOptions = CreateCategoryAxisOptions( options.XAxis ?? new(), categoryAxis );
        if ( applyStreamingViewport )
            ApplyStreamingViewport( labels, series, ResolveStreaming(), out categorySlotCount, out categoryLabelIndexes );
        var categoryRange = ResolveCategoryRange( labels.Count, zoom, viewport );
        var valueAxes = ResolveValueAxes( options, series, zoom, viewport );
        var primaryValueAxis = ResolvePrimaryValueAxis( valueAxes, valueAxisId );

        return new SvgChartRenderModel
        {
            Type = Type,
            Options = options,
            Zoom = zoom,
            Viewport = viewport,
            Labels = labels,
            CategorySlotCount = categorySlotCount,
            CategoryLabelIndexes = categoryLabelIndexes,
            CategoryMin = categoryRange.Min,
            CategoryMax = categoryRange.Max,
            CategoryAxis = categoryAxisOptions,
            CategoryTickFormatter = ResolveCategoryTickFormatter( categoryAxis, categoryAxisOptions ),
            Series = series,
            Min = primaryValueAxis.Min,
            Max = primaryValueAxis.Max,
            Ticks = primaryValueAxis.Ticks,
            ValueAxes = valueAxes,
            PrimaryValueAxis = primaryValueAxis,
            Tooltip = ResolveTooltip( options )
        };
    }

    private static SvgChartCategoryAxis<TItem> ResolveCategoryAxis( SvgChartData<double?> chartData, List<SvgChartSeries<TItem>> childSeries, List<SvgChartCategoryAxis<TItem>> categoryAxes )
    {
        var categoryAxisId = childSeries.Select( x => x.CategoryAxisId )
            .Concat( chartData?.Series?.Select( x => x.CategoryAxisId ) ?? Enumerable.Empty<string>() )
            .FirstOrDefault( x => !string.IsNullOrWhiteSpace( x ) );

        if ( !string.IsNullOrWhiteSpace( categoryAxisId ) )
            return categoryAxes.LastOrDefault( x => string.Equals( x.Id, categoryAxisId, StringComparison.Ordinal ) ) ?? categoryAxes.LastOrDefault();

        return categoryAxes.LastOrDefault();
    }

    private static string ResolveValueAxisId( SvgChartData<double?> chartData, List<SvgChartSeries<TItem>> childSeries )
    {
        return childSeries.Select( x => x.ValueAxisId )
            .Concat( chartData?.Series?.Select( x => x.ValueAxisId ) ?? Enumerable.Empty<string>() )
            .FirstOrDefault( x => !string.IsNullOrWhiteSpace( x ) );
    }

    private List<object> ResolveLabels( SvgChartData<double?> chartData, SvgChartCategoryAxis<TItem> categoryAxis, List<TItem> items )
    {
        if ( chartData?.Labels?.Count > 0 )
            return chartData.Labels.ToList();

        if ( categoryAxis?.Labels?.Count > 0 )
            return categoryAxis.Labels.ToList();

        if ( categoryAxis is SvgChartTimeAxis<TItem> timeAxis && timeAxis.TimeValue is not null && items.Count > 0 )
            return items.Select( x => (object)timeAxis.TimeValue( x ) ).ToList();

        if ( categoryAxis?.Value is not null && items.Count > 0 )
            return items.Select( categoryAxis.Value ).ToList();

        var maxValues = seriesComponents.OfType<SvgChartSeries<TItem>>()
            .Select( x => x.Values?.Count ?? 0 )
            .DefaultIfEmpty( 0 )
            .Max();

        return Enumerable.Range( 1, maxValues ).Select( x => (object)x ).ToList();
    }

    private List<SvgChartRenderSeries> ResolveSeries( SvgChartData<double?> chartData, List<SvgChartSeries<TItem>> childSeries, List<TItem> items, int labelCount )
    {
        var series = new List<SvgChartRenderSeries>();

        if ( chartData?.Series?.Count > 0 )
        {
            for ( var i = 0; i < chartData.Series.Count; i++ )
            {
                var dataSeries = chartData.Series[i];
                var name = string.IsNullOrWhiteSpace( dataSeries.Name ) ? $"Series {i + 1}" : dataSeries.Name;
                var values = dataSeries.Values?.ToList() ?? [];
                var yValues = dataSeries.YValues?.Count > 0 ? dataSeries.YValues.ToList() : values.ToList();
                var xValues = dataSeries.XValues?.Count > 0
                    ? dataSeries.XValues.ToList()
                    : Enumerable.Range( 0, yValues.Count ).Select( x => (double?)x ).ToList();

                series.Add( new()
                {
                    Name = name,
                    Type = Type,
                    Values = values,
                    Color = dataSeries.Color,
                    RenderColor = SvgChartRenderHelpers.ResolveColor( dataSeries.Color, i ),
                    Hidden = dataSeries.Hidden || hiddenSeries.Contains( name ),
                    Order = dataSeries.Order,
                    CategoryAxisId = dataSeries.CategoryAxisId,
                    ValueAxisId = dataSeries.ValueAxisId,
                    Stack = dataSeries.Stack,
                    XValues = xValues,
                    YValues = yValues,
                    RadiusValues = dataSeries.RadiusValues?.ToList() ?? [],
                    BorderRadius = 3,
                    MarkerRadius = 3,
                    StrokeWidth = 2,
                    FillOpacity = 0.18
                } );
            }
        }

        for ( var i = 0; i < childSeries.Count; i++ )
        {
            var child = childSeries[i];
            var name = string.IsNullOrWhiteSpace( child.Name ) ? $"Series {series.Count + 1}" : child.Name;
            var values = child.Values?.ToList()
                ?? ( child.Value is null ? [] : items.Select( child.Value ).ToList() );
            var xValues = child.XValue is null
                ? Enumerable.Range( 0, values.Count ).Select( x => (double?)x ).ToList()
                : items.Select( child.XValue ).ToList();
            var yValues = child.YValue is null ? values.ToList() : items.Select( child.YValue ).ToList();
            var radiusValues = child.RadiusValue is null ? [] : items.Select( child.RadiusValue ).ToList();

            series.Add( new()
            {
                Name = name,
                Type = child.ChartType,
                Values = NormalizeValues( values, labelCount ),
                XValues = xValues,
                YValues = yValues,
                RadiusValues = radiusValues,
                Color = child.Color,
                RenderColor = SvgChartRenderHelpers.ResolveColor( child.Color, series.Count ),
                Hidden = child.Hidden || hiddenSeries.Contains( name ),
                Order = child.Order,
                CategoryAxisId = child.CategoryAxisId,
                ValueAxisId = child.ValueAxisId,
                Stack = child.Stack,
                BorderRadius = child switch
                {
                    SvgColumnSeries<TItem> columnSeries => columnSeries.BorderRadius,
                    SvgBarSeries<TItem> barSeries => barSeries.BorderRadius,
                    _ => 3
                },
                MarkerRadius = child switch
                {
                    SvgLineSeries<TItem> lineSeries => lineSeries.MarkerRadius,
                    SvgScatterSeries<TItem> scatterSeries => scatterSeries.MarkerRadius,
                    SvgBubbleSeries<TItem> bubbleSeries => bubbleSeries.Radius,
                    _ => 3
                },
                StrokeWidth = child switch
                {
                    SvgLineSeries<TItem> lineSeries => lineSeries.StrokeWidth,
                    SvgAreaSeries<TItem> areaSeries => areaSeries.StrokeWidth,
                    _ => 2
                },
                FillOpacity = child switch
                {
                    SvgAreaSeries<TItem> areaSeries => areaSeries.FillOpacity,
                    SvgRadarSeries<TItem> radarSeries => radarSeries.FillOpacity,
                    _ => 0.18
                }
            } );
        }

        ApplyStacking( series, ResolveStackedValueAxisIds( series ) );

        return series;
    }

    private HashSet<string> ResolveStackedValueAxisIds( List<SvgChartRenderSeries> series )
    {
        var axes = valueAxisComponents.Count == 0
            ? new List<SvgChartAxisOptions> { CreateValueAxisOptions( ResolveOptions().YAxis ?? new() ) }
            : valueAxisComponents.Select( CreateValueAxisOptions ).ToList();

        var defaultAxis = axes.Last();

        var result = axes.Where( x => x.Stacked )
            .Select( x => string.IsNullOrWhiteSpace( x.Id ) ? defaultAxis.Id ?? string.Empty : x.Id )
            .ToHashSet( StringComparer.Ordinal );

        if ( defaultAxis.Stacked )
        {
            result.Add( string.Empty );
            foreach ( var axisId in series.Select( x => x.ValueAxisId ).Where( x => !string.IsNullOrWhiteSpace( x ) ) )
                result.Add( axisId );
        }

        return result;
    }

    private static void ApplyStacking( List<SvgChartRenderSeries> series, HashSet<string> stackedAxisIds )
    {
        if ( stackedAxisIds.Count == 0 )
            return;

        foreach ( var group in series.Where( x => IsStackableChart( x.Type ) )
                     .GroupBy( x => new { AxisId = x.ValueAxisId ?? string.Empty, Stack = x.Stack ?? string.Empty, x.Type } ) )
        {
            if ( !stackedAxisIds.Contains( group.Key.AxisId ) )
                continue;

            var positives = new Dictionary<int, double>();
            var negatives = new Dictionary<int, double>();

            foreach ( var item in group )
            {
                if ( item.Hidden )
                {
                    item.StackBaseValues.AddRange( Enumerable.Repeat<double?>( null, item.Values.Count ) );
                    item.StackEndValues.AddRange( Enumerable.Repeat<double?>( null, item.Values.Count ) );
                    continue;
                }

                for ( var i = 0; i < item.Values.Count; i++ )
                {
                    var value = item.Values[i];

                    if ( !value.HasValue )
                    {
                        item.StackBaseValues.Add( null );
                        item.StackEndValues.Add( null );
                        continue;
                    }

                    var totals = value.Value >= 0 ? positives : negatives;
                    var start = totals.TryGetValue( i, out var current ) ? current : 0;
                    var end = start + value.Value;

                    item.StackBaseValues.Add( start );
                    item.StackEndValues.Add( end );
                    totals[i] = end;
                }
            }
        }
    }

    private static bool IsStackableChart( SvgChartType type )
    {
        return type is SvgChartType.Column or SvgChartType.Bar or SvgChartType.Area;
    }

    private static Func<SvgChartAxisTickContext, string> ResolveCategoryTickFormatter( SvgChartCategoryAxis<TItem> categoryAxis, SvgChartAxisOptions categoryAxisOptions )
    {
        if ( categoryAxis?.TickFormatter is not null )
            return categoryAxis.TickFormatter;

        if ( categoryAxis is SvgChartTimeAxis<TItem> timeAxis )
        {
            var culture = ResolveTimeCulture( timeAxis.Culture );

            return context =>
            {
                if ( context.Value is DateTime dateTime )
                    return dateTime.ToString( ResolveTimeFormat( timeAxis ), culture );

                if ( context.Value is DateTimeOffset dateTimeOffset )
                    return dateTimeOffset.ToString( ResolveTimeFormat( timeAxis ), culture );

                return context.Value?.ToString();
            };
        }

        return categoryAxisOptions.TickFormatter;
    }

    private static string ResolveTimeFormat( SvgChartTimeAxis<TItem> axis )
    {
        if ( !string.IsNullOrWhiteSpace( axis.Format ) )
            return axis.Format;

        return axis.Unit switch
        {
            SvgChartTimeUnit.Millisecond => "HH:mm:ss.fff",
            SvgChartTimeUnit.Second => "HH:mm:ss",
            SvgChartTimeUnit.Minute => "HH:mm",
            SvgChartTimeUnit.Hour => "HH:mm",
            SvgChartTimeUnit.Day => "MMM d",
            SvgChartTimeUnit.Month => "MMM yyyy",
            SvgChartTimeUnit.Year => "yyyy",
            _ => "g"
        };
    }

    private static System.Globalization.CultureInfo ResolveTimeCulture( string culture )
    {
        if ( string.IsNullOrWhiteSpace( culture ) )
            return System.Globalization.CultureInfo.CurrentCulture;

        try
        {
            return System.Globalization.CultureInfo.GetCultureInfo( culture );
        }
        catch ( System.Globalization.CultureNotFoundException )
        {
            return System.Globalization.CultureInfo.CurrentCulture;
        }
    }

    private static List<double?> NormalizeValues( List<double?> values, int labelCount )
    {
        if ( labelCount == 0 || values.Count >= labelCount )
            return values;

        var normalized = values.ToList();

        while ( normalized.Count < labelCount )
            normalized.Add( null );

        return normalized;
    }

    private static void ApplyStreamingViewport( List<object> labels, List<SvgChartRenderSeries> series, SvgChartStreamingOptions streaming, out int categorySlotCount, out List<int> categoryLabelIndexes )
    {
        categorySlotCount = labels.Count;
        categoryLabelIndexes = Enumerable.Range( 0, labels.Count ).ToList();

        if ( streaming is null || !streaming.Enabled || !streaming.VisibleDataPoints.HasValue )
            return;

        var visibleDataPoints = Math.Max( 1, streaming.VisibleDataPoints.Value );
        var renderedDataPoints = SvgChartStreamingResolver.IsAnimationEnabled( streaming ) ? visibleDataPoints + 1 : visibleDataPoints;
        var startIndex = Math.Max( 0, labels.Count - renderedDataPoints );
        var visibleLabels = labels.Skip( startIndex ).Take( renderedDataPoints ).ToList();
        var visibleLabelIndexes = Enumerable.Range( startIndex, visibleLabels.Count ).ToList();
        var padCount = renderedDataPoints - visibleLabels.Count;
        categorySlotCount = visibleDataPoints;

        var reverse = SvgChartStreamingResolver.IsReversed( streaming );

        if ( reverse )
        {
            visibleLabels.Reverse();
            visibleLabelIndexes.Reverse();
        }

        if ( padCount > 0 )
        {
            if ( !reverse )
            {
                visibleLabels.InsertRange( 0, Enumerable.Repeat<object>( null, padCount ) );
                visibleLabelIndexes.InsertRange( 0, Enumerable.Repeat( -1, padCount ) );
            }
            else
            {
                visibleLabels.AddRange( Enumerable.Repeat<object>( null, padCount ) );
                visibleLabelIndexes.AddRange( Enumerable.Repeat( -1, padCount ) );
            }
        }

        ReplaceList( labels, visibleLabels );
        categoryLabelIndexes = visibleLabelIndexes;

        foreach ( var item in series )
        {
            ApplyStreamingViewport( item.Values, startIndex, renderedDataPoints, padCount, reverse );
            ApplyStreamingViewport( item.XValues, startIndex, renderedDataPoints, padCount, reverse );
            ApplyStreamingViewport( item.YValues, startIndex, renderedDataPoints, padCount, reverse );
            ApplyStreamingViewport( item.RadiusValues, startIndex, renderedDataPoints, padCount, reverse );
            ApplyStreamingViewport( item.StackBaseValues, startIndex, renderedDataPoints, padCount, reverse );
            ApplyStreamingViewport( item.StackEndValues, startIndex, renderedDataPoints, padCount, reverse );
        }
    }

    private static void ApplyStreamingViewport( List<double?> values, int startIndex, int visibleDataPoints, int padCount, bool reverse )
    {
        if ( values is null )
            return;

        var visibleValues = values.Skip( startIndex ).Take( visibleDataPoints ).ToList();

        if ( reverse )
            visibleValues.Reverse();

        if ( padCount > 0 )
        {
            if ( !reverse )
                visibleValues.InsertRange( 0, Enumerable.Repeat<double?>( null, padCount ) );
            else
                visibleValues.AddRange( Enumerable.Repeat<double?>( null, padCount ) );
        }

        ReplaceList( values, visibleValues );
    }

    private static void ReplaceList<TValue>( List<TValue> values, List<TValue> replacement )
    {
        values.Clear();
        values.AddRange( replacement );
    }

    private SvgChartOptions ResolveOptions()
    {
        return Options ?? new();
    }

    private SvgChartStreamingOptions ResolveStreaming()
    {
        var streamingComponent = pluginComponents.OfType<SvgChartStreaming>().LastOrDefault();

        if ( streamingComponent is not null )
            return SvgChartStreamingResolver.Resolve( streamingComponent, Streaming ?? ResolveOptions().Streaming );

        return Streaming ?? ResolveOptions().Streaming ?? new() { Enabled = false };
    }

    private List<SvgChartRenderValueAxis> ResolveValueAxes( SvgChartOptions options, List<SvgChartRenderSeries> series, SvgChartZoomOptions zoom, SvgChartViewport viewport )
    {
        var axes = valueAxisComponents.Count == 0
            ? new List<SvgChartAxisOptions> { CreateValueAxisOptions( options.YAxis ?? new() ) }
            : valueAxisComponents.Select( CreateValueAxisOptions ).ToList();
        var referencedAxisIds = series.Select( x => x.ValueAxisId )
            .Where( x => !string.IsNullOrWhiteSpace( x ) )
            .Distinct( StringComparer.Ordinal )
            .ToList();

        foreach ( var axisId in referencedAxisIds )
        {
            if ( !axes.Any( x => string.Equals( x.Id, axisId, StringComparison.Ordinal ) ) )
            {
                var axis = CreateValueAxisOptions( options.YAxis ?? new() );
                axis.Id = axisId;
                axes.Add( axis );
            }
        }

        var defaultAxis = axes.Last();

        return axes.Select( axis =>
        {
            var values = series.Where( x => !x.Hidden && BelongsToAxis( x, axis, defaultAxis ) )
                .SelectMany( x => axis.Stacked && x.StackEndValues.Count > 0 ? x.StackEndValues : IsPointChart( x.Type ) ? x.YValues : x.Values )
                .Where( x => x.HasValue )
                .Select( x => x.Value )
                .ToList();
            var scale = BuildScale( values, ApplyValueAxisViewport( axis, zoom, viewport, series.Any( x => x.Type == SvgChartType.Bar ) ) );

            return new SvgChartRenderValueAxis
            {
                Id = axis.Id,
                Position = axis.Position,
                GridLines = axis.GridLines,
                TickFormatter = axis.TickFormatter,
                Stacked = axis.Stacked,
                Min = scale.Min,
                Max = scale.Max,
                Ticks = scale.Ticks
            };
        } ).ToList();
    }

    private static SvgChartRenderValueAxis ResolvePrimaryValueAxis( List<SvgChartRenderValueAxis> axes, string valueAxisId )
    {
        return !string.IsNullOrWhiteSpace( valueAxisId )
            ? axes.LastOrDefault( x => string.Equals( x.Id, valueAxisId, StringComparison.Ordinal ) ) ?? axes.Last()
            : axes.Last();
    }

    private static bool BelongsToAxis( SvgChartRenderSeries series, SvgChartAxisOptions axis, SvgChartAxisOptions defaultAxis )
    {
        if ( string.IsNullOrWhiteSpace( series.ValueAxisId ) )
            return ReferenceEquals( axis, defaultAxis );

        return string.Equals( series.ValueAxisId, axis.Id, StringComparison.Ordinal );
    }

    private SvgChartTooltipOptions ResolveTooltip( SvgChartOptions options )
    {
        var tooltipComponent = tooltipComponents.LastOrDefault();
        var tooltipOptions = options.Tooltip ?? new();

        if ( tooltipComponent is null )
            return tooltipOptions;

        return new()
        {
            Enabled = tooltipComponent.Enabled,
            InteractionMode = tooltipComponent.InteractionMode,
            Formatter = tooltipComponent.Formatter ?? tooltipOptions.Formatter,
            Template = tooltipComponent.Template ?? tooltipOptions.Template,
            Width = tooltipComponent.Width,
            Height = tooltipComponent.Height,
            OffsetX = tooltipComponent.OffsetX,
            OffsetY = tooltipComponent.OffsetY
        };
    }

    private SvgChartZoomOptions ResolveZoom( SvgChartOptions options )
    {
        var zoomComponent = pluginComponents.OfType<SvgChartZoom>().LastOrDefault();
        var zoomOptions = options.Zoom ?? new();

        if ( zoomComponent is null )
            return CreateZoomOptions( zoomOptions );

        return new()
        {
            Enabled = zoomComponent.Enabled,
            Mode = zoomComponent.Mode,
            Wheel = zoomComponent.Wheel,
            Pan = zoomComponent.Pan,
            MinZoom = zoomComponent.MinZoom,
            MaxZoom = zoomComponent.MaxZoom,
            Viewport = zoomComponent.Viewport ?? zoomOptions.Viewport
        };
    }

    private static SvgChartZoomOptions CreateZoomOptions( SvgChartZoomOptions options )
    {
        if ( options is null )
            return new();

        return new()
        {
            Enabled = options.Enabled,
            Mode = options.Mode,
            Wheel = options.Wheel,
            Pan = options.Pan,
            MinZoom = options.MinZoom,
            MaxZoom = options.MaxZoom,
            Viewport = CloneViewport( options.Viewport )
        };
    }

    private SvgChartViewport ResolveViewport( SvgChartZoomOptions zoom )
    {
        return CloneViewport( zoom?.Viewport ?? internalViewport );
    }
    #endregion
}