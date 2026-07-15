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

    private readonly int? rowLimit;

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
        IReadOnlyCollection<string> hiddenSeries,
        int? rowLimit )
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
        this.rowLimit = rowLimit;
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
        var items = LimitValues( Items );
        var labels = LimitValues( ResolveLabels( chartData, categoryAxis, items ) );
        var timeAxisValues = ResolveTimeAxisValues( categoryAxis, labels );
        var series = ResolveSeries( chartData, childSeries, items, labels.Count, timeAxisValues );
        var zoom = ResolveZoom( options );
        var viewport = applyZoomViewport ? ResolveViewport( zoom ) : null;
        var categorySlotCount = labels.Count;
        var categoryLabelIndexes = Enumerable.Range( 0, labels.Count ).ToList();
        var categoryAxisOptions = CreateCategoryAxisOptions( options.XAxis ?? new(), categoryAxis );
        if ( applyStreamingViewport )
            ApplyStreamingViewport( labels, series, ResolveStreaming(), out categorySlotCount, out categoryLabelIndexes );
        var categoryRange = ResolveCategoryRange( categorySlotCount > 0 ? categorySlotCount : labels.Count, zoom, viewport );
        var categoryScale = ResolveContinuousCategoryScale( categoryAxis, categoryAxisOptions, labels, series, zoom, viewport );
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
            CategoryScaleKind = categoryScale is null ? SvgChartAxisScaleKind.Ordinal : SvgChartAxisScaleKind.Continuous,
            CategoryScale = categoryScale,
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

    private List<SvgChartRenderSeries> ResolveSeries( SvgChartData<double?> chartData, List<SvgChartSeries<TItem>> childSeries, List<TItem> items, int labelCount, List<double?> categoryXValues )
    {
        var series = new List<SvgChartRenderSeries>();

        if ( chartData?.Series?.Count > 0 )
        {
            for ( var i = 0; i < chartData.Series.Count; i++ )
            {
                var dataSeries = chartData.Series[i];

                if ( dataSeries is null )
                    continue;

                var name = string.IsNullOrWhiteSpace( dataSeries.Name ) ? $"Series {i + 1}" : dataSeries.Name;
                var values = dataSeries.Values?.ToList() ?? [];
                var yValues = dataSeries.YValues?.Count > 0 ? dataSeries.YValues.ToList() : values.ToList();
                var xValues = dataSeries.XValues?.Count > 0
                    ? dataSeries.XValues.ToList()
                    : ( categoryXValues?.Count > 0
                        ? NormalizeValues( categoryXValues.ToList(), yValues.Count )
                        : Enumerable.Range( 0, yValues.Count ).Select( x => (double?)x ).ToList() );

                series.Add( new()
                {
                    Name = name,
                    Type = Type,
                    Values = values,
                    Color = dataSeries.Color,
                    RenderColor = SvgChartRenderHelpers.ResolveColor( dataSeries.Color, i ),
                    PointColors = ResolvePointColors( dataSeries.Colors, values.Count, dataSeries.Color, i, IsRadialChart( Type ) ),
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
                    FillOpacity = 0.18,
                    Interpolation = dataSeries.Interpolation,
                    Tension = dataSeries.Tension ?? 0.4
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
                ? ( categoryXValues?.Count > 0
                    ? NormalizeValues( categoryXValues.ToList(), values.Count )
                    : Enumerable.Range( 0, values.Count ).Select( x => (double?)x ).ToList() )
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
                PointColors = ResolvePointColors( child.Colors, child.PointColor is null ? null : items.Select( child.PointColor ).ToList(), labelCount, child.Color, series.Count, IsRadialChart( child.ChartType ) ),
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
                },
                Interpolation = child switch
                {
                    SvgLineSeries<TItem> lineSeries => lineSeries.Interpolation,
                    SvgAreaSeries<TItem> areaSeries => areaSeries.Interpolation,
                    _ => SvgChartInterpolationMode.Linear
                },
                Tension = child switch
                {
                    SvgLineSeries<TItem> lineSeries => lineSeries.Tension,
                    SvgAreaSeries<TItem> areaSeries => areaSeries.Tension,
                    _ => 0.4
                }
            } );
        }

        LimitSeries( series );
        ApplyStacking( series, ResolveStackedValueAxisIds( series ) );

        return series;
    }

    private List<TValue> LimitValues<TValue>( IEnumerable<TValue> values )
    {
        if ( values is null )
            return [];

        if ( !rowLimit.HasValue )
            return values.ToList();

        return values.Take( rowLimit.Value ).ToList();
    }

    private void LimitSeries( List<SvgChartRenderSeries> series )
    {
        if ( !rowLimit.HasValue )
            return;

        foreach ( var item in series )
        {
            LimitValues( item.Values, rowLimit.Value );
            LimitValues( item.XValues, rowLimit.Value );
            LimitValues( item.YValues, rowLimit.Value );
            LimitValues( item.RadiusValues, rowLimit.Value );
            LimitValues( item.PointColors, rowLimit.Value );
        }
    }

    private static void LimitValues<TValue>( List<TValue> values, int limit )
    {
        if ( values is not null && values.Count > limit )
            values.RemoveRange( limit, values.Count - limit );
    }

    private static List<string> ResolvePointColors( IReadOnlyList<Color> colors, int count, Color seriesColor, int seriesIndex, bool usePalettePerPoint )
    {
        return ResolvePointColors( colors, null, count, seriesColor, seriesIndex, usePalettePerPoint );
    }

    private static List<string> ResolvePointColors( IReadOnlyList<Color> colors, IReadOnlyList<Color> selectedColors, int count, Color seriesColor, int seriesIndex, bool usePalettePerPoint )
    {
        var result = new List<string>();
        var seriesFallback = SvgChartRenderHelpers.ResolveColor( seriesColor, seriesIndex );
        var palettePerPoint = usePalettePerPoint && SvgChartRenderHelpers.IsDefaultColor( seriesColor );

        for ( var i = 0; i < count; i++ )
        {
            var color = i < ( selectedColors?.Count ?? 0 )
                ? selectedColors[i]
                : i < ( colors?.Count ?? 0 )
                    ? colors[i]
                    : null;

            result.Add( SvgChartRenderHelpers.IsDefaultColor( color )
                ? ( palettePerPoint ? SvgChartRenderHelpers.ResolveColor( null, i ) : seriesFallback )
                : SvgChartRenderHelpers.ResolveColor( color, i ) );
        }

        return result;
    }

    private HashSet<string> ResolveStackedValueAxisIds( List<SvgChartRenderSeries> series )
    {
        var valueAxisOptions = ResolveOptions().YAxis ?? new();
        var axes = valueAxisComponents.Count == 0
            ? new List<SvgChartAxisOptions> { CreateValueAxisOptions( valueAxisOptions ) }
            : valueAxisComponents.Select( axis => axis.ResolveOptions( valueAxisOptions ) ).ToList();

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
                if ( timeAxis.Scale == SvgChartTimeScale.Continuous && context.Value is double doubleValue )
                    return DateTimeOffset.FromUnixTimeMilliseconds( Convert.ToInt64( doubleValue ) ).ToString( ResolveTimeFormat( timeAxis ), culture );

                if ( context.Value is DateTime dateTime )
                    return dateTime.ToString( ResolveTimeFormat( timeAxis ), culture );

                if ( context.Value is DateTimeOffset dateTimeOffset )
                    return dateTimeOffset.ToString( ResolveTimeFormat( timeAxis ), culture );

                return context.Value?.ToString();
            };
        }

        return categoryAxisOptions.TickFormatter;
    }

    private static List<double?> ResolveTimeAxisValues( SvgChartCategoryAxis<TItem> categoryAxis, List<object> labels )
    {
        if ( categoryAxis is not SvgChartTimeAxis<TItem> { Scale: SvgChartTimeScale.Continuous } || labels.Count == 0 )
            return null;

        return labels.Select( ToUnixMilliseconds ).ToList();
    }

    private static SvgChartScale ResolveContinuousCategoryScale( SvgChartCategoryAxis<TItem> categoryAxis, SvgChartAxisOptions categoryAxisOptions, List<object> labels, List<SvgChartRenderSeries> series, SvgChartZoomOptions zoom, SvgChartViewport viewport )
    {
        if ( categoryAxis is not SvgChartTimeAxis<TItem> { Scale: SvgChartTimeScale.Continuous } timeAxis )
            return null;

        var visibleSeries = series.Where( x => !x.Hidden && !SvgChartGeometry.IsRadialChart( x.Type ) ).ToList();

        if ( visibleSeries.Count == 0 || visibleSeries.Any( x => x.Type is not ( SvgChartType.Line or SvgChartType.Area or SvgChartType.Scatter or SvgChartType.Bubble ) ) )
            return null;

        var values = visibleSeries
            .SelectMany( x => x.XValues )
            .Concat( labels.Select( ToUnixMilliseconds ) )
            .Where( x => x.HasValue )
            .Select( x => x.Value )
            .ToList();

        if ( values.Count == 0 )
            return null;

        var axis = SvgChartGeometry.ApplyPointXAxisViewport( categoryAxisOptions, zoom, viewport );
        axis.BeginAtZero = false;
        var range = SvgChartGeometry.BuildScale( values, axis );

        return BuildTimeScale( range.Min, range.Max, Math.Max( 2, axis.TickCount ), timeAxis );
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

    private static SvgChartScale BuildTimeScale( double min, double max, int tickCount, SvgChartTimeAxis<TItem> timeAxis )
    {
        var minDate = DateTimeOffset.FromUnixTimeMilliseconds( Convert.ToInt64( min ) );
        var maxDate = DateTimeOffset.FromUnixTimeMilliseconds( Convert.ToInt64( max ) );
        var unit = ResolveTimeUnit( timeAxis, maxDate - minDate );
        var step = ResolveTimeStep( unit, maxDate - minDate, tickCount );
        var current = FloorTime( minDate, unit, step );
        var ticks = new List<double>();

        while ( current < minDate )
            current = AddTimeUnit( current, unit, step );

        for ( var i = 0; current <= maxDate && i < tickCount * 4; i++ )
        {
            ticks.Add( current.ToUnixTimeMilliseconds() );
            current = AddTimeUnit( current, unit, step );
        }

        if ( ticks.Count == 0 || ticks[0] > min )
            ticks.Insert( 0, min );

        if ( ticks[^1] < max )
            ticks.Add( max );

        return new()
        {
            Min = min,
            Max = max,
            Ticks = ticks
        };
    }

    private static SvgChartTimeUnit ResolveTimeUnit( SvgChartTimeAxis<TItem> axis, TimeSpan range )
    {
        if ( axis.Unit != SvgChartTimeUnit.Auto )
            return axis.Unit;

        if ( range <= TimeSpan.FromMinutes( 2 ) )
            return SvgChartTimeUnit.Second;

        if ( range <= TimeSpan.FromHours( 2 ) )
            return SvgChartTimeUnit.Minute;

        if ( range <= TimeSpan.FromDays( 2 ) )
            return SvgChartTimeUnit.Hour;

        if ( range <= TimeSpan.FromDays( 120 ) )
            return SvgChartTimeUnit.Day;

        if ( range <= TimeSpan.FromDays( 730 ) )
            return SvgChartTimeUnit.Month;

        return SvgChartTimeUnit.Year;
    }

    private static int ResolveTimeStep( SvgChartTimeUnit unit, TimeSpan range, int tickCount )
    {
        var intervals = Math.Max( 1, tickCount - 1 );
        var rawStep = unit switch
        {
            SvgChartTimeUnit.Millisecond => range.TotalMilliseconds / intervals,
            SvgChartTimeUnit.Second => range.TotalSeconds / intervals,
            SvgChartTimeUnit.Minute => range.TotalMinutes / intervals,
            SvgChartTimeUnit.Hour => range.TotalHours / intervals,
            SvgChartTimeUnit.Day => range.TotalDays / intervals,
            SvgChartTimeUnit.Month => range.TotalDays / 30 / intervals,
            SvgChartTimeUnit.Year => range.TotalDays / 365 / intervals,
            _ => range.TotalMinutes / intervals
        };

        var steps = unit switch
        {
            SvgChartTimeUnit.Millisecond => new[] { 1, 2, 5, 10, 25, 50, 100, 250, 500 },
            SvgChartTimeUnit.Second or SvgChartTimeUnit.Minute => new[] { 1, 2, 5, 10, 15, 30 },
            SvgChartTimeUnit.Hour => new[] { 1, 2, 3, 6, 12 },
            SvgChartTimeUnit.Day => new[] { 1, 2, 7, 14 },
            SvgChartTimeUnit.Month => new[] { 1, 2, 3, 6 },
            SvgChartTimeUnit.Year => new[] { 1, 2, 5, 10 },
            _ => new[] { 1, 2, 5, 10 }
        };

        var step = steps.FirstOrDefault( x => x >= rawStep );

        return step == 0 ? steps[^1] : step;
    }

    private static DateTimeOffset FloorTime( DateTimeOffset value, SvgChartTimeUnit unit, int step )
    {
        return unit switch
        {
            SvgChartTimeUnit.Millisecond => value.AddMilliseconds( -( value.Millisecond % step ) ),
            SvgChartTimeUnit.Second => new( value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second - value.Second % step, value.Offset ),
            SvgChartTimeUnit.Minute => new( value.Year, value.Month, value.Day, value.Hour, value.Minute - value.Minute % step, 0, value.Offset ),
            SvgChartTimeUnit.Hour => new( value.Year, value.Month, value.Day, value.Hour - value.Hour % step, 0, 0, value.Offset ),
            SvgChartTimeUnit.Day => new( value.Year, value.Month, Math.Max( 1, value.Day - ( value.Day - 1 ) % step ), 0, 0, 0, value.Offset ),
            SvgChartTimeUnit.Month => new( value.Year, Math.Max( 1, value.Month - ( value.Month - 1 ) % step ), 1, 0, 0, 0, value.Offset ),
            SvgChartTimeUnit.Year => new( Math.Max( 1, value.Year - ( value.Year - 1 ) % step ), 1, 1, 0, 0, 0, value.Offset ),
            _ => value
        };
    }

    private static DateTimeOffset AddTimeUnit( DateTimeOffset value, SvgChartTimeUnit unit, int step )
    {
        return unit switch
        {
            SvgChartTimeUnit.Millisecond => value.AddMilliseconds( step ),
            SvgChartTimeUnit.Second => value.AddSeconds( step ),
            SvgChartTimeUnit.Minute => value.AddMinutes( step ),
            SvgChartTimeUnit.Hour => value.AddHours( step ),
            SvgChartTimeUnit.Day => value.AddDays( step ),
            SvgChartTimeUnit.Month => value.AddMonths( step ),
            SvgChartTimeUnit.Year => value.AddYears( step ),
            _ => value.AddMinutes( step )
        };
    }

    private static double? ToUnixMilliseconds( object value )
    {
        return value switch
        {
            DateTime dateTime => new DateTimeOffset( dateTime ).ToUnixTimeMilliseconds(),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToUnixTimeMilliseconds(),
            _ => null
        };
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
            var paddingLabelIndexes = Enumerable.Range( startIndex - padCount, padCount ).ToList();

            if ( !reverse )
            {
                visibleLabels.InsertRange( 0, Enumerable.Repeat<object>( null, padCount ) );
                visibleLabelIndexes.InsertRange( 0, paddingLabelIndexes );
            }
            else
            {
                visibleLabels.AddRange( Enumerable.Repeat<object>( null, padCount ) );
                paddingLabelIndexes.Reverse();
                visibleLabelIndexes.AddRange( paddingLabelIndexes );
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
        var valueAxisOptions = options.YAxis ?? new();
        var axes = valueAxisComponents.Count == 0
            ? new List<SvgChartAxisOptions> { CreateValueAxisOptions( valueAxisOptions ) }
            : valueAxisComponents.Select( axis => axis.ResolveOptions( valueAxisOptions ) ).ToList();
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
                Labels = axis.Labels,
                Title = axis.Title,
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

        return tooltipComponent?.ResolveOptions( tooltipOptions ) ?? tooltipOptions;
    }

    private SvgChartZoomOptions ResolveZoom( SvgChartOptions options )
    {
        var zoomComponent = pluginComponents.OfType<SvgChartZoom>().LastOrDefault();
        var zoomOptions = options.Zoom ?? new();

        if ( zoomComponent is null )
            return CreateZoomOptions( zoomOptions );

        return zoomComponent.ResolveOptions( zoomOptions );
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