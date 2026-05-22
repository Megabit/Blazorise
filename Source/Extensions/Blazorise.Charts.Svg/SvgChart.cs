#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgChart<TItem> : SvgChartBase
{
    #region Members

    private readonly List<object> seriesComponents = [];

    private readonly List<object> categoryAxisComponents = [];

    private readonly List<SvgChartValueAxis> valueAxisComponents = [];

    private readonly List<SvgChartTitle> titleComponents = [];

    private readonly List<SvgChartLegend> legendComponents = [];

    private readonly List<SvgChartTooltip> tooltipComponents = [];

    private readonly List<ISvgChartPlugin> pluginComponents = [];

    private readonly HashSet<string> hiddenSeries = [];

    private readonly HashSet<string> hiddenDataPoints = [];

    private SvgChartData<double?> internalChartData;

    private SvgChartTooltipContext activeTooltip;

    private ComponentParameterInfo<SvgChartType> paramType;

    private ComponentParameterInfo<IEnumerable<TItem>> paramItems;

    private ComponentParameterInfo<SvgChartData<double?>> paramData;

    private ComponentParameterInfo<SvgChartOptions> paramOptions;

    private ComponentParameterInfo<SvgChartStreamingOptions> paramStreaming;

    private bool activeTooltipPinned;

    private bool streamingPaused;

    private int streamingAnimationVersion;

    private bool streamingAnimationActive;

    private bool panning;

    private double panStartClientX;

    private double panStartClientY;

    private SvgChartViewport panStartViewport;

    private SvgChartViewport panLastViewport;

    private SvgChartViewport internalViewport;

    private IJSObjectReference jsModule;

    private bool zoomWheelInitialized;

    private DateTimeOffset lastStreamingRender = DateTimeOffset.MinValue;

    private static readonly string[] Palette =
    [
        "#4c6ef5",
        "#12b886",
        "#f59f00",
        "#e03131",
        "#845ef7",
        "#228be6"
    ];

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "svg-chart" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Type, out paramType );
        parameters.TryGetParameter( Items, out paramItems );
        parameters.TryGetParameter( Data, out paramData );
        parameters.TryGetParameter( Options, out paramOptions );
        parameters.TryGetParameter( Streaming, out paramStreaming );

        if ( paramType.Changed || paramItems.Changed || paramData.Changed )
            activeTooltip = null;

        if ( paramData.Changed )
            internalChartData = null;

        return base.SetParametersAsync( parameters );
    }

    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        var sequence = 0;
        var model = BuildModel();
        var options = ResolveOptions();
        var legend = ResolveLegend( options );
        var title = ResolveTitleOptions( options );
        var subtitle = ResolveSubtitleOptions( options );
        var hasTopLegend = legend.Visible && legend.Position == SvgChartLegendPosition.Top;
        var hasBottomLegend = legend.Visible && legend.Position == SvgChartLegendPosition.Bottom;
        var plot = BuildPlotArea( options, title, subtitle, hasTopLegend, hasBottomLegend );
        var streamingAnimation = ResolveStreamingAnimation( model, plot );
        var pluginContext = CreatePluginRenderContext( model, plot );
        var zoom = model.Zoom;

        builder.OpenComponent<CascadingValue<SvgChartBase>>( sequence++ );
        builder.AddAttribute( sequence++, "Value", this );
        builder.AddAttribute( sequence++, "IsFixed", true );
        builder.AddAttribute( sequence++, "ChildContent", ChildContent );
        builder.CloseComponent();

        builder.OpenElement( sequence++, "div" );
        builder.AddMultipleAttributes( sequence++, Attributes );
        builder.AddAttribute( sequence++, "id", ElementId );
        builder.AddAttribute( sequence++, "class", ClassNames );
        builder.AddAttribute( sequence++, "style", StyleNames );
        builder.AddElementReferenceCapture( sequence++, elementRef => ElementRef = elementRef );

        builder.OpenElement( sequence++, "svg" );
        builder.AddAttribute( sequence++, "xmlns", "http://www.w3.org/2000/svg" );
        builder.AddAttribute( sequence++, "class", zoom?.Enabled == true && zoom.Pan ? "svg-chart-surface svg-chart-pannable" : "svg-chart-surface" );
        builder.AddAttribute( sequence++, "draggable", "false" );
        builder.AddAttribute( sequence++, "role", "img" );
        builder.AddAttribute( sequence++, "aria-label", IsTextVisible( title ) ? title.Text : "SVG chart" );
        builder.AddAttribute( sequence++, "viewBox", $"0 0 {Format( options.Width )} {Format( options.Height )}" );
        builder.AddAttribute( sequence++, "style", ResolveSvgStyle( options, zoom, panning ) );

        if ( zoom.Enabled && !IsRadialChart( model ) )
        {
            if ( zoom.Wheel )
            {
                builder.AddAttribute( sequence++, "onwheel", EventCallback.Factory.Create<WheelEventArgs>( this, HandleChartWheel ) );
                builder.AddAttribute( sequence++, "onwheel:preventDefault", true );
                builder.AddAttribute( sequence++, "onwheel:stopPropagation", true );
            }

            if ( zoom.Pan )
            {
                builder.AddAttribute( sequence++, "onmousedown", EventCallback.Factory.Create<MouseEventArgs>( this, HandleChartMouseDown ) );
                builder.AddAttribute( sequence++, "onmousemove", EventCallback.Factory.Create<MouseEventArgs>( this, HandleChartMouseMove ) );
                builder.AddAttribute( sequence++, "onmouseup", EventCallback.Factory.Create<MouseEventArgs>( this, HandleChartMouseUp ) );
                builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, HandleChartMouseLeave ) );
                builder.AddAttribute( sequence++, "onmousedown:preventDefault", true );
                builder.AddAttribute( sequence++, "onmousemove:preventDefault", true );
            }
        }

        AddFontFamilyAttribute( builder, ref sequence, options.Font?.Family );

        RenderFocusStyles( builder, ref sequence );
        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.Background );

        RenderChartText( builder, ref sequence, options, title, subtitle );

        if ( hasTopLegend )
            RenderLegend( builder, ref sequence, model, options, 48 );

        if ( IsBarChart( model ) )
            RenderHorizontalGridAndAxes( builder, ref sequence, model, plot );
        else if ( !IsRadialChart( model ) )
            RenderGridAndAxes( builder, ref sequence, model, plot, streamingAnimation );

        if ( !IsRadialChart( model ) )
            RenderPlotClipPath( builder, ref sequence, plot, options );

        if ( !IsBarChart( model ) && !IsRadialChart( model ) && streamingAnimation.Enabled )
            RenderCategoryAxisLabels( builder, ref sequence, model, plot, streamingAnimation );

        if ( IsRadialChart( model ) )
        {
            RenderRadialChart( builder, ref sequence, model, plot );
            RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.SeriesOverlay );
        }

        if ( !IsRadialChart( model ) )
            RenderCartesianSeries( builder, ref sequence, model, plot, streamingAnimation, pluginContext );

        if ( hasBottomLegend )
            RenderLegend( builder, ref sequence, model, options, options.Height - 30 );

        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.InteractionOverlay );
        RenderActiveTooltip( builder, ref sequence, model );
        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.Tooltip );

        builder.CloseElement();
        builder.CloseElement();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        var zoom = ResolveZoom( ResolveOptions() );
        var shouldPreventWheelScroll = zoom.Enabled && zoom.Wheel;

        if ( shouldPreventWheelScroll && !zoomWheelInitialized )
        {
            jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>( "import", "./_content/Blazorise.Charts.Svg/svgChart.js" );
            await jsModule.InvokeVoidAsync( "initializeZoomWheel", ElementRef );
            zoomWheelInitialized = true;
        }
        else if ( !shouldPreventWheelScroll && zoomWheelInitialized )
        {
            await DestroyZoomWheel();
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            await DestroyZoomWheel();

            if ( jsModule is not null )
            {
                try
                {
                    await jsModule.DisposeAsync();
                }
                catch ( JSDisconnectedException )
                {
                }

                jsModule = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    private async ValueTask DestroyZoomWheel()
    {
        if ( jsModule is null || !zoomWheelInitialized )
            return;

        try
        {
            await jsModule.InvokeVoidAsync( "destroyZoomWheel", ElementRef );
        }
        catch ( JSDisconnectedException )
        {
        }

        zoomWheelInitialized = false;
    }

    private static void RenderFocusStyles( RenderTreeBuilder builder, ref int sequence )
    {
        builder.OpenElement( sequence++, "style" );
        builder.AddContent( sequence++, ".svg-chart-surface [tabindex]:focus{outline:none;}.svg-chart-surface.svg-chart-pannable,.svg-chart-surface.svg-chart-pannable *{user-select:none;-webkit-user-select:none;-webkit-user-drag:none;}" );
        builder.CloseElement();
    }

    private static string ResolveSvgStyle( SvgChartOptions options, SvgChartZoomOptions zoom, bool panning )
    {
        var style = options.Responsive
            ? "display:block;width:100%;height:auto;overflow:visible;"
            : $"display:block;width:{Format( options.Width )}px;height:{Format( options.Height )}px;overflow:visible;";

        if ( zoom?.Enabled == true )
            style += "touch-action:none;";

        if ( zoom?.Enabled == true && zoom.Wheel )
            style += "overscroll-behavior:contain;";

        if ( zoom?.Enabled == true && zoom.Pan )
            style += panning ? "cursor:grabbing;" : "cursor:grab;";

        return style;
    }

    private SvgChartPluginRenderContext CreatePluginRenderContext( RenderModel model, PlotArea plot )
    {
        var pointXScale = ResolvePointXScale( model );

        return new(
            model.Type,
            model.Options,
            new()
            {
                Left = plot.Left,
                Top = plot.Top,
                Right = plot.Right,
                Bottom = plot.Bottom
            },
            model.Labels,
            model.Series.Select( x => new SvgChartPluginSeries
            {
                Name = x.Name,
                Type = x.Type,
                Values = x.Values,
                XValues = x.XValues,
                YValues = x.YValues,
                RadiusValues = x.RadiusValues,
                Color = x.RenderColor,
                Hidden = x.Hidden,
                Order = x.Order,
                CategoryAxisId = x.CategoryAxisId,
                ValueAxisId = x.ValueAxisId,
                BorderRadius = x.BorderRadius,
                StrokeWidth = x.StrokeWidth,
                MarkerRadius = x.MarkerRadius,
                FillOpacity = x.FillOpacity
            } ).ToList(),
            IsRadialChart( model ),
            model.Min,
            model.Max,
            GetCategorySlotCount( model ),
            this,
            value => GetCategoryX( (int)Math.Round( value, MidpointRounding.AwayFromZero ), plot, model ),
            value => GetCategoryBoundaryX( value, plot, model ),
            ( value, valueAxisId ) =>
            {
                if ( pointXScale is not null )
                    return GetX( value, plot, pointXScale.Min, pointXScale.Max );

                var axis = ResolveValueAxis( model, valueAxisId );

                return GetX( value, plot, axis.Min, axis.Max );
            },
            ( value, valueAxisId ) => GetY( value, plot, ResolveValueAxis( model, valueAxisId ) ),
            ( value, fallback ) => ResolveAnnotationX( model, plot, value, pointXScale, fallback ),
            ( value, valueAxisId, fallback ) => ResolveAnnotationY( model, plot, value, valueAxisId, fallback ),
            IsDataPointHidden,
            HandlePointClicked,
            HandlePointHovered,
            HandleDataLabelClicked,
            HandleDataLabelHovered,
            HandlePointLeft,
            ShowTooltip,
            () => InvokeAsync( StateHasChanged ) );
    }

    private void RenderPlugins( RenderTreeBuilder builder, ref int sequence, SvgChartPluginRenderContext context, SvgChartRenderLayer layer )
    {
        var plugins = ResolveRenderablePlugins( context.Options )
            .Where( x => x.RendersContent && x.Layer == layer )
            .OrderBy( ResolvePluginOrder )
            .ToList();

        if ( plugins.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", $"svg-chart-plugins svg-chart-plugins-{layer.ToString().ToLowerInvariant()}" );

        foreach ( var plugin in plugins )
            plugin.Render( context, builder, ref sequence );

        builder.CloseElement();
    }

    private List<ISvgChartPlugin> ResolveRenderablePlugins( SvgChartOptions options )
    {
        var result = new List<ISvgChartPlugin>();

        if ( options.Annotations is not null )
            result.AddRange( options.Annotations.Select( SvgChartAnnotationPluginFactory.Create ).Where( x => x is not null ) );

        if ( options.Trendlines is not null )
            result.AddRange( options.Trendlines.Select( SvgChartTrendline.Create ) );

        if ( options.DataLabels?.Visible == true && !pluginComponents.OfType<SvgChartDataLabels>().Any() )
            result.Add( SvgChartDataLabels.Create( options.DataLabels ) );

        result.AddRange( pluginComponents );

        return result;
    }

    private static int ResolvePluginOrder( ISvgChartPlugin plugin )
    {
        return plugin switch
        {
            SvgChartLineAnnotation annotation => annotation.Order ?? 0,
            SvgChartBoxAnnotation annotation => annotation.Order ?? 0,
            SvgChartLabelAnnotation annotation => annotation.Order ?? 0,
            SvgChartPointAnnotation annotation => annotation.Order ?? 0,
            SvgChartEllipseAnnotation annotation => annotation.Order ?? 0,
            SvgChartTrendline trendline => trendline.Order ?? 0,
            _ => 0
        };
    }

    private void RenderChartText( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, SvgChartTextOptions title, SvgChartTextOptions subtitle )
    {
        var top = 0d;
        var bottom = 0d;
        var start = 0d;
        var end = 0d;

        RenderChartText( builder, ref sequence, options, title, ref top, ref bottom, ref start, ref end );
        RenderChartText( builder, ref sequence, options, subtitle, ref top, ref bottom, ref start, ref end );
    }

    private void RenderChartText( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, SvgChartTextOptions text, ref double top, ref double bottom, ref double start, ref double end )
    {
        if ( !IsTextVisible( text ) )
            return;

        var padding = text.Padding ?? new();
        var fontSize = text.Font?.Size ?? 12;
        var x = ResolveTextX( options, text, start, end, fontSize );
        var y = ResolveTextY( options, text, top, bottom, fontSize );

        builder.OpenElement( sequence++, "text" );
        builder.AddAttribute( sequence++, "x", Format( x ) );
        builder.AddAttribute( sequence++, "y", Format( y ) );
        builder.AddAttribute( sequence++, "text-anchor", ResolveTextAnchor( text.Alignment ) );
        builder.AddAttribute( sequence++, "font-size", Format( fontSize ) );
        AddFontFamilyAttribute( builder, ref sequence, text.Font?.Family ?? options.Font?.Family );
        builder.AddAttribute( sequence++, "fill", ResolveTextColor( options, text ) );
        builder.AddAttribute( sequence++, "opacity", Format( Math.Clamp( text.Opacity ?? 1, 0, 1 ) ) );

        var fontWeight = text.Font?.Weight ?? options.Font?.Weight;

        if ( !string.IsNullOrWhiteSpace( fontWeight ) )
            builder.AddAttribute( sequence++, "font-weight", fontWeight );

        if ( text.Position is SvgChartTextPosition.Start or SvgChartTextPosition.End )
        {
            builder.AddAttribute( sequence++, "transform", $"rotate(-90 {Format( x )} {Format( y )})" );
        }

        builder.AddContent( sequence++, text.Text );
        builder.CloseElement();

        switch ( text.Position )
        {
            case SvgChartTextPosition.Bottom:
                bottom += padding.Top + fontSize + padding.Bottom;
                break;
            case SvgChartTextPosition.Start:
                start += padding.Start + fontSize + padding.End;
                break;
            case SvgChartTextPosition.End:
                end += padding.Start + fontSize + padding.End;
                break;
            default:
                top += padding.Top + fontSize + padding.Bottom;
                break;
        }
    }

    private void RenderPlotClipPath( RenderTreeBuilder builder, ref int sequence, PlotArea plot, SvgChartOptions options )
    {
        builder.OpenElement( sequence++, "defs" );
        builder.OpenElement( sequence++, "clipPath" );
        builder.AddAttribute( sequence++, "id", GetPlotClipPathId() );

        builder.OpenElement( sequence++, "rect" );
        builder.AddAttribute( sequence++, "x", Format( plot.Left + 0.5 ) );
        builder.AddAttribute( sequence++, "y", Format( plot.Top ) );
        builder.AddAttribute( sequence++, "width", Format( Math.Max( 0, plot.Width - 0.5 ) ) );
        builder.AddAttribute( sequence++, "height", Format( plot.Height ) );
        builder.CloseElement();

        builder.CloseElement();

        builder.OpenElement( sequence++, "clipPath" );
        builder.AddAttribute( sequence++, "id", GetCategoryAxisLabelsClipPathId() );

        builder.OpenElement( sequence++, "rect" );
        builder.AddAttribute( sequence++, "x", Format( plot.Left + 0.5 ) );
        builder.AddAttribute( sequence++, "y", Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "width", Format( Math.Max( 0, plot.Width - 0.5 ) ) );
        builder.AddAttribute( sequence++, "height", Format( Math.Max( 0, options.Height - plot.Bottom ) ) );
        builder.CloseElement();

        builder.CloseElement();
        builder.CloseElement();
    }

    private void RenderGridAndAxes( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, (bool Enabled, double OffsetX, TimeSpan Duration) streamingAnimation )
    {
        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid" );

        var primaryAxis = model.PrimaryValueAxis;

        var primaryGridLines = primaryAxis.GridLines;

        if ( primaryGridLines?.Visible == true )
        {
            foreach ( var tick in primaryAxis.Ticks )
            {
                var y = GetY( tick, plot, primaryAxis );

                builder.OpenElement( sequence++, "line" );
                builder.AddAttribute( sequence++, "x1", Format( plot.Left ) );
                builder.AddAttribute( sequence++, "x2", Format( plot.Right ) );
                builder.AddAttribute( sequence++, "y1", Format( y ) );
                builder.AddAttribute( sequence++, "y2", Format( y ) );
                AddGridLineAttributes( builder, ref sequence, primaryGridLines );
                builder.CloseElement();
            }
        }

        if ( model.Series.Any( x => !x.Hidden && IsPointChart( x.Type ) ) )
            RenderPointXAxisGridAndLabels( builder, ref sequence, model, plot );
        else
            RenderCategoryAxisGridLines( builder, ref sequence, model, plot, streamingAnimation );

        foreach ( var tick in primaryAxis.Ticks )
        {
            var y = GetY( tick, plot, primaryAxis );

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", Format( plot.Left - 10 ) );
            builder.AddAttribute( sequence++, "y", Format( y + 4 ) );
            builder.AddAttribute( sequence++, "text-anchor", "end" );
            AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.68 );
            builder.AddContent( sequence++, FormatTick( tick ) );
            builder.CloseElement();
        }

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "x1", Format( plot.Left ) );
        builder.AddAttribute( sequence++, "x2", Format( plot.Left ) );
        builder.AddAttribute( sequence++, "y1", Format( plot.Top ) );
        builder.AddAttribute( sequence++, "y2", Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "stroke", "currentColor" );
        builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
        builder.CloseElement();

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "x1", Format( plot.Left ) );
        builder.AddAttribute( sequence++, "x2", Format( plot.Right ) );
        builder.AddAttribute( sequence++, "y1", Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "y2", Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "stroke", "currentColor" );
        builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
        builder.CloseElement();

        if ( !streamingAnimation.Enabled && !model.Series.Any( x => !x.Hidden && IsPointChart( x.Type ) ) )
            RenderCategoryAxisLabels( builder, ref sequence, model, plot, streamingAnimation );

        RenderRightValueAxes( builder, ref sequence, model, plot, primaryAxis );

        builder.CloseElement();
    }

    private void RenderCategoryAxisGridLines( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, (bool Enabled, double OffsetX, TimeSpan Duration) streamingAnimation )
    {
        var gridLines = model.CategoryAxis.GridLines;

        if ( gridLines?.Visible != true )
            return;

        var labels = model.CategoryAxis.Labels ?? new();
        var labelStep = Math.Max( 1, labels.Step );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid svg-chart-category-grid" );
        builder.AddAttribute( sequence++, "clip-path", $"url(#{GetPlotClipPathId()})" );

        if ( streamingAnimation.Enabled )
        {
            builder.OpenElement( sequence++, "g" );
            builder.SetKey( $"streaming-category-grid-{streamingAnimationVersion}" );
            builder.AddAttribute( sequence++, "style", ResolveStreamingAnimationStyle( streamingAnimation ) );
        }

        for ( var i = 0; i < model.Labels.Count; i++ )
        {
            var labelIndex = i < model.CategoryLabelIndexes.Count ? model.CategoryLabelIndexes[i] : i;

            if ( labelIndex < 0 || labelIndex % labelStep != 0 )
                continue;

            var x = GetCategoryX( i, plot, model );

            builder.OpenElement( sequence++, "line" );
            builder.AddAttribute( sequence++, "x1", Format( x ) );
            builder.AddAttribute( sequence++, "x2", Format( x ) );
            builder.AddAttribute( sequence++, "y1", Format( plot.Top ) );
            builder.AddAttribute( sequence++, "y2", Format( plot.Bottom ) );
            AddGridLineAttributes( builder, ref sequence, gridLines );
            builder.CloseElement();
        }

        if ( streamingAnimation.Enabled )
            builder.CloseElement();

        builder.CloseElement();
    }

    private void RenderCategoryAxisLabels( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, (bool Enabled, double OffsetX, TimeSpan Duration) streamingAnimation )
    {
        var labels = model.CategoryAxis.Labels ?? new();
        var labelStep = Math.Max( 1, labels.Step );

        if ( !labels.Visible )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-axis-labels svg-chart-category-axis-labels" );

        if ( streamingAnimation.Enabled || model.Zoom?.Enabled == true )
        {
            builder.AddAttribute( sequence++, "clip-path", $"url(#{GetCategoryAxisLabelsClipPathId()})" );
        }

        if ( streamingAnimation.Enabled )
        {
            builder.OpenElement( sequence++, "g" );
            builder.SetKey( $"streaming-axis-labels-{streamingAnimationVersion}" );
            builder.AddAttribute( sequence++, "style", ResolveStreamingAnimationStyle( streamingAnimation ) );
        }

        for ( var i = 0; i < model.Labels.Count; i++ )
        {
            var labelIndex = i < model.CategoryLabelIndexes.Count ? model.CategoryLabelIndexes[i] : i;

            if ( labelIndex < 0 || labelIndex % labelStep != 0 )
                continue;

            var x = GetCategoryX( i, plot, model );

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", Format( x ) );
            builder.AddAttribute( sequence++, "y", Format( plot.Bottom + labels.Offset ) );
            builder.AddAttribute( sequence++, "text-anchor", "middle" );
            AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.72 );
            builder.AddContent( sequence++, model.Labels[i]?.ToString() );
            builder.CloseElement();
        }

        if ( streamingAnimation.Enabled )
            builder.CloseElement();

        builder.CloseElement();
    }

    private void RenderPointXAxisGridAndLabels( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot )
    {
        var scale = ResolvePointXScale( model );

        if ( scale is null )
            return;

        var gridLines = model.CategoryAxis.GridLines;
        var labels = model.CategoryAxis.Labels ?? new();

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid svg-chart-point-xaxis-grid" );
        builder.AddAttribute( sequence++, "clip-path", $"url(#{GetPlotClipPathId()})" );

        foreach ( var tick in scale.Ticks )
        {
            var x = GetX( tick, plot, scale.Min, scale.Max );

            if ( gridLines?.Visible == true )
            {
                builder.OpenElement( sequence++, "line" );
                builder.AddAttribute( sequence++, "x1", Format( x ) );
                builder.AddAttribute( sequence++, "x2", Format( x ) );
                builder.AddAttribute( sequence++, "y1", Format( plot.Top ) );
                builder.AddAttribute( sequence++, "y2", Format( plot.Bottom ) );
                AddGridLineAttributes( builder, ref sequence, gridLines );
                builder.CloseElement();
            }
        }

        builder.CloseElement();

        if ( !labels.Visible )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-axis-labels svg-chart-point-xaxis-labels" );
        builder.AddAttribute( sequence++, "clip-path", $"url(#{GetCategoryAxisLabelsClipPathId()})" );

        for ( var i = 0; i < scale.Ticks.Count; i += Math.Max( 1, labels.Step ) )
        {
            var tick = scale.Ticks[i];
            var x = GetX( tick, plot, scale.Min, scale.Max );

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", Format( x ) );
            builder.AddAttribute( sequence++, "y", Format( plot.Bottom + labels.Offset ) );
            builder.AddAttribute( sequence++, "text-anchor", "middle" );
            AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.72 );
            builder.AddContent( sequence++, FormatTick( tick ) );
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    private void RenderRightValueAxes( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, RenderValueAxis primaryAxis )
    {
        foreach ( var axis in model.ValueAxes.Where( x => x != primaryAxis && x.Position == SvgChartAxisPosition.Right ) )
        {
            builder.OpenElement( sequence++, "g" );
            builder.AddAttribute( sequence++, "class", "svg-chart-axis svg-chart-value-axis-right" );

            builder.OpenElement( sequence++, "line" );
            builder.AddAttribute( sequence++, "x1", Format( plot.Right ) );
            builder.AddAttribute( sequence++, "x2", Format( plot.Right ) );
            builder.AddAttribute( sequence++, "y1", Format( plot.Top ) );
            builder.AddAttribute( sequence++, "y2", Format( plot.Bottom ) );
            builder.AddAttribute( sequence++, "stroke", "currentColor" );
            builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
            builder.CloseElement();

            foreach ( var tick in axis.Ticks )
            {
                var y = GetY( tick, plot, axis );

                builder.OpenElement( sequence++, "text" );
                builder.AddAttribute( sequence++, "x", Format( plot.Right + 10 ) );
                builder.AddAttribute( sequence++, "y", Format( y + 4 ) );
                builder.AddAttribute( sequence++, "text-anchor", "start" );
                AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.68 );
                builder.AddContent( sequence++, FormatTick( tick ) );
                builder.CloseElement();
            }

            builder.CloseElement();
        }
    }

    private static void AddGridLineAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartGridLinesOptions gridLines )
    {
        builder.AddAttribute( sequence++, "stroke", ResolveGridLineColor( gridLines ) );
        builder.AddAttribute( sequence++, "stroke-width", Format( Math.Max( 0, gridLines?.Width ?? 1 ) ) );
        builder.AddAttribute( sequence++, "stroke-opacity", Format( Math.Clamp( gridLines?.Opacity ?? 0.14, 0, 1 ) ) );

        if ( !string.IsNullOrWhiteSpace( gridLines?.DashPattern ) )
            builder.AddAttribute( sequence++, "stroke-dasharray", gridLines.DashPattern );
    }

    private void RenderHorizontalGridAndAxes( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot )
    {
        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-grid" );

        foreach ( var tick in model.Ticks )
        {
            var x = GetX( tick, plot, model );

            var gridLines = model.PrimaryValueAxis.GridLines;

            if ( gridLines?.Visible == true )
            {
                builder.OpenElement( sequence++, "line" );
                builder.AddAttribute( sequence++, "x1", Format( x ) );
                builder.AddAttribute( sequence++, "x2", Format( x ) );
                builder.AddAttribute( sequence++, "y1", Format( plot.Top ) );
                builder.AddAttribute( sequence++, "y2", Format( plot.Bottom ) );
                AddGridLineAttributes( builder, ref sequence, gridLines );
                builder.CloseElement();
            }

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", Format( x ) );
            builder.AddAttribute( sequence++, "y", Format( plot.Bottom + 24 ) );
            builder.AddAttribute( sequence++, "text-anchor", "middle" );
            AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.68 );
            builder.AddContent( sequence++, FormatTick( tick ) );
            builder.CloseElement();
        }

        builder.OpenElement( sequence++, "line" );
        builder.AddAttribute( sequence++, "x1", Format( plot.Left ) );
        builder.AddAttribute( sequence++, "x2", Format( plot.Right ) );
        builder.AddAttribute( sequence++, "y1", Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "y2", Format( plot.Bottom ) );
        builder.AddAttribute( sequence++, "stroke", "currentColor" );
        builder.AddAttribute( sequence++, "stroke-opacity", "0.22" );
        builder.CloseElement();

        for ( var i = 0; i < model.Labels.Count; i++ )
        {
            var y = plot.Top + plot.Height * ( i + 0.5 ) / Math.Max( model.Labels.Count, 1 );

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", Format( plot.Left - 10 ) );
            builder.AddAttribute( sequence++, "y", Format( y + 4 ) );
            builder.AddAttribute( sequence++, "text-anchor", "end" );
            AddFontAttributes( builder, ref sequence, model.Options, opacity: 0.72 );
            builder.AddContent( sequence++, model.Labels[i]?.ToString() );
            builder.CloseElement();
        }

        builder.CloseElement();
    }

    private void RenderCartesianSeries( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, (bool Enabled, double OffsetX, TimeSpan Duration) animation, SvgChartPluginRenderContext pluginContext )
    {
        var renderOrders = model.Series.Where( x => !x.Hidden && !IsRadialChart( x.Type ) )
            .Select( ResolveRenderOrder )
            .Distinct()
            .OrderBy( x => x )
            .ToList();

        var clipped = animation.Enabled || model.Zoom?.Enabled == true;

        if ( clipped )
        {
            builder.OpenElement( sequence++, "g" );
            builder.AddAttribute( sequence++, "class", "svg-chart-streaming-viewport" );
            builder.AddAttribute( sequence++, "clip-path", $"url(#{GetPlotClipPathId()})" );
        }

        if ( animation.Enabled )
        {
            builder.OpenElement( sequence++, "g" );
            builder.SetKey( $"streaming-animation-{streamingAnimationVersion}" );
            builder.AddAttribute( sequence++, "class", "svg-chart-streaming-content" );
            builder.AddAttribute( sequence++, "style", ResolveStreamingAnimationStyle( animation ) );
        }

        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.BeforeSeries );

        foreach ( var renderOrder in renderOrders )
        {
            bool ShouldRender( RenderSeries series ) => ResolveRenderOrder( series ) == renderOrder;

            if ( model.Series.Any( x => x.Type == SvgChartType.Area && ShouldRender( x ) ) )
                RenderAreas( builder, ref sequence, model, plot, ShouldRender );

            if ( model.Series.Any( x => x.Type == SvgChartType.Column && ShouldRender( x ) ) )
                RenderColumns( builder, ref sequence, model, plot, ShouldRender );

            if ( model.Series.Any( x => x.Type == SvgChartType.Bar && ShouldRender( x ) ) )
                RenderBars( builder, ref sequence, model, plot, ShouldRender );

            if ( model.Series.Any( x => x.Type == SvgChartType.Line && ShouldRender( x ) ) )
                RenderLines( builder, ref sequence, model, plot, ShouldRender );

            if ( model.Series.Any( x => IsPointChart( x.Type ) && ShouldRender( x ) ) )
                RenderPointCharts( builder, ref sequence, model, plot, ShouldRender );
        }

        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.SeriesOverlay );

        if ( animation.Enabled )
        {
            builder.CloseElement();
        }

        if ( clipped )
        {
            builder.CloseElement();
        }
    }

    private void RenderColumns( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, Func<RenderSeries, bool> shouldRender = null )
    {
        var visibleColumnSeries = model.Series.Where( x => x.Type == SvgChartType.Column && !x.Hidden ).ToList();

        if ( visibleColumnSeries.Count == 0 || model.Labels.Count == 0 || !visibleColumnSeries.Any( x => shouldRender?.Invoke( x ) ?? true ) )
            return;

        var categoryWidth = GetCategoryWidth( plot, model );
        var groupWidth = categoryWidth * 0.72;
        var barWidth = Math.Max( 1, groupWidth / visibleColumnSeries.Count );
        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-columns" );

        for ( var seriesIndex = 0; seriesIndex < visibleColumnSeries.Count; seriesIndex++ )
        {
            var series = visibleColumnSeries[seriesIndex];

            if ( shouldRender is not null && !shouldRender( series ) )
                continue;

            var baseline = GetY( 0, plot, model, series );

            for ( var pointIndex = 0; pointIndex < model.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
            {
                var value = series.Values[pointIndex];

                if ( !value.HasValue )
                    continue;

                var categoryStart = GetCategoryBoundaryX( pointIndex, plot, model ) + ( categoryWidth - groupWidth ) / 2;
                var x = categoryStart + barWidth * seriesIndex + barWidth * 0.1;
                var y = GetY( value.Value, plot, model, series );
                var height = Math.Abs( baseline - y );
                var rectY = Math.Min( y, baseline );
                var rectWidth = Math.Max( 1, barWidth * 0.8 );
                var bounds = new SvgChartPointBounds { X = x, Y = rectY, Width = rectWidth, Height = height };
                var point = CreatePointArgs( model, series, pointIndex, value.Value, bounds );

                builder.OpenElement( sequence++, "rect" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-column" );
                builder.AddAttribute( sequence++, "x", Format( x ) );
                builder.AddAttribute( sequence++, "y", Format( rectY ) );
                builder.AddAttribute( sequence++, "width", Format( rectWidth ) );
                builder.AddAttribute( sequence++, "height", Format( height ) );
                builder.AddAttribute( sequence++, "rx", Format( series.BorderRadius ) );
                builder.AddAttribute( sequence++, "fill", series.RenderColor );
                builder.AddAttribute( sequence++, "tabindex", "0" );
                builder.AddAttribute( sequence++, "role", "img" );
                builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointClicked( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointHovered( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointLeft() ) );
                builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, () => ShowTooltip( point, series.RenderColor, false ) ) );
                builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, () => HandlePointLeft() ) );

                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    private void RenderBars( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, Func<RenderSeries, bool> shouldRender = null )
    {
        var visibleBarSeries = model.Series.Where( x => x.Type == SvgChartType.Bar && !x.Hidden ).ToList();

        if ( visibleBarSeries.Count == 0 || model.Labels.Count == 0 || !visibleBarSeries.Any( x => shouldRender?.Invoke( x ) ?? true ) )
            return;

        var categoryHeight = plot.Height / model.Labels.Count;
        var groupHeight = categoryHeight * 0.72;
        var barHeight = Math.Max( 1, groupHeight / visibleBarSeries.Count );
        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-bars" );

        for ( var seriesIndex = 0; seriesIndex < visibleBarSeries.Count; seriesIndex++ )
        {
            var series = visibleBarSeries[seriesIndex];

            if ( shouldRender is not null && !shouldRender( series ) )
                continue;

            var baseline = GetX( 0, plot, model, series );

            for ( var pointIndex = 0; pointIndex < model.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
            {
                var value = series.Values[pointIndex];

                if ( !value.HasValue )
                    continue;

                var categoryStart = plot.Top + categoryHeight * pointIndex + ( categoryHeight - groupHeight ) / 2;
                var x = GetX( value.Value, plot, model, series );
                var width = Math.Abs( x - baseline );
                var rectX = Math.Min( x, baseline );
                var y = categoryStart + barHeight * seriesIndex + barHeight * 0.1;
                var rectHeight = Math.Max( 1, barHeight * 0.8 );
                var bounds = new SvgChartPointBounds { X = rectX, Y = y, Width = width, Height = rectHeight };
                var point = CreatePointArgs( model, series, pointIndex, value.Value, bounds );

                builder.OpenElement( sequence++, "rect" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-bar" );
                builder.AddAttribute( sequence++, "x", Format( rectX ) );
                builder.AddAttribute( sequence++, "y", Format( y ) );
                builder.AddAttribute( sequence++, "width", Format( width ) );
                builder.AddAttribute( sequence++, "height", Format( rectHeight ) );
                builder.AddAttribute( sequence++, "rx", Format( series.BorderRadius ) );
                builder.AddAttribute( sequence++, "fill", series.RenderColor );
                builder.AddAttribute( sequence++, "tabindex", "0" );
                builder.AddAttribute( sequence++, "role", "img" );
                builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointClicked( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointHovered( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointLeft() ) );
                builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, () => ShowTooltip( point, series.RenderColor, false ) ) );
                builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, () => HandlePointLeft() ) );

                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    private void RenderLines( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, Func<RenderSeries, bool> shouldRender = null )
    {
        var lineSeries = model.Series.Where( x => x.Type == SvgChartType.Line && !x.Hidden && ( shouldRender?.Invoke( x ) ?? true ) ).ToList();

        if ( lineSeries.Count == 0 || model.Labels.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-lines" );

        foreach ( var series in lineSeries )
        {
            var points = new List<(int Index, double X, double Y, double Value)>();

            for ( var pointIndex = 0; pointIndex < model.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
            {
                var value = series.Values[pointIndex];

                if ( value.HasValue )
                {
                    points.Add( (pointIndex, GetCategoryX( pointIndex, plot, model ), GetY( value.Value, plot, model, series ), value.Value) );
                }
            }

            if ( points.Count > 1 )
            {
                builder.OpenElement( sequence++, "path" );
                builder.AddAttribute( sequence++, "class", "svg-chart-line" );
                builder.AddAttribute( sequence++, "d", BuildLinePath( points ) );
                builder.AddAttribute( sequence++, "fill", "none" );
                builder.AddAttribute( sequence++, "stroke", series.RenderColor );
                builder.AddAttribute( sequence++, "stroke-width", Format( series.StrokeWidth ) );
                builder.AddAttribute( sequence++, "stroke-linecap", "round" );
                builder.AddAttribute( sequence++, "stroke-linejoin", "round" );
                builder.CloseElement();
            }

            foreach ( var renderedPoint in points )
            {
                var bounds = new SvgChartPointBounds
                {
                    X = renderedPoint.X - series.MarkerRadius,
                    Y = renderedPoint.Y - series.MarkerRadius,
                    Width = series.MarkerRadius * 2,
                    Height = series.MarkerRadius * 2
                };
                var point = CreatePointArgs( model, series, renderedPoint.Index, renderedPoint.Value, bounds );

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-marker" );
                builder.AddAttribute( sequence++, "cx", Format( renderedPoint.X ) );
                builder.AddAttribute( sequence++, "cy", Format( renderedPoint.Y ) );
                builder.AddAttribute( sequence++, "r", Format( series.MarkerRadius ) );
                builder.AddAttribute( sequence++, "fill", series.RenderColor );
                builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
                builder.AddAttribute( sequence++, "stroke-width", "1.5" );
                builder.AddAttribute( sequence++, "tabindex", "0" );
                builder.AddAttribute( sequence++, "role", "img" );
                builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointClicked( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointHovered( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointLeft() ) );
                builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, () => ShowTooltip( point, series.RenderColor, false ) ) );
                builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, () => HandlePointLeft() ) );

                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    private void RenderAreas( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, Func<RenderSeries, bool> shouldRender = null )
    {
        var areaSeries = model.Series.Where( x => x.Type == SvgChartType.Area && !x.Hidden && ( shouldRender?.Invoke( x ) ?? true ) ).ToList();

        if ( areaSeries.Count == 0 || model.Labels.Count == 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-areas" );

        foreach ( var series in areaSeries )
        {
            var points = new List<(int Index, double X, double Y, double Value)>();
            var baseline = GetY( 0, plot, model, series );

            for ( var pointIndex = 0; pointIndex < model.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
            {
                var value = series.Values[pointIndex];

                if ( value.HasValue )
                    points.Add( (pointIndex, GetCategoryX( pointIndex, plot, model ), GetY( value.Value, plot, model, series ), value.Value) );
            }

            if ( points.Count > 1 )
            {
                var areaPath = $"{BuildLinePath( points )} L {Format( points[^1].X )} {Format( baseline )} L {Format( points[0].X )} {Format( baseline )} Z";

                builder.OpenElement( sequence++, "path" );
                builder.AddAttribute( sequence++, "class", "svg-chart-area" );
                builder.AddAttribute( sequence++, "d", areaPath );
                builder.AddAttribute( sequence++, "fill", series.RenderColor );
                builder.AddAttribute( sequence++, "opacity", Format( series.FillOpacity ) );
                builder.CloseElement();

                builder.OpenElement( sequence++, "path" );
                builder.AddAttribute( sequence++, "class", "svg-chart-area-line" );
                builder.AddAttribute( sequence++, "d", BuildLinePath( points ) );
                builder.AddAttribute( sequence++, "fill", "none" );
                builder.AddAttribute( sequence++, "stroke", series.RenderColor );
                builder.AddAttribute( sequence++, "stroke-width", Format( series.StrokeWidth ) );
                builder.AddAttribute( sequence++, "stroke-linecap", "round" );
                builder.AddAttribute( sequence++, "stroke-linejoin", "round" );
                builder.CloseElement();
            }

            foreach ( var renderedPoint in points )
            {
                var markerRadius = Math.Max( 3, series.StrokeWidth + 1 );
                var bounds = new SvgChartPointBounds
                {
                    X = renderedPoint.X - markerRadius,
                    Y = renderedPoint.Y - markerRadius,
                    Width = markerRadius * 2,
                    Height = markerRadius * 2
                };
                var point = CreatePointArgs( model, series, renderedPoint.Index, renderedPoint.Value, bounds );

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-area-marker" );
                builder.AddAttribute( sequence++, "cx", Format( renderedPoint.X ) );
                builder.AddAttribute( sequence++, "cy", Format( renderedPoint.Y ) );
                builder.AddAttribute( sequence++, "r", Format( markerRadius ) );
                builder.AddAttribute( sequence++, "fill", series.RenderColor );
                builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
                builder.AddAttribute( sequence++, "stroke-width", "1.5" );
                builder.AddAttribute( sequence++, "tabindex", "0" );
                builder.AddAttribute( sequence++, "role", "img" );
                builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointClicked( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointHovered( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointLeft() ) );
                builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, () => ShowTooltip( point, series.RenderColor, false ) ) );
                builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, () => HandlePointLeft() ) );
                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    private void RenderLegend( RenderTreeBuilder builder, ref int sequence, RenderModel model, SvgChartOptions options, double y )
    {
        var legendItems = ResolveLegendItems( model );

        if ( legendItems.Count == 0 )
            return;

        var itemWidth = Math.Min( 140, options.Width / Math.Max( legendItems.Count, 1 ) );
        var totalWidth = itemWidth * legendItems.Count;
        var startX = Math.Max( 8, ( options.Width - totalWidth ) / 2 );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-legend" );

        for ( var i = 0; i < legendItems.Count; i++ )
        {
            var item = legendItems[i];
            var x = startX + itemWidth * i;

            builder.OpenElement( sequence++, "g" );
            builder.AddAttribute( sequence++, "class", "svg-chart-legend-item" );
            builder.AddAttribute( sequence++, "role", "button" );
            builder.AddAttribute( sequence++, "tabindex", "0" );
            builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, item.Toggle ) );

            builder.OpenElement( sequence++, "rect" );
            builder.AddAttribute( sequence++, "x", Format( x ) );
            builder.AddAttribute( sequence++, "y", Format( y - 9 ) );
            builder.AddAttribute( sequence++, "width", "12" );
            builder.AddAttribute( sequence++, "height", "12" );
            builder.AddAttribute( sequence++, "rx", "3" );
            builder.AddAttribute( sequence++, "fill", item.Color );
            builder.AddAttribute( sequence++, "opacity", item.Hidden ? "0.35" : "1" );
            builder.CloseElement();

            builder.OpenElement( sequence++, "text" );
            builder.AddAttribute( sequence++, "x", Format( x + 18 ) );
            builder.AddAttribute( sequence++, "y", Format( y + 1 ) );
            AddFontAttributes( builder, ref sequence, options, fallbackSize: 12, opacity: item.Hidden ? 0.45 : 0.8 );
            builder.AddContent( sequence++, item.Label );
            builder.CloseElement();

            builder.CloseElement();
        }

        builder.CloseElement();
    }

    private List<LegendItem> ResolveLegendItems( RenderModel model )
    {
        if ( model.Series.Count == 0 )
            return [];

        if ( IsRadialCategoryLegendChart( model ) )
            return ResolveRadialLegendItems( model );

        return model.Series.Select( series => new LegendItem
        {
            Label = series.Name,
            Color = series.RenderColor,
            Hidden = series.Hidden,
            Toggle = () => ToggleSeries( series.Name )
        } ).ToList();
    }

    private List<LegendItem> ResolveRadialLegendItems( RenderModel model )
    {
        var series = model.Series.FirstOrDefault();

        if ( series is null )
            return [];

        var count = Math.Max( model.Labels.Count, series.Values.Count );
        var result = new List<LegendItem>();

        for ( var i = 0; i < count; i++ )
        {
            var pointIndex = i;
            var category = i < model.Labels.Count ? model.Labels[i] : i + 1;

            result.Add( new()
            {
                Label = category?.ToString() ?? string.Empty,
                Color = ResolveColor( null, i ),
                Hidden = series.Hidden || IsDataPointHidden( series.Name, pointIndex ),
                Toggle = () => ToggleDataPoint( series.Name, pointIndex )
            } );
        }

        return result;
    }

    private void RenderPointCharts( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot, Func<RenderSeries, bool> shouldRender = null )
    {
        var pointSeries = model.Series.Where( x => !x.Hidden && IsPointChart( x.Type ) && ( shouldRender?.Invoke( x ) ?? true ) ).ToList();

        if ( pointSeries.Count == 0 )
            return;

        var xScale = ResolvePointXScale( model, pointSeries );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-points" );

        foreach ( var series in pointSeries )
        {
            for ( var pointIndex = 0; pointIndex < series.YValues.Count; pointIndex++ )
            {
                var yValue = series.YValues[pointIndex];
                var xValue = pointIndex < series.XValues.Count ? series.XValues[pointIndex] : pointIndex;

                if ( !xValue.HasValue || !yValue.HasValue )
                    continue;

                var x = GetX( xValue.Value, plot, xScale.Min, xScale.Max );
                var y = GetY( yValue.Value, plot, model, series );
                var radius = series.Type == SvgChartType.Bubble
                    ? Math.Max( 2, pointIndex < series.RadiusValues.Count && series.RadiusValues[pointIndex].HasValue ? series.RadiusValues[pointIndex].Value : series.MarkerRadius )
                    : series.MarkerRadius;
                var bounds = new SvgChartPointBounds { X = x - radius, Y = y - radius, Width = radius * 2, Height = radius * 2 };
                var point = new SvgChartPointEventArgs
                {
                    SeriesName = series.Name,
                    SeriesIndex = model.Series.IndexOf( series ),
                    PointIndex = pointIndex,
                    Category = xValue.Value,
                    Value = yValue.Value,
                    Bounds = bounds
                };

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", $"svg-chart-point svg-chart-{series.Type.ToString().ToLowerInvariant()}" );
                builder.AddAttribute( sequence++, "cx", Format( x ) );
                builder.AddAttribute( sequence++, "cy", Format( y ) );
                builder.AddAttribute( sequence++, "r", Format( radius ) );
                builder.AddAttribute( sequence++, "fill", series.RenderColor );
                builder.AddAttribute( sequence++, "opacity", series.Type == SvgChartType.Bubble ? "0.72" : "1" );
                builder.AddAttribute( sequence++, "tabindex", "0" );
                builder.AddAttribute( sequence++, "role", "img" );
                builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointClicked( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointHovered( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointLeft() ) );
                builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, () => ShowTooltip( point, series.RenderColor, false ) ) );
                builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, () => HandlePointLeft() ) );

                builder.CloseElement();
            }
        }

        builder.CloseElement();
    }

    private void RenderRadialChart( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot )
    {
        if ( model.Series.Count == 0 )
            return;

        var radialType = model.Series.FirstOrDefault()?.Type ?? Type;

        if ( radialType == SvgChartType.Radar )
        {
            RenderRadarChart( builder, ref sequence, model, plot );
            return;
        }

        var series = model.Series.FirstOrDefault( x => !x.Hidden );

        if ( series is null )
            return;

        var values = series.Values
            .Select( ( value, index ) => new { Value = value, Index = index } )
            .Where( x => x.Value.HasValue && x.Value.Value >= 0 && !IsDataPointHidden( series.Name, x.Index ) )
            .Select( x => (Value: x.Value.Value, Index: x.Index) )
            .ToList();

        if ( values.Count == 0 )
            return;

        var centerX = plot.Left + plot.Width / 2;
        var centerY = plot.Top + plot.Height / 2;
        var radius = Math.Max( 1, Math.Min( plot.Width, plot.Height ) * 0.42 );
        var total = radialType == SvgChartType.PolarArea ? values.Count : values.Sum( x => x.Value );
        var startAngle = -Math.PI / 2;
        var max = values.Max( x => x.Value );

        if ( total <= 0 )
            return;

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-radial" );

        for ( var i = 0; i < values.Count; i++ )
        {
            var value = values[i].Value;
            var pointIndex = values[i].Index;
            var sweep = radialType == SvgChartType.PolarArea ? ( Math.PI * 2 / values.Count ) : ( value / total * Math.PI * 2 );
            var endAngle = startAngle + sweep;
            var pointRadius = radialType == SvgChartType.PolarArea ? radius * Math.Sqrt( value / Math.Max( max, 1 ) ) : radius;
            var innerRadius = radialType == SvgChartType.Doughnut ? radius * 0.58 : 0;
            var color = ResolveColor( null, pointIndex );
            var category = pointIndex < model.Labels.Count ? model.Labels[pointIndex] : pointIndex + 1;
            var bounds = new SvgChartPointBounds { X = centerX - pointRadius, Y = centerY - pointRadius, Width = pointRadius * 2, Height = pointRadius * 2 };
            var point = new SvgChartPointEventArgs { SeriesName = series.Name, SeriesIndex = model.Series.IndexOf( series ), PointIndex = pointIndex, Category = category, Value = value, Bounds = bounds };

            builder.OpenElement( sequence++, "path" );
            builder.AddAttribute( sequence++, "class", $"svg-chart-point svg-chart-{radialType.ToString().ToLowerInvariant()}-segment" );
            builder.AddAttribute( sequence++, "d", BuildArcPath( centerX, centerY, innerRadius, pointRadius, startAngle, endAngle ) );
            builder.AddAttribute( sequence++, "fill", color );
            builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
            builder.AddAttribute( sequence++, "stroke-width", "1" );
            builder.AddAttribute( sequence++, "tabindex", "0" );
            builder.AddAttribute( sequence++, "role", "img" );
            builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
            builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointClicked( point, color ) ) );
            builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointHovered( point, color ) ) );
            builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointLeft() ) );
            builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, () => ShowTooltip( point, color, false ) ) );
            builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, () => HandlePointLeft() ) );

            builder.CloseElement();

            startAngle = endAngle;
        }

        builder.CloseElement();
    }

    private void RenderRadarChart( RenderTreeBuilder builder, ref int sequence, RenderModel model, PlotArea plot )
    {
        var radarSeries = model.Series.Where( x => x.Type == SvgChartType.Radar && !x.Hidden ).ToList();

        if ( radarSeries.Count == 0 || model.Labels.Count == 0 )
            return;

        var centerX = plot.Left + plot.Width / 2;
        var centerY = plot.Top + plot.Height / 2;
        var radius = Math.Max( 1, Math.Min( plot.Width, plot.Height ) * 0.42 );
        var max = Math.Max( model.Max, 1 );

        builder.OpenElement( sequence++, "g" );
        builder.AddAttribute( sequence++, "class", "svg-chart-radar" );

        for ( var i = 1; i <= 4; i++ )
        {
            builder.OpenElement( sequence++, "polygon" );
            builder.AddAttribute( sequence++, "points", BuildRadarPoints( Enumerable.Repeat( max * i / 4, model.Labels.Count ).Select( x => (double?)x ).ToList(), centerX, centerY, radius, max ) );
            builder.AddAttribute( sequence++, "fill", "none" );
            builder.AddAttribute( sequence++, "stroke", "currentColor" );
            builder.AddAttribute( sequence++, "stroke-opacity", "0.12" );
            builder.CloseElement();
        }

        foreach ( var series in radarSeries )
        {
            builder.OpenElement( sequence++, "polygon" );
            builder.AddAttribute( sequence++, "class", "svg-chart-radar-area" );
            builder.AddAttribute( sequence++, "points", BuildRadarPoints( series.Values, centerX, centerY, radius, max ) );
            builder.AddAttribute( sequence++, "fill", series.RenderColor );
            builder.AddAttribute( sequence++, "opacity", Format( series.FillOpacity ) );
            builder.AddAttribute( sequence++, "stroke", series.RenderColor );
            builder.AddAttribute( sequence++, "stroke-width", "2" );
            builder.CloseElement();

            for ( var pointIndex = 0; pointIndex < model.Labels.Count && pointIndex < series.Values.Count; pointIndex++ )
            {
                var value = series.Values[pointIndex];

                if ( !value.HasValue )
                    continue;

                var angle = -Math.PI / 2 + Math.PI * 2 * pointIndex / model.Labels.Count;
                var renderedRadius = radius * Math.Max( value.Value, 0 ) / max;
                var renderedPoint = PolarToCartesian( centerX, centerY, renderedRadius, angle );
                var markerRadius = 4d;
                var bounds = new SvgChartPointBounds
                {
                    X = renderedPoint.X - markerRadius,
                    Y = renderedPoint.Y - markerRadius,
                    Width = markerRadius * 2,
                    Height = markerRadius * 2
                };
                var point = CreatePointArgs( model, series, pointIndex, value.Value, bounds );

                builder.OpenElement( sequence++, "circle" );
                builder.AddAttribute( sequence++, "class", "svg-chart-point svg-chart-radar-marker" );
                builder.AddAttribute( sequence++, "cx", Format( renderedPoint.X ) );
                builder.AddAttribute( sequence++, "cy", Format( renderedPoint.Y ) );
                builder.AddAttribute( sequence++, "r", Format( markerRadius ) );
                builder.AddAttribute( sequence++, "fill", series.RenderColor );
                builder.AddAttribute( sequence++, "stroke", "var(--bs-body-bg, #fff)" );
                builder.AddAttribute( sequence++, "stroke-width", "1.5" );
                builder.AddAttribute( sequence++, "tabindex", "0" );
                builder.AddAttribute( sequence++, "role", "img" );
                builder.AddAttribute( sequence++, "aria-label", GetPointLabel( point ) );
                builder.AddAttribute( sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointClicked( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointHovered( point, series.RenderColor ) ) );
                builder.AddAttribute( sequence++, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>( this, () => HandlePointLeft() ) );
                builder.AddAttribute( sequence++, "onfocus", EventCallback.Factory.Create<FocusEventArgs>( this, () => ShowTooltip( point, series.RenderColor, false ) ) );
                builder.AddAttribute( sequence++, "onblur", EventCallback.Factory.Create<FocusEventArgs>( this, () => HandlePointLeft() ) );
                builder.CloseElement();

            }
        }

        builder.CloseElement();
    }

    private void RenderActiveTooltip( RenderTreeBuilder builder, ref int sequence, RenderModel model )
    {
        var tooltip = model.Tooltip;
        var context = activeTooltip;

        if ( context is null || tooltip is null || !tooltip.Enabled )
            return;

        builder.OpenElement( sequence++, "foreignObject" );
        builder.AddAttribute( sequence++, "class", "svg-chart-tooltip" );
        builder.AddAttribute( sequence++, "x", Format( context.X ) );
        builder.AddAttribute( sequence++, "y", Format( context.Y ) );
        builder.AddAttribute( sequence++, "width", Format( context.Width ) );
        builder.AddAttribute( sequence++, "height", Format( context.Height ) );
        builder.AddAttribute( sequence++, "style", "pointer-events:none;overflow:visible;" );

        builder.OpenElement( sequence++, "div" );
        builder.AddAttribute( sequence++, "xmlns", "http://www.w3.org/1999/xhtml" );
        builder.AddAttribute( sequence++, "class", "svg-chart-tooltip-content" );
        builder.AddAttribute( sequence++, "style", ResolveTooltipStyle( model.Options, context ) );

        if ( tooltip.Template is not null )
        {
            builder.AddContent( sequence++, tooltip.Template( context ) );
        }
        else
        {
            builder.OpenElement( sequence++, "div" );
            builder.AddAttribute( sequence++, "style", "font-weight:600;line-height:1.2;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;" );
            builder.AddContent( sequence++, context.SeriesName );
            builder.CloseElement();

            builder.OpenElement( sequence++, "div" );
            builder.AddAttribute( sequence++, "style", "line-height:1.35;opacity:.86;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;" );
            builder.AddContent( sequence++, tooltip.Formatter?.Invoke( context ) ?? context.Text );
            builder.CloseElement();
        }

        builder.CloseElement();
        builder.CloseElement();
    }

    private RenderModel BuildModel( bool applyStreamingViewport = true, bool applyZoomViewport = true )
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

        return new RenderModel
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

        if ( categoryAxis?.Value is not null && items.Count > 0 )
            return items.Select( categoryAxis.Value ).ToList();

        var maxValues = seriesComponents.OfType<SvgChartSeries<TItem>>()
            .Select( x => x.Values?.Count ?? 0 )
            .DefaultIfEmpty( 0 )
            .Max();

        return Enumerable.Range( 1, maxValues ).Select( x => (object)x ).ToList();
    }

    private List<RenderSeries> ResolveSeries( SvgChartData<double?> chartData, List<SvgChartSeries<TItem>> childSeries, List<TItem> items, int labelCount )
    {
        var series = new List<RenderSeries>();

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
                    RenderColor = ResolveColor( dataSeries.Color, i ),
                    Hidden = dataSeries.Hidden || hiddenSeries.Contains( name ),
                    Order = dataSeries.Order,
                    CategoryAxisId = dataSeries.CategoryAxisId,
                    ValueAxisId = dataSeries.ValueAxisId,
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
                RenderColor = ResolveColor( child.Color, series.Count ),
                Hidden = child.Hidden || hiddenSeries.Contains( name ),
                Order = child.Order,
                CategoryAxisId = child.CategoryAxisId,
                ValueAxisId = child.ValueAxisId,
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

        return series;
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

    private static void ApplyStreamingViewport( List<object> labels, List<RenderSeries> series, SvgChartStreamingOptions streaming, out int categorySlotCount, out List<int> categoryLabelIndexes )
    {
        categorySlotCount = labels.Count;
        categoryLabelIndexes = Enumerable.Range( 0, labels.Count ).ToList();

        if ( streaming is null || !streaming.Enabled || !streaming.VisibleDataPoints.HasValue )
            return;

        var visibleDataPoints = Math.Max( 1, streaming.VisibleDataPoints.Value );
        var renderedDataPoints = IsStreamingAnimationEnabled( streaming ) ? visibleDataPoints + 1 : visibleDataPoints;
        var startIndex = Math.Max( 0, labels.Count - renderedDataPoints );
        var visibleLabels = labels.Skip( startIndex ).Take( renderedDataPoints ).ToList();
        var visibleLabelIndexes = Enumerable.Range( startIndex, visibleLabels.Count ).ToList();
        var padCount = renderedDataPoints - visibleLabels.Count;
        categorySlotCount = visibleDataPoints;

        var reverse = IsStreamingReversed( streaming );

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
            return CreateStreamingOptions( streamingComponent, Streaming ?? ResolveOptions().Streaming );

        return Streaming ?? ResolveOptions().Streaming ?? new() { Enabled = false };
    }

    private static SvgChartStreamingOptions CreateStreamingOptions( SvgChartStreaming streaming, SvgChartStreamingOptions fallback )
    {
        if ( streaming is null )
            return fallback ?? new() { Enabled = false };

        return new()
        {
            Enabled = streaming.Enabled,
            MaxDataPoints = streaming.MaxDataPoints,
            VisibleDataPoints = streaming.VisibleDataPoints,
            Duration = streaming.Duration,
            IndexAxis = streaming.IndexAxis,
            Reverse = streaming.Reverse,
            Animation = streaming.Animation ?? fallback?.Animation ?? new(),
            RefreshInterval = streaming.RefreshInterval
        };
    }

    private List<RenderValueAxis> ResolveValueAxes( SvgChartOptions options, List<RenderSeries> series, SvgChartZoomOptions zoom, SvgChartViewport viewport )
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
                .SelectMany( x => IsPointChart( x.Type ) ? x.YValues : x.Values )
                .Where( x => x.HasValue )
                .Select( x => x.Value )
                .ToList();
            var scale = BuildScale( values, ApplyValueAxisViewport( axis, zoom, viewport, series.Any( x => x.Type == SvgChartType.Bar ) ) );

            return new RenderValueAxis
            {
                Id = axis.Id,
                Position = axis.Position,
                GridLines = axis.GridLines,
                Min = scale.Min,
                Max = scale.Max,
                Ticks = scale.Ticks
            };
        } ).ToList();
    }

    private static SvgChartAxisOptions CreateValueAxisOptions( SvgChartAxisOptions axis )
    {
        return new()
        {
            Id = axis.Id,
            Position = axis.Position,
            BeginAtZero = axis.BeginAtZero,
            Min = axis.Min,
            Max = axis.Max,
            TickCount = axis.TickCount,
            GridLines = CreateGridLinesOptions( axis.GridLines ),
            Labels = CreateLabelsOptions( axis.Labels ),
            Title = axis.Title
        };
    }

    private static SvgChartAxisOptions CreateCategoryAxisOptions( SvgChartAxisOptions options, SvgChartCategoryAxis<TItem> axis )
    {
        if ( axis is null )
            return CreateValueAxisOptions( options );

        return new()
        {
            Id = axis.Id,
            Position = axis.Position,
            BeginAtZero = options.BeginAtZero,
            Min = options.Min,
            Max = options.Max,
            TickCount = options.TickCount,
            GridLines = CreateGridLinesOptions( options.GridLines, axis.GridLines ),
            Labels = CreateLabelsOptions( options.Labels, axis.LabelsOptions ),
            Title = axis.Title
        };
    }

    private static SvgChartAxisOptions CreateValueAxisOptions( SvgChartValueAxis axis )
    {
        return new()
        {
            Id = axis.Id,
            Position = axis.Position,
            BeginAtZero = axis.BeginAtZero,
            Min = axis.Min,
            Max = axis.Max,
            TickCount = axis.TickCount,
            GridLines = CreateGridLinesOptions( axis.GridLines ),
            Labels = new(),
            Title = axis.Title
        };
    }

    private static SvgChartAxisLabelsOptions CreateLabelsOptions( SvgChartAxisLabelsOptions labels )
    {
        if ( labels is null )
            return null;

        return new()
        {
            Visible = labels.Visible,
            Step = labels.Step,
            Offset = labels.Offset
        };
    }

    private static SvgChartAxisLabelsOptions CreateLabelsOptions( SvgChartAxisLabelsOptions options, SvgChartAxisLabelsOptions overrides )
    {
        if ( overrides is null )
            return CreateLabelsOptions( options );

        return new()
        {
            Visible = overrides.Visible,
            Step = overrides.Step,
            Offset = overrides.Offset
        };
    }

    private static SvgChartGridLinesOptions CreateGridLinesOptions( SvgChartGridLinesOptions gridLines )
    {
        if ( gridLines is null )
            return null;

        return new()
        {
            Visible = gridLines.Visible,
            Color = gridLines.Color,
            Width = gridLines.Width,
            Opacity = gridLines.Opacity,
            DashPattern = gridLines.DashPattern
        };
    }

    private static SvgChartGridLinesOptions CreateGridLinesOptions( SvgChartGridLinesOptions options, SvgChartGridLinesOptions overrides )
    {
        if ( overrides is null )
            return CreateGridLinesOptions( options );

        return new()
        {
            Visible = overrides.Visible,
            Color = overrides.Color ?? options?.Color,
            Width = overrides.Width,
            Opacity = overrides.Opacity,
            DashPattern = overrides.DashPattern ?? options?.DashPattern
        };
    }

    private static RenderValueAxis ResolvePrimaryValueAxis( List<RenderValueAxis> axes, string valueAxisId )
    {
        return !string.IsNullOrWhiteSpace( valueAxisId )
            ? axes.LastOrDefault( x => string.Equals( x.Id, valueAxisId, StringComparison.Ordinal ) ) ?? axes.Last()
            : axes.Last();
    }

    private static bool BelongsToAxis( RenderSeries series, SvgChartAxisOptions axis, SvgChartAxisOptions defaultAxis )
    {
        if ( string.IsNullOrWhiteSpace( series.ValueAxisId ) )
            return ReferenceEquals( axis, defaultAxis );

        return string.Equals( series.ValueAxisId, axis.Id, StringComparison.Ordinal );
    }

    private SvgChartAxisOptions ResolveAxis( SvgChartOptions options, string valueAxisId = null )
    {
        var axisComponent = !string.IsNullOrWhiteSpace( valueAxisId )
            ? valueAxisComponents.LastOrDefault( x => string.Equals( x.Id, valueAxisId, StringComparison.Ordinal ) )
            : valueAxisComponents.LastOrDefault();
        axisComponent ??= valueAxisComponents.LastOrDefault();

        if ( axisComponent is null )
            return options.YAxis ?? new();

        return new()
        {
            Id = axisComponent.Id,
            Position = axisComponent.Position,
            BeginAtZero = axisComponent.BeginAtZero,
            Min = axisComponent.Min,
            Max = axisComponent.Max,
            TickCount = axisComponent.TickCount,
            Title = axisComponent.Title
        };
    }

    private SvgChartLegendOptions ResolveLegend( SvgChartOptions options )
    {
        var legendComponent = legendComponents.LastOrDefault();

        if ( legendComponent is null )
            return options.Legend ?? new();

        return new()
        {
            Visible = legendComponent.Visible,
            Position = legendComponent.Position
        };
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

    private SvgChartTextOptions ResolveTitleOptions( SvgChartOptions options )
    {
        var titleComponent = titleComponents.LastOrDefault();
        var resolved = CreateTextOptions( CreateDefaultTitleOptions(), options.Title );
        var componentOptions = titleComponent?.Title;

        if ( componentOptions is not null )
            resolved = CreateTextOptions( resolved, componentOptions );

        return resolved;
    }

    private SvgChartTextOptions ResolveSubtitleOptions( SvgChartOptions options )
    {
        var titleComponent = titleComponents.LastOrDefault();
        var resolved = CreateTextOptions( CreateDefaultSubtitleOptions(), options.Subtitle );
        var componentOptions = titleComponent?.Subtitle;

        if ( componentOptions is not null )
            resolved = CreateTextOptions( resolved, componentOptions );

        return resolved;
    }

    private static SvgChartTextOptions CreateDefaultTitleOptions()
    {
        return new()
        {
            Font = new()
            {
                Size = 16,
                Weight = "600",
            },
            Opacity = 1,
            Padding = new()
            {
                Top = 8,
            },
        };
    }

    private static SvgChartTextOptions CreateDefaultSubtitleOptions()
    {
        return new()
        {
            Font = new()
            {
                Size = 12,
            },
            Opacity = 0.7,
            Padding = new()
            {
                Top = 7,
            },
        };
    }

    private static PlotArea BuildPlotArea( SvgChartOptions options, SvgChartTextOptions title, SvgChartTextOptions subtitle, bool hasTopLegend, bool hasBottomLegend )
    {
        var top = 24d + GetTopTextHeight( title ) + GetTopTextHeight( subtitle );

        if ( hasTopLegend )
            top += 28;

        var bottom = options.Height - 42 - ( hasBottomLegend ? 38 : 0 ) - GetBottomTextHeight( title ) - GetBottomTextHeight( subtitle );
        var left = 52d + GetStartTextWidth( title ) + GetStartTextWidth( subtitle );
        var right = options.Width - 18 - GetEndTextWidth( title ) - GetEndTextWidth( subtitle );

        return new()
        {
            Left = left,
            Top = top,
            Right = right,
            Bottom = bottom
        };
    }

    private static SvgChartTextOptions CreateTextOptions( SvgChartTextOptions options )
    {
        if ( options is null )
            return new() { Visible = false };

        return new()
        {
            Visible = options.Visible,
            Text = options.Text,
            Position = options.Position,
            Alignment = options.Alignment,
            Padding = CreateSpacing( options.Padding ),
            Font = CreateFontOptions( options.Font ),
            Opacity = options.Opacity
        };
    }

    private static SvgChartTextOptions CreateTextOptions( SvgChartTextOptions options, SvgChartTextOptions overrides )
    {
        if ( overrides is null )
            return CreateTextOptions( options );

        return new()
        {
            Visible = overrides.Visible,
            Text = overrides.Text ?? options?.Text,
            Position = overrides.Position,
            Alignment = overrides.Alignment,
            Padding = CreateSpacing( overrides.Padding ?? options?.Padding ),
            Font = CreateFontOptions( options?.Font, overrides.Font ),
            Opacity = overrides.Opacity ?? options?.Opacity
        };
    }

    private static SvgChartFontOptions CreateFontOptions( SvgChartFontOptions options )
    {
        if ( options is null )
            return null;

        return new()
        {
            Family = options.Family,
            Size = options.Size,
            Weight = options.Weight,
            Color = options.Color
        };
    }

    private static SvgChartFontOptions CreateFontOptions( SvgChartFontOptions options, SvgChartFontOptions overrides )
    {
        if ( overrides is null )
            return CreateFontOptions( options );

        return new()
        {
            Family = overrides.Family ?? options?.Family,
            Size = overrides.Size ?? options?.Size,
            Weight = overrides.Weight ?? options?.Weight,
            Color = IsDefaultColor( overrides.Color ) ? options?.Color : overrides.Color
        };
    }

    private static SvgChartSpacing CreateSpacing( SvgChartSpacing spacing )
    {
        if ( spacing is null )
            return new();

        return new()
        {
            Top = spacing.Top,
            End = spacing.End,
            Bottom = spacing.Bottom,
            Start = spacing.Start
        };
    }

    private static bool IsTextVisible( SvgChartTextOptions text )
    {
        return text?.Visible == true && !string.IsNullOrWhiteSpace( text.Text );
    }

    private static double GetTopTextHeight( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.Top
            ? GetTextBlockHeight( text )
            : 0;
    }

    private static double GetBottomTextHeight( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.Bottom
            ? GetTextBlockHeight( text )
            : 0;
    }

    private static double GetStartTextWidth( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.Start
            ? GetTextBlockWidth( text )
            : 0;
    }

    private static double GetEndTextWidth( SvgChartTextOptions text )
    {
        return IsTextVisible( text ) && text.Position == SvgChartTextPosition.End
            ? GetTextBlockWidth( text )
            : 0;
    }

    private static double GetTextBlockHeight( SvgChartTextOptions text )
    {
        var padding = text.Padding ?? new();
        var fontSize = text.Font?.Size ?? 12;

        return padding.Top + fontSize + padding.Bottom;
    }

    private static double GetTextBlockWidth( SvgChartTextOptions text )
    {
        var padding = text.Padding ?? new();
        var fontSize = text.Font?.Size ?? 12;

        return padding.Start + fontSize + padding.End;
    }

    private static (double Min, double Max) ResolveCategoryRange( int labelCount, SvgChartZoomOptions zoom, SvgChartViewport viewport )
    {
        var min = -0.5;
        var max = Math.Max( 0.5, labelCount - 0.5 );

        if ( zoom?.Enabled != true || !SupportsHorizontalZoom( zoom.Mode ) )
            return (min, max);

        return NormalizeViewportRange( viewport?.XMin, viewport?.XMax, min, max, zoom.MinZoom, zoom.MaxZoom );
    }

    private static SvgChartAxisOptions ApplyValueAxisViewport( SvgChartAxisOptions axis, SvgChartZoomOptions zoom, SvgChartViewport viewport, bool horizontalValueAxis )
    {
        if ( zoom?.Enabled != true || viewport is null )
            return axis;

        var supportsAxis = horizontalValueAxis
            ? SupportsHorizontalZoom( zoom.Mode )
            : SupportsVerticalZoom( zoom.Mode );

        if ( !supportsAxis )
            return axis;

        var viewportMin = horizontalValueAxis ? viewport.XMin : viewport.YMin;
        var viewportMax = horizontalValueAxis ? viewport.XMax : viewport.YMax;

        if ( !viewportMin.HasValue || !viewportMax.HasValue || viewportMax <= viewportMin )
            return axis;

        var result = CreateValueAxisOptions( axis );
        result.Min = viewportMin;
        result.Max = viewportMax;

        return result;
    }

    private static SvgChartAxisOptions ApplyPointXAxisViewport( SvgChartAxisOptions axis, SvgChartZoomOptions zoom, SvgChartViewport viewport )
    {
        if ( zoom?.Enabled != true || !SupportsHorizontalZoom( zoom.Mode ) || viewport?.XMin is null || viewport.XMax is null || viewport.XMax <= viewport.XMin )
            return axis;

        var result = CreateValueAxisOptions( axis );
        result.Min = viewport.XMin;
        result.Max = viewport.XMax;

        return result;
    }

    private static Scale BuildScale( List<double> values, SvgChartAxisOptions axis )
    {
        var min = axis.Min ?? ( values.Count == 0 ? 0 : values.Min() );
        var max = axis.Max ?? ( values.Count == 0 ? 1 : values.Max() );

        if ( axis.BeginAtZero )
        {
            min = Math.Min( min, 0 );
            max = Math.Max( max, 0 );
        }

        if ( Math.Abs( max - min ) < double.Epsilon )
        {
            max += 1;
            min -= 1;
        }

        var tickCount = Math.Max( 2, axis.TickCount );
        var step = NiceNumber( ( max - min ) / ( tickCount - 1 ), true );
        var niceMin = axis.Min ?? Math.Floor( min / step ) * step;
        var niceMax = axis.Max ?? Math.Ceiling( max / step ) * step;
        var ticks = new List<double>();

        for ( var tick = niceMin; tick <= niceMax + step / 2; tick += step )
            ticks.Add( Math.Abs( tick ) < step / 1000000 ? 0 : tick );

        return new()
        {
            Min = niceMin,
            Max = niceMax,
            Ticks = ticks
        };
    }

    private static (double Min, double Max) NormalizeViewportRange( double? requestedMin, double? requestedMax, double fullMin, double fullMax, double minZoom, double maxZoom )
    {
        if ( !requestedMin.HasValue || !requestedMax.HasValue || requestedMax <= requestedMin )
            return (fullMin, fullMax);

        return ClampRange( requestedMin.Value, requestedMax.Value, fullMin, fullMax, minZoom, maxZoom );
    }

    private static (double Min, double Max) ClampRange( double min, double max, double fullMin, double fullMax, double minZoom, double maxZoom )
    {
        var fullRange = fullMax - fullMin;

        if ( fullRange <= 0 )
            return (fullMin, fullMax);

        var requestedRange = Math.Max( double.Epsilon, max - min );
        var minimumZoom = Math.Max( 1, minZoom );
        var maximumZoom = Math.Max( minimumZoom, maxZoom );
        var maxRange = fullRange / minimumZoom;
        var minRange = fullRange / maximumZoom;
        var range = Math.Clamp( requestedRange, minRange, maxRange );
        var center = min + requestedRange / 2;
        var resultMin = center - range / 2;
        var resultMax = center + range / 2;

        if ( resultMin < fullMin )
        {
            resultMax += fullMin - resultMin;
            resultMin = fullMin;
        }

        if ( resultMax > fullMax )
        {
            resultMin -= resultMax - fullMax;
            resultMax = fullMax;
        }

        return (Math.Max( fullMin, resultMin ), Math.Min( fullMax, resultMax ));
    }

    private static bool SupportsHorizontalZoom( SvgChartZoomMode mode )
    {
        return mode is SvgChartZoomMode.X or SvgChartZoomMode.XY;
    }

    private static bool SupportsVerticalZoom( SvgChartZoomMode mode )
    {
        return mode is SvgChartZoomMode.Y or SvgChartZoomMode.XY;
    }

    private static SvgChartViewport CloneViewport( SvgChartViewport viewport )
    {
        if ( viewport is null )
            return null;

        return new()
        {
            XMin = viewport.XMin,
            XMax = viewport.XMax,
            YMin = viewport.YMin,
            YMax = viewport.YMax
        };
    }

    private static double NiceNumber( double range, bool round )
    {
        if ( range <= 0 || double.IsNaN( range ) || double.IsInfinity( range ) )
            return 1;

        var exponent = Math.Floor( Math.Log10( range ) );
        var fraction = range / Math.Pow( 10, exponent );
        double niceFraction;

        if ( round )
        {
            niceFraction = fraction switch
            {
                < 1.5 => 1,
                < 3 => 2,
                < 7 => 5,
                _ => 10
            };
        }
        else
        {
            niceFraction = fraction switch
            {
                <= 1 => 1,
                <= 2 => 2,
                <= 5 => 5,
                _ => 10
            };
        }

        return niceFraction * Math.Pow( 10, exponent );
    }

    private static double GetY( double value, PlotArea plot, RenderModel model )
    {
        return GetY( value, plot, model.PrimaryValueAxis );
    }

    private static double GetY( double value, PlotArea plot, RenderModel model, RenderSeries series )
    {
        return GetY( value, plot, ResolveValueAxis( model, series ) );
    }

    private static double GetY( double value, PlotArea plot, RenderValueAxis axis )
    {
        var range = axis.Max - axis.Min;

        if ( range <= 0 )
            return plot.Bottom;

        return plot.Bottom - ( value - axis.Min ) / range * plot.Height;
    }

    private static double GetX( double value, PlotArea plot, RenderModel model )
    {
        return GetX( value, plot, model.PrimaryValueAxis.Min, model.PrimaryValueAxis.Max );
    }

    private static double GetX( double value, PlotArea plot, RenderModel model, RenderSeries series )
    {
        var axis = ResolveValueAxis( model, series );

        return GetX( value, plot, axis.Min, axis.Max );
    }

    private static double GetX( double value, PlotArea plot, double min, double max )
    {
        var range = max - min;

        if ( range <= 0 )
            return plot.Left;

        return plot.Left + ( value - min ) / range * plot.Width;
    }

    private static double GetCategoryX( int index, PlotArea plot, RenderModel model )
    {
        return GetX( index, plot, model.CategoryMin, model.CategoryMax );
    }

    private static double GetCategoryBoundaryX( double index, PlotArea plot, RenderModel model )
    {
        return GetX( index - 0.5, plot, model.CategoryMin, model.CategoryMax );
    }

    private static double GetCategoryWidth( PlotArea plot, RenderModel model )
    {
        var range = model.CategoryMax - model.CategoryMin;

        if ( range <= 0 )
            return plot.Width;

        return plot.Width / range;
    }

    private static Scale ResolvePointXScale( RenderModel model, IEnumerable<RenderSeries> pointSeries = null )
    {
        var series = pointSeries?.ToList()
            ?? model.Series.Where( x => !x.Hidden && IsPointChart( x.Type ) ).ToList();

        if ( series.Count == 0 )
            return null;

        var axis = ApplyPointXAxisViewport( model.Options.XAxis ?? new(), model.Zoom, model.Viewport );

        return BuildScale( series.SelectMany( x => x.XValues ).Where( x => x.HasValue ).Select( x => x.Value ).ToList(), axis );
    }

    private static RenderValueAxis ResolveValueAxis( RenderModel model, RenderSeries series )
    {
        if ( string.IsNullOrWhiteSpace( series.ValueAxisId ) )
            return model.PrimaryValueAxis;

        return model.ValueAxes.LastOrDefault( x => string.Equals( x.Id, series.ValueAxisId, StringComparison.Ordinal ) ) ?? model.PrimaryValueAxis;
    }

    private static RenderValueAxis ResolveValueAxis( RenderModel model, string valueAxisId )
    {
        if ( string.IsNullOrWhiteSpace( valueAxisId ) )
            return model.PrimaryValueAxis;

        return model.ValueAxes.LastOrDefault( x => string.Equals( x.Id, valueAxisId, StringComparison.Ordinal ) ) ?? model.PrimaryValueAxis;
    }

    private static string BuildLinePath( List<(int Index, double X, double Y, double Value)> points )
    {
        var builder = new StringBuilder();

        for ( var i = 0; i < points.Count; i++ )
        {
            builder.Append( i == 0 ? "M " : " L " );
            builder.Append( Format( points[i].X ) );
            builder.Append( ' ' );
            builder.Append( Format( points[i].Y ) );
        }

        return builder.ToString();
    }

    private static double ResolveAnnotationX( RenderModel model, PlotArea plot, double? value, Scale pointChartScale, double fallback )
    {
        if ( !value.HasValue )
            return fallback;

        if ( pointChartScale is not null )
            return GetX( value.Value, plot, pointChartScale.Min, pointChartScale.Max );

        return GetX( value.Value, plot, model.CategoryMin, model.CategoryMax );
    }

    private static double ResolveAnnotationY( RenderModel model, PlotArea plot, double? value, string valueAxisId, double fallback )
    {
        if ( !value.HasValue )
            return fallback;

        return GetY( value.Value, plot, ResolveValueAxis( model, valueAxisId ) );
    }

    private static string BuildArcPath( double centerX, double centerY, double innerRadius, double outerRadius, double startAngle, double endAngle )
    {
        var sweep = endAngle - startAngle;

        if ( sweep >= Math.PI * 2 )
            endAngle = startAngle + Math.PI * 2 - 0.0001;

        var largeArc = sweep > Math.PI ? 1 : 0;
        var outerStart = PolarToCartesian( centerX, centerY, outerRadius, startAngle );
        var outerEnd = PolarToCartesian( centerX, centerY, outerRadius, endAngle );

        if ( innerRadius <= 0 )
        {
            return $"M {Format( centerX )} {Format( centerY )} L {Format( outerStart.X )} {Format( outerStart.Y )} A {Format( outerRadius )} {Format( outerRadius )} 0 {largeArc} 1 {Format( outerEnd.X )} {Format( outerEnd.Y )} Z";
        }

        var innerEnd = PolarToCartesian( centerX, centerY, innerRadius, endAngle );
        var innerStart = PolarToCartesian( centerX, centerY, innerRadius, startAngle );

        return $"M {Format( outerStart.X )} {Format( outerStart.Y )} A {Format( outerRadius )} {Format( outerRadius )} 0 {largeArc} 1 {Format( outerEnd.X )} {Format( outerEnd.Y )} L {Format( innerEnd.X )} {Format( innerEnd.Y )} A {Format( innerRadius )} {Format( innerRadius )} 0 {largeArc} 0 {Format( innerStart.X )} {Format( innerStart.Y )} Z";
    }

    private static string BuildRadarPoints( List<double?> values, double centerX, double centerY, double radius, double max )
    {
        var builder = new StringBuilder();
        var count = values.Count;

        if ( count == 0 || max <= 0 )
            return string.Empty;

        for ( var i = 0; i < count; i++ )
        {
            var value = Math.Max( values[i] ?? 0, 0 );
            var angle = -Math.PI / 2 + Math.PI * 2 * i / count;
            var point = PolarToCartesian( centerX, centerY, radius * value / max, angle );

            if ( i > 0 )
                builder.Append( ' ' );

            builder.Append( Format( point.X ) );
            builder.Append( ',' );
            builder.Append( Format( point.Y ) );
        }

        return builder.ToString();
    }

    private static (double X, double Y) PolarToCartesian( double centerX, double centerY, double radius, double angle )
    {
        return (centerX + radius * Math.Cos( angle ), centerY + radius * Math.Sin( angle ));
    }

    private static bool IsBarChart( RenderModel model )
    {
        return model.Series.Any( x => x.Type == SvgChartType.Bar );
    }

    private static bool IsRadialChart( RenderModel model )
    {
        return model.Series.Any( x => IsRadialChart( x.Type ) );
    }

    private static bool IsRadialChart( SvgChartType chartType )
    {
        return chartType is SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea or SvgChartType.Radar;
    }

    private static bool IsRadialCategoryLegendChart( RenderModel model )
    {
        return model.Series.Any( x => IsRadialCategoryLegendChart( x.Type ) );
    }

    private static bool IsRadialCategoryLegendChart( SvgChartType chartType )
    {
        return chartType is SvgChartType.Pie or SvgChartType.Doughnut or SvgChartType.PolarArea;
    }

    private static bool IsPointChart( SvgChartType chartType )
    {
        return chartType == SvgChartType.Scatter || chartType == SvgChartType.Bubble;
    }

    private static bool SupportsStreamingAnimation( RenderModel model )
    {
        return model.Series.Any( x => !x.Hidden && x.Type is SvgChartType.Column or SvgChartType.Line or SvgChartType.Area );
    }

    private static bool IsStreamingReversed( SvgChartStreamingOptions streaming )
    {
        return streaming?.Reverse == true;
    }

    private static bool IsStreamingAnimationEnabled( SvgChartStreamingOptions streaming )
    {
        return streaming?.Animation?.Enabled == true;
    }

    private static int GetCategorySlotCount( RenderModel model )
    {
        return Math.Max( model.CategorySlotCount > 0 ? model.CategorySlotCount : model.Labels.Count, 1 );
    }

    private static int ResolveRenderOrder( RenderSeries series )
    {
        if ( series.Order.HasValue )
            return series.Order.Value;

        return series.Type switch
        {
            SvgChartType.Area => 0,
            SvgChartType.Column or SvgChartType.Bar => 10,
            SvgChartType.Line => 20,
            SvgChartType.Scatter or SvgChartType.Bubble => 30,
            _ => 0
        };
    }

    private SvgChartPointEventArgs CreatePointArgs( RenderModel model, RenderSeries series, int pointIndex, double value, SvgChartPointBounds bounds )
    {
        return new()
        {
            SeriesName = series.Name,
            SeriesIndex = model.Series.IndexOf( series ),
            PointIndex = pointIndex,
            Category = pointIndex >= 0 && pointIndex < model.Labels.Count ? model.Labels[pointIndex] : null,
            Value = value,
            Bounds = bounds
        };
    }

    private async Task HandlePointClicked( SvgChartPointEventArgs point, string color )
    {
        ShowTooltip( point, color, true );

        await Clicked.InvokeAsync( point );
    }

    private async Task HandlePointHovered( SvgChartPointEventArgs point, string color )
    {
        ShowTooltip( point, color, false );

        await Hovered.InvokeAsync( point );
    }

    private async Task HandleDataLabelClicked( SvgChartPointEventArgs point, string color )
    {
        var dataLabelsComponent = pluginComponents.OfType<SvgChartDataLabels>().LastOrDefault();

        ShowTooltip( point, color, true );

        if ( dataLabelsComponent is not null )
            await dataLabelsComponent.Clicked.InvokeAsync( point );

        await Clicked.InvokeAsync( point );
    }

    private async Task HandleDataLabelHovered( SvgChartPointEventArgs point, string color )
    {
        var dataLabelsComponent = pluginComponents.OfType<SvgChartDataLabels>().LastOrDefault();

        ShowTooltip( point, color, false );

        if ( dataLabelsComponent is not null )
            await dataLabelsComponent.Hovered.InvokeAsync( point );

        await Hovered.InvokeAsync( point );
    }

    private void HandlePointLeft()
    {
        if ( activeTooltipPinned )
            return;

        activeTooltip = null;
    }

    private void ShowTooltip( SvgChartPointEventArgs point, string color, bool pinned )
    {
        var options = ResolveOptions();
        var tooltip = ResolveTooltip( options );

        if ( !tooltip.Enabled )
            return;

        activeTooltip = BuildTooltipContext( point, color, options, tooltip );
        activeTooltipPinned = pinned;
    }

    private static SvgChartTooltipContext BuildTooltipContext( SvgChartPointEventArgs point, string color, SvgChartOptions options, SvgChartTooltipOptions tooltip )
    {
        var anchorX = point.Bounds.X + point.Bounds.Width / 2;
        var anchorY = point.Bounds.Y;
        var width = Math.Max( 1, tooltip.Width );
        var height = Math.Max( 1, tooltip.Height );
        var x = Math.Min( Math.Max( anchorX + tooltip.OffsetX, 0 ), Math.Max( options.Width - width, 0 ) );
        var y = Math.Min( Math.Max( anchorY - height - tooltip.OffsetY, 0 ), Math.Max( options.Height - height, 0 ) );

        return new()
        {
            SeriesName = point.SeriesName,
            SeriesIndex = point.SeriesIndex,
            PointIndex = point.PointIndex,
            Category = point.Category,
            Value = point.Value,
            Bounds = point.Bounds,
            Color = color,
            Text = GetPointLabel( point ),
            X = x,
            Y = y,
            Width = width,
            Height = height,
            Point = point
        };
    }

    private static string GetPointLabel( SvgChartPointEventArgs point )
    {
        return $"{point.Category}, {point.Value}. {point.SeriesName}.";
    }

    private static string ResolveColor( Color color, int index )
    {
        if ( color is null || color == Color.Default || string.IsNullOrWhiteSpace( color.Name ) )
            return Palette[index % Palette.Length];

        var name = color.Name;
        var fallback = ResolveColorFallback( name, index );

        if ( name.StartsWith( "#", StringComparison.Ordinal )
             || name.StartsWith( "rgb", StringComparison.OrdinalIgnoreCase )
             || name.StartsWith( "hsl", StringComparison.OrdinalIgnoreCase )
             || name.StartsWith( "var(", StringComparison.OrdinalIgnoreCase ) )
            return name;

        return $"var(--b-theme-{name}, var(--bs-{name}, {fallback}))";
    }

    private static string ResolveColorFallback( string name, int index )
    {
        return name?.ToLowerInvariant() switch
        {
            "primary" => Palette[0],
            "secondary" => "#868e96",
            "success" => Palette[1],
            "danger" => Palette[3],
            "warning" => Palette[2],
            "info" => Palette[5],
            "light" => "#f8f9fa",
            "dark" => "#343a40",
            "link" => Palette[0],
            _ => Palette[index % Palette.Length]
        };
    }

    private static string ResolveGridLineColor( SvgChartGridLinesOptions gridLines )
    {
        var color = gridLines?.Color;

        if ( color is null || color == Color.Default || string.IsNullOrWhiteSpace( color.Name ) )
            return "currentColor";

        return ResolveColor( color, 0 );
    }

    private static double ResolveTextX( SvgChartOptions options, SvgChartTextOptions text, double start, double end, double fontSize )
    {
        var padding = text.Padding ?? new();

        return text.Position switch
        {
            SvgChartTextPosition.Start => start + padding.Start + fontSize,
            SvgChartTextPosition.End => options.Width - end - padding.End - fontSize,
            _ => padding.Start + ResolveTextAlignmentOffset( options.Width - padding.Start - padding.End, text.Alignment )
        };
    }

    private static double ResolveTextY( SvgChartOptions options, SvgChartTextOptions text, double top, double bottom, double fontSize )
    {
        var padding = text.Padding ?? new();

        return text.Position switch
        {
            SvgChartTextPosition.Bottom => options.Height - bottom - padding.Bottom,
            SvgChartTextPosition.Start => options.Height - padding.Bottom - ResolveTextAlignmentOffset( options.Height - padding.Top - padding.Bottom, text.Alignment ),
            SvgChartTextPosition.End => padding.Top + ResolveTextAlignmentOffset( options.Height - padding.Top - padding.Bottom, text.Alignment ),
            _ => top + padding.Top + fontSize
        };
    }

    private static double ResolveTextAlignmentOffset( double size, SvgChartTextAlignment alignment )
    {
        return alignment switch
        {
            SvgChartTextAlignment.Start => 0,
            SvgChartTextAlignment.End => size,
            _ => size / 2
        };
    }

    private static string ResolveTextAnchor( SvgChartTextAlignment alignment )
    {
        return alignment switch
        {
            SvgChartTextAlignment.Start => "start",
            SvgChartTextAlignment.End => "end",
            _ => "middle"
        };
    }

    private static void AddFontAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartOptions options, double fallbackSize = 11, double? opacity = null )
    {
        var font = options?.Font;

        builder.AddAttribute( sequence++, "font-size", Format( font?.Size ?? fallbackSize ) );
        AddFontFamilyAttribute( builder, ref sequence, font?.Family );
        builder.AddAttribute( sequence++, "fill", ResolveFontColor( font ) );

        if ( !string.IsNullOrWhiteSpace( font?.Weight ) )
            builder.AddAttribute( sequence++, "font-weight", font.Weight );

        if ( opacity is not null )
            builder.AddAttribute( sequence++, "opacity", Format( Math.Clamp( opacity.Value, 0, 1 ) ) );
    }

    private static void AddFontFamilyAttribute( RenderTreeBuilder builder, ref int sequence, string fontFamily )
    {
        if ( !string.IsNullOrWhiteSpace( fontFamily ) )
            builder.AddAttribute( sequence++, "font-family", fontFamily );
    }

    private static string ResolveTooltipStyle( SvgChartOptions options, SvgChartTooltipContext context )
    {
        var font = options?.Font;
        var color = IsDefaultColor( font?.Color )
            ? "var(--b-tooltip-color,#fff)"
            : ResolveFontColor( font );
        var fontSize = font?.Size is null
            ? "var(--b-tooltip-font-size,.875rem)"
            : $"{Format( font.Size.Value )}px";
        var fontFamily = string.IsNullOrWhiteSpace( font?.Family )
            ? null
            : $"font-family:{font.Family};";

        return $"height:100%;box-sizing:border-box;overflow:hidden;border-left:3px solid {context.Color};background:rgba(var(--b-tooltip-background-color-r,33),var(--b-tooltip-background-color-g,37),var(--b-tooltip-background-color-b,41),var(--b-tooltip-background-opacity,.94));color:{color};border-radius:var(--b-tooltip-border-radius,.375rem);padding:var(--b-tooltip-padding,.5rem .75rem);font-size:{fontSize};{fontFamily}box-shadow:0 .35rem 1rem rgba(0,0,0,.18);";
    }

    private static string ResolveTextColor( SvgChartOptions options, SvgChartTextOptions text )
    {
        var textColor = text?.Font?.Color;
        return ResolveFontColor( IsDefaultColor( textColor ) ? options?.Font?.Color : textColor );
    }

    private static string ResolveFontColor( SvgChartFontOptions font )
    {
        return ResolveFontColor( font?.Color );
    }

    private static string ResolveFontColor( Color color )
    {
        if ( IsDefaultColor( color ) )
            return "currentColor";

        return ResolveColor( color, 0 );
    }

    private static bool IsDefaultColor( Color color )
    {
        return color is null || color == Color.Default || string.IsNullOrWhiteSpace( color.Name );
    }

    private static string FormatTick( double value )
    {
        return value.ToString( "0.##", CultureInfo.InvariantCulture );
    }

    private static string Format( double value )
    {
        return value.ToString( "0.###", CultureInfo.InvariantCulture );
    }

    private static string FormatDuration( TimeSpan value )
    {
        return $"{value.TotalSeconds.ToString( "0.###", CultureInfo.InvariantCulture )}s";
    }

    private string ResolveStreamingAnimationStyle( (bool Enabled, double OffsetX, TimeSpan Duration) animation )
    {
        var offsetX = streamingAnimationActive ? animation.OffsetX : 0;
        var transition = streamingAnimationActive
            ? $"transition:transform {FormatDuration( animation.Duration )} linear;"
            : "transition:none;";

        return $"transform:translateX({Format( offsetX )}px);{transition}";
    }

    private string GetPlotClipPathId()
    {
        return $"{ElementId}-plot-clip";
    }

    private string GetCategoryAxisLabelsClipPathId()
    {
        return $"{ElementId}-category-axis-labels-clip";
    }

    private (bool Enabled, double OffsetX, TimeSpan Duration) ResolveStreamingAnimation( RenderModel model, PlotArea plot )
    {
        var streaming = ResolveStreaming();

        if ( streaming is null
             || !streaming.Enabled
             || !IsStreamingAnimationEnabled( streaming )
             || !streaming.VisibleDataPoints.HasValue
             || streaming.VisibleDataPoints.Value <= 0
             || model.Labels.Count == 0
             || !SupportsStreamingAnimation( model ) )
            return (false, 0, TimeSpan.Zero);

        var duration = ResolveStreamingAnimationDuration( streaming );

        if ( duration <= TimeSpan.Zero )
            return (false, 0, TimeSpan.Zero);

        var categoryWidth = GetCategoryWidth( plot, model );
        var offsetX = IsStreamingReversed( streaming )
            ? categoryWidth
            : -categoryWidth;

        if ( streaming.IndexAxis == SvgChartIndexAxis.Y )
        {
            offsetX = -offsetX;
        }

        return (true, offsetX, duration);
    }

    private static TimeSpan ResolveStreamingAnimationDuration( SvgChartStreamingOptions streaming )
    {
        if ( streaming.Animation?.Duration > TimeSpan.Zero )
            return streaming.Animation.Duration;

        if ( streaming.RefreshInterval > TimeSpan.Zero )
            return streaming.RefreshInterval;

        return TimeSpan.FromMilliseconds( 500 );
    }

    private PlotArea BuildCurrentPlotArea( RenderModel model )
    {
        var options = model.Options;
        var legend = ResolveLegend( options );
        var title = ResolveTitleOptions( options );
        var subtitle = ResolveSubtitleOptions( options );

        return BuildPlotArea( options, title, subtitle, legend.Visible && legend.Position == SvgChartLegendPosition.Top, legend.Visible && legend.Position == SvgChartLegendPosition.Bottom );
    }

    private static bool IsInsidePlot( double x, double y, PlotArea plot )
    {
        return x >= plot.Left && x <= plot.Right && y >= plot.Top && y <= plot.Bottom;
    }

    private static SvgChartViewport ResolveFullViewport( RenderModel model )
    {
        return new()
        {
            XMin = ResolveFullViewportXMin( model ),
            XMax = ResolveFullViewportXMax( model ),
            YMin = model.PrimaryValueAxis.Min,
            YMax = model.PrimaryValueAxis.Max
        };
    }

    private static double ResolveFullViewportXMin( RenderModel model )
    {
        var pointScale = ResolvePointXScale( model );

        if ( pointScale is not null )
            return pointScale.Min;

        if ( IsBarChart( model ) )
            return model.PrimaryValueAxis.Min;

        return -0.5;
    }

    private static double ResolveFullViewportXMax( RenderModel model )
    {
        var pointScale = ResolvePointXScale( model );

        if ( pointScale is not null )
            return pointScale.Max;

        if ( IsBarChart( model ) )
            return model.PrimaryValueAxis.Max;

        return Math.Max( 0.5, model.Labels.Count - 0.5 );
    }

    private static SvgChartViewport ResolveEffectiveViewport( RenderModel model, SvgChartViewport fullViewport )
    {
        var viewport = model.Viewport;

        return new()
        {
            XMin = viewport?.XMin ?? fullViewport.XMin,
            XMax = viewport?.XMax ?? fullViewport.XMax,
            YMin = viewport?.YMin ?? fullViewport.YMin,
            YMax = viewport?.YMax ?? fullViewport.YMax
        };
    }

    private static SvgChartViewport ZoomViewport( SvgChartViewport viewport, SvgChartViewport fullViewport, SvgChartZoomOptions zoom, PlotArea plot, double anchorX, double anchorY, double factor )
    {
        var result = CloneViewport( viewport );

        if ( SupportsHorizontalZoom( zoom.Mode ) )
        {
            var range = viewport.XMax.Value - viewport.XMin.Value;
            var anchor = viewport.XMin.Value + range * Math.Clamp( ( anchorX - plot.Left ) / plot.Width, 0, 1 );
            var min = anchor - ( anchor - viewport.XMin.Value ) * factor;
            var max = anchor + ( viewport.XMax.Value - anchor ) * factor;
            var clamped = ClampRange( min, max, fullViewport.XMin.Value, fullViewport.XMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.XMin = clamped.Min;
            result.XMax = clamped.Max;
        }

        if ( SupportsVerticalZoom( zoom.Mode ) )
        {
            var range = viewport.YMax.Value - viewport.YMin.Value;
            var anchor = viewport.YMax.Value - range * Math.Clamp( ( anchorY - plot.Top ) / plot.Height, 0, 1 );
            var min = anchor - ( anchor - viewport.YMin.Value ) * factor;
            var max = anchor + ( viewport.YMax.Value - anchor ) * factor;
            var clamped = ClampRange( min, max, fullViewport.YMin.Value, fullViewport.YMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.YMin = clamped.Min;
            result.YMax = clamped.Max;
        }

        return result;
    }

    private static SvgChartViewport PanViewport( SvgChartViewport viewport, SvgChartViewport fullViewport, SvgChartZoomOptions zoom, PlotArea plot, double deltaX, double deltaY )
    {
        var result = CloneViewport( viewport );

        if ( SupportsHorizontalZoom( zoom.Mode ) )
        {
            var range = viewport.XMax.Value - viewport.XMin.Value;
            var delta = -deltaX / plot.Width * range;
            var clamped = ClampRange( viewport.XMin.Value + delta, viewport.XMax.Value + delta, fullViewport.XMin.Value, fullViewport.XMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.XMin = clamped.Min;
            result.XMax = clamped.Max;
        }

        if ( SupportsVerticalZoom( zoom.Mode ) )
        {
            var range = viewport.YMax.Value - viewport.YMin.Value;
            var delta = deltaY / plot.Height * range;
            var clamped = ClampRange( viewport.YMin.Value + delta, viewport.YMax.Value + delta, fullViewport.YMin.Value, fullViewport.YMax.Value, zoom.MinZoom, zoom.MaxZoom );
            result.YMin = clamped.Min;
            result.YMax = clamped.Max;
        }

        return result;
    }

    private async Task ApplyViewport( SvgChartViewport previousViewport, SvgChartViewport viewport )
    {
        internalViewport = CloneViewport( viewport );
        activeTooltip = null;

        var zoomComponent = pluginComponents.OfType<SvgChartZoom>().LastOrDefault();

        if ( zoomComponent is not null && zoomComponent.ViewportChanged.HasDelegate )
            await zoomComponent.ViewportChanged.InvokeAsync( CloneViewport( viewport ) );

        StateHasChanged();
    }

    private async Task NotifyZoomed( SvgChartViewport previousViewport, SvgChartViewport viewport, SvgChartZoomSource source )
    {
        var zoomComponent = pluginComponents.OfType<SvgChartZoom>().LastOrDefault();

        if ( zoomComponent is null || !zoomComponent.Zoomed.HasDelegate )
            return;

        await zoomComponent.Zoomed.InvokeAsync( new()
        {
            PreviousViewport = CloneViewport( previousViewport ),
            Viewport = CloneViewport( viewport ),
            Source = source
        } );
    }

    private async Task NotifyPanned( SvgChartViewport previousViewport, SvgChartViewport viewport )
    {
        var zoomComponent = pluginComponents.OfType<SvgChartZoom>().LastOrDefault();

        if ( zoomComponent is null || !zoomComponent.Panned.HasDelegate )
            return;

        await zoomComponent.Panned.InvokeAsync( new()
        {
            PreviousViewport = CloneViewport( previousViewport ),
            Viewport = CloneViewport( viewport ),
            DeltaX = ( viewport.XMin ?? 0 ) - ( previousViewport.XMin ?? 0 ),
            DeltaY = ( viewport.YMin ?? 0 ) - ( previousViewport.YMin ?? 0 )
        } );
    }

    private async Task HandleChartWheel( WheelEventArgs eventArgs )
    {
        var model = BuildModel();
        var zoom = model.Zoom;

        if ( zoom?.Enabled != true || !zoom.Wheel || IsRadialChart( model ) )
            return;

        var plot = BuildCurrentPlotArea( model );

        if ( !IsInsidePlot( eventArgs.OffsetX, eventArgs.OffsetY, plot ) )
            return;

        var fullViewport = ResolveFullViewport( BuildModel( false, false ) );
        var previousViewport = ResolveEffectiveViewport( model, fullViewport );
        var factor = eventArgs.DeltaY < 0 ? 0.82 : 1.22;
        var nextViewport = ZoomViewport( previousViewport, fullViewport, zoom, plot, eventArgs.OffsetX, eventArgs.OffsetY, factor );

        await ApplyViewport( previousViewport, nextViewport );
        await NotifyZoomed( previousViewport, nextViewport, SvgChartZoomSource.Wheel );
    }

    private void HandleChartMouseDown( MouseEventArgs eventArgs )
    {
        var model = BuildModel();
        var zoom = model.Zoom;

        if ( zoom?.Enabled != true || !zoom.Pan || IsRadialChart( model ) )
            return;

        var plot = BuildCurrentPlotArea( model );

        if ( !IsInsidePlot( eventArgs.OffsetX, eventArgs.OffsetY, plot ) )
            return;

        panning = true;
        panStartClientX = eventArgs.ClientX;
        panStartClientY = eventArgs.ClientY;
        panStartViewport = ResolveEffectiveViewport( model, ResolveFullViewport( BuildModel( false, false ) ) );
        panLastViewport = CloneViewport( panStartViewport );
    }

    private async Task HandleChartMouseMove( MouseEventArgs eventArgs )
    {
        if ( !panning )
            return;

        var model = BuildModel();
        var zoom = model.Zoom;

        if ( zoom?.Enabled != true || !zoom.Pan )
            return;

        var plot = BuildCurrentPlotArea( model );
        var fullViewport = ResolveFullViewport( BuildModel( false, false ) );
        var nextViewport = PanViewport( panStartViewport, fullViewport, zoom, plot, eventArgs.ClientX - panStartClientX, eventArgs.ClientY - panStartClientY );

        panLastViewport = CloneViewport( nextViewport );
        await ApplyViewport( ResolveEffectiveViewport( model, fullViewport ), nextViewport );
    }

    private async Task HandleChartMouseUp( MouseEventArgs eventArgs )
    {
        if ( !panning )
            return;

        panning = false;

        if ( panStartViewport is not null && panLastViewport is not null )
        {
            await NotifyPanned( panStartViewport, panLastViewport );
        }

        panStartViewport = null;
        panLastViewport = null;
    }

    private async Task HandleChartMouseLeave( MouseEventArgs eventArgs )
    {
        await HandleChartMouseUp( eventArgs );
    }

    public Task SetData( SvgChartData<double?> data )
    {
        internalChartData = data;
        streamingAnimationActive = false;
        StateHasChanged();

        return Task.CompletedTask;
    }
    public Task SetOptions( SvgChartOptions options )
    {
        Options = options;
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task AddLabel( object label )
    {
        EnsureInternalData().Labels.Add( label );
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task AddSeries( SvgChartSeriesData<double?> series )
    {
        EnsureInternalData().Series.Add( series );
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task RemoveSeries( string name )
    {
        var data = EnsureInternalData();
        data.Series.RemoveAll( x => x.Name == name );
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task AddValue( string seriesName, double? value )
    {
        var series = EnsureInternalData().Series.FirstOrDefault( x => x.Name == seriesName );

        if ( series is not null )
        {
            series.Values.Add( value );
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    #region Streaming

    /// <summary>
    /// Appends a streamed value to a series and adds the label as the next category.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <param name="label">The category label.</param>
    /// <param name="value">The value to append.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AppendValue( string seriesName, object label, double? value )
    {
        return AppendValues( label, new Dictionary<string, double?> { [seriesName] = value } );
    }

    /// <summary>
    /// Appends streamed values to matching series and adds the label as the next category.
    /// </summary>
    /// <param name="label">The category label.</param>
    /// <param name="values">The values keyed by series name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AppendValues( object label, IReadOnlyDictionary<string, double?> values )
    {
        var streaming = ResolveStreaming();
        var streamingValues = values ?? new Dictionary<string, double?>();

        if ( streamingPaused )
            return;

        var data = EnsureInternalData();
        var previousCount = data.Labels.Count;

        foreach ( var value in streamingValues )
            EnsureSeries( data, value.Key, previousCount );

        foreach ( var series in data.Series )
        {
            while ( series.Values.Count < previousCount )
                series.Values.Add( null );
        }

        data.Labels.Add( label );

        foreach ( var series in data.Series )
            series.Values.Add( streamingValues.TryGetValue( series.Name, out var value ) ? value : null );

        if ( streaming.Enabled && IsStreamingAnimationEnabled( streaming ) && streaming.VisibleDataPoints.HasValue )
        {
            streamingAnimationVersion++;
            streamingAnimationActive = false;
        }

        TrimStreamingData( data, streaming, label );

        await RefreshStreaming( streaming );

        if ( streaming.Enabled && IsStreamingAnimationEnabled( streaming ) && streaming.VisibleDataPoints.HasValue )
        {
            await Task.Delay( 16 );
            streamingAnimationActive = true;
            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Updates the maximum number of streamed data points to keep.
    /// </summary>
    /// <param name="maxDataPoints">The maximum data point count, or null to keep all points by count.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SetMaxDataPoints( int? maxDataPoints )
    {
        var streaming = Streaming ??= new();
        streaming.MaxDataPoints = maxDataPoints;

        TrimStreamingData( EnsureInternalData(), streaming, null );
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Pauses accepting streamed values.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task PauseStreaming()
    {
        streamingPaused = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Resumes accepting streamed values.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ResumeStreaming()
    {
        streamingPaused = false;
        StateHasChanged();

        return Task.CompletedTask;
    }

    #endregion

    public Task SetValue( string seriesName, int index, double? value )
    {
        var series = EnsureInternalData().Series.FirstOrDefault( x => x.Name == seriesName );

        if ( series is not null && index >= 0 && index < series.Values.Count )
        {
            series.Values[index] = value;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    public Task ToggleSeries( string seriesName )
    {
        if ( !hiddenSeries.Add( seriesName ) )
            hiddenSeries.Remove( seriesName );

        activeTooltip = null;
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task ShowSeries( string seriesName )
    {
        hiddenSeries.Remove( seriesName );
        activeTooltip = null;
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task HideSeries( string seriesName )
    {
        hiddenSeries.Add( seriesName );
        activeTooltip = null;
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task ToggleDataPoint( string seriesName, int pointIndex )
    {
        var key = GetDataPointKey( seriesName, pointIndex );

        if ( !hiddenDataPoints.Add( key ) )
            hiddenDataPoints.Remove( key );

        activeTooltip = null;
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task ShowDataPoint( string seriesName, int pointIndex )
    {
        hiddenDataPoints.Remove( GetDataPointKey( seriesName, pointIndex ) );
        activeTooltip = null;
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task HideDataPoint( string seriesName, int pointIndex )
    {
        hiddenDataPoints.Add( GetDataPointKey( seriesName, pointIndex ) );
        activeTooltip = null;
        StateHasChanged();

        return Task.CompletedTask;
    }

    private static string GetDataPointKey( string seriesName, int pointIndex )
    {
        return $"{seriesName ?? string.Empty}:{pointIndex}";
    }

    private bool IsDataPointHidden( string seriesName, int pointIndex )
    {
        return hiddenDataPoints.Contains( GetDataPointKey( seriesName, pointIndex ) );
    }

    public Task Update()
    {
        StateHasChanged();

        return Task.CompletedTask;
    }

    public Task Resize()
    {
        StateHasChanged();

        return Task.CompletedTask;
    }

    public async Task SetViewport( SvgChartViewport viewport )
    {
        var model = BuildModel();
        var fullViewport = ResolveFullViewport( BuildModel( false, false ) );
        var previousViewport = ResolveEffectiveViewport( model, fullViewport );
        var nextViewport = CloneViewport( viewport );

        await ApplyViewport( previousViewport, nextViewport );
    }

    public async Task ResetZoom()
    {
        var model = BuildModel();
        var fullViewport = ResolveFullViewport( BuildModel( false, false ) );
        var previousViewport = ResolveEffectiveViewport( model, fullViewport );

        await ApplyViewport( previousViewport, null );
        await NotifyZoomed( previousViewport, fullViewport, SvgChartZoomSource.Api );
    }

    public async Task ZoomIn()
    {
        await ZoomBy( 0.82 );
    }

    public async Task ZoomOut()
    {
        await ZoomBy( 1.22 );
    }

    public Task Clear()
    {
        var data = EnsureInternalData();

        data.Labels.Clear();

        foreach ( var series in data.Series )
        {
            series.Values.Clear();
            series.XValues?.Clear();
            series.YValues?.Clear();
            series.RadiusValues?.Clear();
        }

        hiddenSeries.Clear();
        hiddenDataPoints.Clear();
        activeTooltip = null;
        activeTooltipPinned = false;
        internalViewport = null;
        panning = false;
        streamingAnimationVersion = 0;
        streamingAnimationActive = false;
        lastStreamingRender = DateTimeOffset.MinValue;
        StateHasChanged();

        return Task.CompletedTask;
    }

    private async Task ZoomBy( double factor )
    {
        var model = BuildModel();
        var zoom = model.Zoom;

        if ( zoom?.Enabled != true )
            return;

        var fullViewport = ResolveFullViewport( BuildModel( false, false ) );
        var previousViewport = ResolveEffectiveViewport( model, fullViewport );
        var plot = BuildCurrentPlotArea( model );
        var nextViewport = ZoomViewport( previousViewport, fullViewport, zoom, plot, plot.Left + plot.Width / 2, plot.Top + plot.Height / 2, factor );

        await ApplyViewport( previousViewport, nextViewport );
        await NotifyZoomed( previousViewport, nextViewport, SvgChartZoomSource.Api );
    }

    public ValueTask<string> ToSvgString()
    {
        var model = BuildModel();
        var options = ResolveOptions();
        var title = ResolveTitleOptions( options );
        var ariaLabel = IsTextVisible( title ) ? title.Text : "SVG chart";

        return ValueTask.FromResult( $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {Format( options.Width )} {Format( options.Height )}\" role=\"img\" aria-label=\"{ariaLabel}\"><desc>{model.Series.Count} series, {model.Labels.Count} categories.</desc></svg>" );
    }

    private SvgChartData<double?> EnsureInternalData()
    {
        if ( internalChartData is not null )
            return internalChartData;

        if ( Data is not null )
        {
            internalChartData = Data;
            return internalChartData;
        }

        var model = BuildModel( false, false );

        internalChartData = new()
        {
            Labels = model.Labels.ToList(),
            Series = model.Series.Select( x => new SvgChartSeriesData<double?>
            {
                Name = x.Name,
                Values = x.Values.ToList(),
                XValues = x.XValues.ToList(),
                YValues = x.YValues.ToList(),
                RadiusValues = x.RadiusValues.ToList(),
                Order = x.Order,
                CategoryAxisId = x.CategoryAxisId,
                ValueAxisId = x.ValueAxisId,
                Color = x.Color,
                Hidden = x.Hidden
            } ).ToList()
        };

        return internalChartData;
    }

    private static SvgChartSeriesData<double?> EnsureSeries( SvgChartData<double?> data, string seriesName, int valueCount )
    {
        var series = data.Series.FirstOrDefault( x => x.Name == seriesName );

        if ( series is null )
        {
            series = new()
            {
                Name = seriesName
            };

            data.Series.Add( series );
        }

        while ( series.Values.Count < valueCount )
            series.Values.Add( null );

        return series;
    }

    private void TrimStreamingData( SvgChartData<double?> data, SvgChartStreamingOptions streaming, object latestLabel )
    {
        if ( !streaming.Enabled )
            return;

        if ( streaming.MaxDataPoints.HasValue )
        {
            var maxDataPoints = Math.Max( 0, streaming.MaxDataPoints.Value );

            while ( data.Labels.Count > maxDataPoints )
                RemoveDataPoint( data, 0 );
        }

        if ( streaming.Duration.HasValue && TryGetDateTimeOffset( latestLabel ?? data.Labels.LastOrDefault(), out var latest ) )
        {
            var cutoff = latest - streaming.Duration.Value;

            while ( data.Labels.Count > 0 && TryGetDateTimeOffset( data.Labels[0], out var timestamp ) && timestamp < cutoff )
                RemoveDataPoint( data, 0 );
        }
    }

    private static void RemoveDataPoint( SvgChartData<double?> data, int index )
    {
        if ( index >= 0 && index < data.Labels.Count )
            data.Labels.RemoveAt( index );

        foreach ( var series in data.Series )
        {
            RemoveAt( series.Values, index );
            RemoveAt( series.XValues, index );
            RemoveAt( series.YValues, index );
            RemoveAt( series.RadiusValues, index );
        }
    }

    private static void RemoveAt<TValue>( List<TValue> values, int index )
    {
        if ( index >= 0 && index < values.Count )
            values.RemoveAt( index );
    }

    private Task RefreshStreaming( SvgChartStreamingOptions streaming )
    {
        activeTooltip = null;

        if ( !streaming.Enabled || streaming.RefreshInterval <= TimeSpan.Zero )
        {
            StateHasChanged();
            return Task.CompletedTask;
        }

        var now = DateTimeOffset.UtcNow;

        if ( now - lastStreamingRender >= streaming.RefreshInterval )
        {
            lastStreamingRender = now;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    private static bool TryGetDateTimeOffset( object value, out DateTimeOffset dateTimeOffset )
    {
        switch ( value )
        {
            case DateTimeOffset offset:
                dateTimeOffset = offset;
                return true;
            case DateTime dateTime:
                dateTimeOffset = dateTime.Kind == DateTimeKind.Unspecified
                    ? new DateTimeOffset( DateTime.SpecifyKind( dateTime, DateTimeKind.Local ) )
                    : new DateTimeOffset( dateTime );
                return true;
            default:
                dateTimeOffset = default;
                return false;
        }
    }

    internal override void RegisterSeries( object series )
    {
        if ( !seriesComponents.Contains( series ) )
            seriesComponents.Add( series );
    }

    internal override void UnregisterSeries( object series )
    {
        seriesComponents.Remove( series );
    }

    internal override void RegisterCategoryAxis( object axis )
    {
        if ( !categoryAxisComponents.Contains( axis ) )
            categoryAxisComponents.Add( axis );
    }

    internal override void UnregisterCategoryAxis( object axis )
    {
        categoryAxisComponents.Remove( axis );
    }

    internal override void RegisterValueAxis( SvgChartValueAxis axis )
    {
        if ( !valueAxisComponents.Contains( axis ) )
            valueAxisComponents.Add( axis );
    }

    internal override void UnregisterValueAxis( SvgChartValueAxis axis )
    {
        valueAxisComponents.Remove( axis );
    }

    internal override void RegisterTitle( SvgChartTitle title )
    {
        if ( !titleComponents.Contains( title ) )
            titleComponents.Add( title );
    }

    internal override void UnregisterTitle( SvgChartTitle title )
    {
        titleComponents.Remove( title );
    }

    internal override void RegisterLegend( SvgChartLegend legend )
    {
        if ( !legendComponents.Contains( legend ) )
            legendComponents.Add( legend );
    }

    internal override void UnregisterLegend( SvgChartLegend legend )
    {
        legendComponents.Remove( legend );
    }

    internal override void RegisterTooltip( SvgChartTooltip tooltip )
    {
        if ( !tooltipComponents.Contains( tooltip ) )
            tooltipComponents.Add( tooltip );
    }

    internal override void UnregisterTooltip( SvgChartTooltip tooltip )
    {
        tooltipComponents.Remove( tooltip );
    }

    internal override void RegisterPlugin( ISvgChartPlugin plugin )
    {
        if ( plugin is not null && !pluginComponents.Contains( plugin ) )
            pluginComponents.Add( plugin );
    }

    internal override void UnregisterPlugin( ISvgChartPlugin plugin )
    {
        pluginComponents.Remove( plugin );
    }

    internal override void Refresh()
    {
        _ = InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Specifies the chart rendering type.
    /// </summary>
    [Parameter] public SvgChartType Type { get; set; } = SvgChartType.Column;

    /// <summary>
    /// Specifies the item collection used by declarative axis and series selectors.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Items { get; set; }

    /// <summary>
    /// Specifies explicit chart data used by imperative and model-based chart definitions.
    /// </summary>
    [Parameter] public SvgChartData<double?> Data { get; set; }

    /// <summary>
    /// Specifies chart rendering options.
    /// </summary>
    [Parameter] public SvgChartOptions Options { get; set; }

    /// <summary>
    /// Specifies chart streaming options.
    /// </summary>
    [Parameter] public SvgChartStreamingOptions Streaming { get; set; }

    /// <summary>
    /// Occurs when a rendered chart point is clicked.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when a rendered chart point is hovered.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Hovered { get; set; }

    /// <summary>
    /// Specifies the content to render inside the chart, including declarative axes, series, and options.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    #endregion

    #region Models

    private sealed class RenderModel
    {
        public SvgChartType Type { get; init; }

        public SvgChartOptions Options { get; init; }

        public SvgChartZoomOptions Zoom { get; init; }

        public SvgChartViewport Viewport { get; init; }

        public List<object> Labels { get; init; } = [];

        public int CategorySlotCount { get; init; }

        public List<int> CategoryLabelIndexes { get; init; } = [];

        public double CategoryMin { get; init; }

        public double CategoryMax { get; init; }

        public SvgChartAxisOptions CategoryAxis { get; init; }

        public List<RenderSeries> Series { get; init; } = [];

        public double Min { get; init; }

        public double Max { get; init; }

        public List<double> Ticks { get; init; } = [];

        public List<RenderValueAxis> ValueAxes { get; init; } = [];

        public RenderValueAxis PrimaryValueAxis { get; init; }

        public SvgChartTooltipOptions Tooltip { get; init; }
    }

    private sealed class RenderSeries
    {
        public string Name { get; init; }

        public SvgChartType Type { get; init; }

        public List<double?> Values { get; init; } = [];

        public List<double?> XValues { get; init; } = [];

        public List<double?> YValues { get; init; } = [];

        public List<double?> RadiusValues { get; init; } = [];

        public Color Color { get; init; }

        public string RenderColor { get; init; }

        public bool Hidden { get; init; }

        public int? Order { get; init; }

        public string CategoryAxisId { get; init; }

        public string ValueAxisId { get; init; }

        public double BorderRadius { get; init; }

        public double StrokeWidth { get; init; }

        public double MarkerRadius { get; init; }

        public double FillOpacity { get; init; }
    }

    private sealed class LegendItem
    {
        public string Label { get; init; }

        public string Color { get; init; }

        public bool Hidden { get; init; }

        public Func<Task> Toggle { get; init; }
    }

    private sealed class PlotArea
    {
        public double Left { get; init; }

        public double Top { get; init; }

        public double Right { get; init; }

        public double Bottom { get; init; }

        public double Width => Right - Left;

        public double Height => Bottom - Top;
    }

    private sealed class RenderValueAxis
    {
        public string Id { get; init; }

        public SvgChartAxisPosition Position { get; init; }

        public SvgChartGridLinesOptions GridLines { get; init; }

        public double Min { get; init; }

        public double Max { get; init; }

        public List<double> Ticks { get; init; } = [];
    }

    private sealed class Scale
    {
        public double Min { get; init; }

        public double Max { get; init; }

        public List<double> Ticks { get; init; } = [];
    }

    #endregion
}