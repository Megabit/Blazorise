#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Licensing;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using static Blazorise.Charts.Svg.SvgChartGeometry;
using static Blazorise.Charts.Svg.SvgChartOptionsMapper;
using static Blazorise.Charts.Svg.SvgChartTextRenderer;
using static Blazorise.Charts.Svg.SvgChartZoomController;
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

    private static readonly TimeSpan PanRenderInterval = TimeSpan.FromMilliseconds( 16 );

    private static readonly IReadOnlyList<ISvgChartSeriesRenderer> BuiltInSeriesRenderers =
    [
        new SvgChartAreaSeriesRenderer(),
        new SvgChartColumnSeriesRenderer(),
        new SvgChartBarSeriesRenderer(),
        new SvgChartLineSeriesRenderer(),
        new SvgChartPointSeriesRenderer(),
        new SvgChartRadialSeriesRenderer(),
        new SvgChartRadarSeriesRenderer()
    ];

    private readonly HashSet<string> hiddenSeries = [];

    private readonly HashSet<string> hiddenDataPoints = [];

    private SvgChartData<double?> internalChartData;

    private SvgChartTooltipContext activeTooltip;

    private string activeTooltipKey;

    private IReadOnlyDictionary<string, SvgChartPointBounds> previousAnimationPointBounds = new Dictionary<string, SvgChartPointBounds>();

    private IReadOnlyDictionary<string, string> previousAnimationPathValues = new Dictionary<string, string>();

    private ComponentParameterInfo<SvgChartType> paramType;

    private ComponentParameterInfo<IEnumerable<TItem>> paramItems;

    private ComponentParameterInfo<SvgChartData<double?>> paramData;

    private ComponentParameterInfo<SvgChartOptions> paramOptions;

    private ComponentParameterInfo<SvgChartStreamingOptions> paramStreaming;

    private ComponentParameterInfo<SvgChartAnimationOptions> paramAnimation;

    private bool activeTooltipPinned;

    private bool renderedOnce;

    private int animationVersion;

    private bool streamingPaused;

    private int streamingAnimationVersion;

    private bool runStreamingAnimationAfterRender;

    private bool panning;

    private double panStartClientX;

    private double panStartClientY;

    private SvgChartViewport panStartViewport;

    private SvgChartViewport panLastViewport;

    private SvgChartViewport panRenderedViewport;

    private SvgChartViewport panFullViewport;

    private SvgChartPlotArea panPlot;

    private SvgChartZoomOptions panZoom;

    private DateTimeOffset lastPanRender = DateTimeOffset.MinValue;

    private SvgChartViewport internalViewport;

    private IJSObjectReference jsModule;

    private bool zoomWheelInitialized;

    private DateTimeOffset lastStreamingRender = DateTimeOffset.MinValue;

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
        parameters.TryGetParameter( Animation, out paramAnimation );

        if ( paramType.Changed || paramItems.Changed || paramData.Changed || paramAnimation.Changed )
            ClearTooltip();

        if ( paramType.Changed || paramAnimation.Changed )
        {
            previousAnimationPointBounds = new Dictionary<string, SvgChartPointBounds>();
            previousAnimationPathValues = new Dictionary<string, string>();
            animationVersion = 0;
        }

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
        var title = ResolveTitleOptions( options, titleComponents );
        var subtitle = ResolveSubtitleOptions( options, titleComponents );
        var hasTopLegend = legend.Visible && legend.Position == SvgChartLegendPosition.Top;
        var hasBottomLegend = legend.Visible && legend.Position == SvgChartLegendPosition.Bottom;
        var plot = BuildPlotArea( options, title, subtitle, hasTopLegend, hasBottomLegend, model );
        var streamingAnimation = ResolveStreamingAnimation( model, plot );
        var chartAnimation = ResolveAnimation( options, streamingAnimation.Enabled );
        var currentAnimationPointBounds = new Dictionary<string, SvgChartPointBounds>();
        var currentAnimationPathValues = new Dictionary<string, string>();
        var pluginContext = CreatePluginRenderContext( model, plot );
        var seriesRendererContext = new SvgChartSeriesRendererContext( pluginContext, chartAnimation, previousAnimationPointBounds, currentAnimationPointBounds, previousAnimationPathValues, currentAnimationPathValues, ( value, index ) => FormatCategory( model, value, index ) );
        var zoom = model.Zoom;

        runStreamingAnimationAfterRender = streamingAnimation.Enabled;

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
        builder.AddAttribute( sequence++, "role", ResolveAccessibilityRole() );
        builder.AddAttribute( sequence++, "aria-label", ResolveAccessibilityLabel( title ) );
        builder.AddAttribute( sequence++, "viewBox", $"0 0 {Format( options.Width )} {Format( options.Height )}" );
        builder.AddAttribute( sequence++, "style", ResolveSvgStyle( options, zoom, panning ) );

        if ( TabIndex.HasValue )
            builder.AddAttribute( sequence++, "tabindex", TabIndex.Value );

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

        SvgChartRenderHelpers.AddFontFamilyAttribute( builder, ref sequence, options.Font?.Family );

        RenderAccessibilityText( builder, ref sequence, model );
        RenderFocusStyles( builder, ref sequence );
        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.Background );

        SvgChartTextRenderer.Render( builder, ref sequence, options, title, subtitle );

        if ( hasTopLegend )
            SvgChartLegendRenderer.Render( builder, ref sequence, model, options, legend.Position, 48, this, ToggleSeries, ToggleDataPoint, IsDataPointHidden );

        if ( IsBarChart( model ) )
            SvgChartAxesRenderer.RenderHorizontalGridAndAxes( builder, ref sequence, model, plot );
        else if ( !IsRadialChart( model ) )
            SvgChartAxesRenderer.RenderGridAndAxes( builder, ref sequence, model, plot, streamingAnimation, GetPlotClipPathId(), GetCategoryAxisLabelsClipPathId() );

        if ( !IsRadialChart( model ) )
            RenderPlotClipPath( builder, ref sequence, plot, options );

        if ( !IsBarChart( model ) && !IsRadialChart( model ) && streamingAnimation.Enabled )
            SvgChartAxesRenderer.RenderCategoryAxisLabels( builder, ref sequence, model, plot, streamingAnimation, GetCategoryAxisLabelsClipPathId() );

        if ( IsRadialChart( model ) )
        {
            RenderSeriesCore( builder, ref sequence, seriesRendererContext, static series => IsRadialChart( series.Type ) );
            RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.SeriesOverlay );
        }

        if ( !IsRadialChart( model ) )
            RenderCartesianSeries( builder, ref sequence, model, streamingAnimation, pluginContext, seriesRendererContext );

        if ( hasBottomLegend )
            SvgChartLegendRenderer.Render( builder, ref sequence, model, options, legend.Position, options.Height - 30, this, ToggleSeries, ToggleDataPoint, IsDataPointHidden );

        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.InteractionOverlay );
        RenderActiveTooltip( builder, ref sequence, model );
        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.Tooltip );

        builder.CloseElement();
        builder.CloseElement();

        previousAnimationPointBounds = currentAnimationPointBounds;
        previousAnimationPathValues = currentAnimationPathValues;
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        var zoom = ResolveZoom( ResolveOptions() );
        var shouldPreventWheelScroll = zoom.Enabled && zoom.Wheel;
        var shouldRunAnimations = ShouldRunAnimations();
        var shouldRunStreamingAnimations = runStreamingAnimationAfterRender;

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

        if ( shouldRunAnimations )
        {
            jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>( "import", "./_content/Blazorise.Charts.Svg/svgChart.js" );
            await jsModule.InvokeVoidAsync( "runAnimations", ElementRef );
        }

        if ( shouldRunStreamingAnimations )
        {
            jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>( "import", "./_content/Blazorise.Charts.Svg/svgChart.js" );
            await jsModule.InvokeVoidAsync( "runStreamingAnimations", ElementRef );
        }

        renderedOnce = true;

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
                    await jsModule.InvokeVoidAsync( "destroyAnimations", ElementRef );
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
        builder.AddContent( sequence++, ".svg-chart-surface [tabindex]:focus{outline:none;}.svg-chart-surface [data-svg-chart-animation-initial='true']{visibility:hidden;}.svg-chart-surface.svg-chart-pannable,.svg-chart-surface.svg-chart-pannable *{user-select:none;-webkit-user-select:none;-webkit-user-drag:none;}" );
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

    private SvgChartPluginRenderContext CreatePluginRenderContext( SvgChartRenderModel model, SvgChartPlotArea plot )
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
                PointColors = x.PointColors,
                Hidden = x.Hidden,
                Order = x.Order,
                CategoryAxisId = x.CategoryAxisId,
                ValueAxisId = x.ValueAxisId,
                Stack = x.Stack,
                StackBaseValues = x.StackBaseValues,
                StackEndValues = x.StackEndValues,
                BorderRadius = x.BorderRadius,
                StrokeWidth = x.StrokeWidth,
                OutlineColor = x.OutlineColor,
                OutlineStrokeWidth = x.OutlineStrokeWidth,
                OutlineOpacity = x.OutlineOpacity,
                MarkerRadius = x.MarkerRadius,
                FillOpacity = x.FillOpacity,
                Interpolation = x.Interpolation,
                Tension = x.Tension
            } ).ToList(),
            IsRadialChart( model ),
            model.Min,
            model.Max,
            valueAxisId => ResolveValueAxis( model, valueAxisId ).Min,
            valueAxisId => ResolveValueAxis( model, valueAxisId ).Max,
            GetCategorySlotCount( model ),
            model.CategoryScaleKind,
            this,
            ( value, categoryAxisId ) => GetCategoryX( (int)Math.Round( value, MidpointRounding.AwayFromZero ), plot, model ),
            ( value, categoryAxisId ) => GetCategoryBoundaryX( value, plot, model ),
            ( value, valueAxisId ) =>
            {
                if ( pointXScale is not null )
                    return GetX( value, plot, pointXScale.Min, pointXScale.Max );

                var axis = ResolveValueAxis( model, valueAxisId );

                return GetX( value, plot, axis.Min, axis.Max );
            },
            ( value, valueAxisId ) =>
            {
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

        result.AddRange( pluginComponents.Select( plugin => plugin is SvgChartDataLabels dataLabels
            ? SvgChartDataLabels.Create( options.DataLabels, dataLabels )
            : plugin ) );

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

    private void RenderPlotClipPath( RenderTreeBuilder builder, ref int sequence, SvgChartPlotArea plot, SvgChartOptions options )
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

    private void RenderCartesianSeries( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model, SvgChartStreamingAnimation animation, SvgChartPluginRenderContext pluginContext, SvgChartSeriesRendererContext seriesRendererContext )
    {
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
            AddStreamingAnimationAttributes( builder, ref sequence, animation );
        }

        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.BeforeSeries );
        RenderSeriesCore( builder, ref sequence, seriesRendererContext, static series => !IsRadialChart( series.Type ) );

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

    private void RenderSeriesCore( RenderTreeBuilder builder, ref int sequence, SvgChartSeriesRendererContext context, Func<SvgChartPluginSeries, bool> filter )
    {
        var renderers = ResolveSeriesRenderers();
        var rendererItems = context.Chart.Series
            .Where( x => !x.Hidden && filter( x ) )
            .Select( series => new { Series = series, Renderer = ResolveSeriesRenderer( renderers, series ) } )
            .Where( x => x.Renderer is not null )
            .Select( x => new { x.Series, x.Renderer, Order = x.Renderer.GetRenderOrder( x.Series ) } )
            .GroupBy( x => x.Order )
            .OrderBy( x => x.Key )
            .ToList();

        foreach ( var renderOrder in rendererItems )
        {
            foreach ( var renderer in renderers )
            {
                var series = renderOrder
                    .Where( x => ReferenceEquals( x.Renderer, renderer ) )
                    .Select( x => x.Series )
                    .ToList();

                if ( series.Count > 0 )
                    renderer.Render( context, series, builder, ref sequence );
            }
        }
    }

    private static ISvgChartSeriesRenderer ResolveSeriesRenderer( IReadOnlyList<ISvgChartSeriesRenderer> renderers, SvgChartPluginSeries series )
    {
        return renderers.FirstOrDefault( x => x.CanRender( series ) );
    }

    internal virtual IReadOnlyList<ISvgChartSeriesRenderer> ResolveSeriesRenderers()
    {
        return BuiltInSeriesRenderers;
    }

    private void RenderActiveTooltip( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model )
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
            builder.AddContent( sequence++, context.Items.Count > 1 ? FormatCategory( model, context.Category, context.PointIndex ) : context.SeriesName );
            builder.CloseElement();

            if ( tooltip.Formatter is not null || context.Items.Count <= 1 )
            {
                builder.OpenElement( sequence++, "div" );
                builder.AddAttribute( sequence++, "style", "line-height:1.35;opacity:.86;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;" );
                builder.AddContent( sequence++, tooltip.Formatter?.Invoke( context ) ?? context.Text );
                builder.CloseElement();
            }
            else
            {
                foreach ( var item in context.Items )
                {
                    builder.OpenElement( sequence++, "div" );
                    builder.AddAttribute( sequence++, "style", "display:flex;align-items:center;gap:.35rem;line-height:1.35;opacity:.86;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;" );

                    builder.OpenElement( sequence++, "span" );
                    builder.AddAttribute( sequence++, "style", $"display:inline-block;width:.55rem;height:.55rem;border-radius:50%;background:{item.Color};flex:0 0 auto;" );
                    builder.CloseElement();

                    builder.AddContent( sequence++, $"{item.SeriesName}: {item.Value}" );
                    builder.CloseElement();
                }
            }
        }

        builder.CloseElement();
        builder.CloseElement();
    }

    private SvgChartRenderModel BuildModel( bool applyStreamingViewport = true, bool applyZoomViewport = true )
    {
        return new SvgChartModelBuilder<TItem>(
            Type,
            Items,
            Data,
            Options,
            Streaming,
            internalViewport,
            internalChartData,
            seriesComponents,
            categoryAxisComponents,
            valueAxisComponents,
            pluginComponents,
            tooltipComponents,
            hiddenSeries,
            BlazoriseLicenseLimitsHelper.GetChartsRowsLimit( ComponentLicenseChecker ) ).Build( applyStreamingViewport, applyZoomViewport );
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

    private bool ShouldRunAnimations()
    {
        var animationComponent = pluginComponents.OfType<SvgChartAnimation>().LastOrDefault();
        var animationOptions = Animation ?? ResolveOptions().Animation ?? new();
        var animation = CreateAnimationOptions( animationComponent?.ResolveOptions( animationOptions ) ?? animationOptions );

        return animation?.Enabled == true && IsAnyAnimationTargetEnabled( animation );
    }

    private SvgChartResolvedAnimation ResolveAnimation( SvgChartOptions options, bool streamingAnimationEnabled )
    {
        var animationComponent = pluginComponents.OfType<SvgChartAnimation>().LastOrDefault();
        var animationOptions = Animation ?? options.Animation ?? new();
        var animation = CreateAnimationOptions( animationComponent?.ResolveOptions( animationOptions ) ?? animationOptions );

        if ( streamingAnimationEnabled
             || !animation.Enabled
             || !IsAnyAnimationTargetEnabled( animation ) )
            return new();

        var geometry = ResolveGeometryAnimation( animation );
        var opacity = ResolveOpacityAnimation( animation );
        var stroke = ResolveAnimationTarget( animation.Stroke, animation );
        var transform = ResolveAnimationTarget( animation.Transform, animation );
        var path = ResolveAnimationTarget( animation.Path, animation );

        if ( !geometry.Enabled && !opacity.Enabled && !stroke.Enabled && !transform.Enabled && !path.Enabled )
            return new();

        return new()
        {
            Enabled = true,
            InitialRender = !renderedOnce,
            Version = ++animationVersion,
            Geometry = geometry,
            Opacity = opacity,
            Stroke = stroke,
            Transform = transform,
            Path = path
        };
    }

    private SvgChartResolvedGeometryAnimation ResolveGeometryAnimation( SvgChartAnimationOptions animation )
    {
        var target = ResolveAnimationTarget( animation.Geometry, animation );

        return new()
        {
            Enabled = target.Enabled,
            AnimateInitial = target.AnimateInitial,
            AnimateUpdates = target.AnimateUpdates,
            Duration = target.Duration,
            Delay = target.Delay,
            KeySplines = target.KeySplines,
            AnimatePosition = animation.Geometry?.AnimatePosition ?? true,
            AnimateSize = animation.Geometry?.AnimateSize ?? true
        };
    }

    private SvgChartResolvedOpacityAnimation ResolveOpacityAnimation( SvgChartAnimationOptions animation )
    {
        var target = ResolveAnimationTarget( animation.Opacity, animation );

        return new()
        {
            Enabled = target.Enabled,
            AnimateInitial = target.AnimateInitial,
            AnimateUpdates = target.AnimateUpdates,
            Duration = target.Duration,
            Delay = target.Delay,
            KeySplines = target.KeySplines,
            From = Format( animation.Opacity?.From ?? 0 )
        };
    }

    private SvgChartResolvedAnimationTarget ResolveAnimationTarget( SvgChartAnimationTargetOptions target, SvgChartAnimationOptions animation )
    {
        var duration = target?.Duration ?? animation.Duration;

        return new()
        {
            Enabled = target?.Enabled == true && duration > TimeSpan.Zero,
            AnimateInitial = !renderedOnce && ( target?.AnimateOnLoad ?? animation.AnimateOnLoad ),
            AnimateUpdates = target?.AnimateOnUpdate ?? animation.AnimateOnUpdate,
            Duration = FormatDuration( duration ),
            Delay = FormatDuration( target?.Delay ?? animation.Delay ),
            KeySplines = ResolveAnimationKeySplines( target?.Easing ?? animation.Easing )
        };
    }

    private static SvgChartAnimationOptions CreateAnimationOptions( SvgChartAnimationOptions options )
    {
        if ( options is null )
            return new();

        return new()
        {
            Enabled = options.Enabled,
            Duration = options.Duration,
            Delay = options.Delay,
            Easing = options.Easing,
            AnimateOnLoad = options.AnimateOnLoad,
            AnimateOnUpdate = options.AnimateOnUpdate,
            Geometry = CreateGeometryAnimationOptions( options.Geometry ),
            Opacity = CreateOpacityAnimationOptions( options.Opacity ),
            Stroke = CreateStrokeAnimationOptions( options.Stroke ),
            Transform = CreateTransformAnimationOptions( options.Transform ),
            Path = CreatePathAnimationOptions( options.Path )
        };
    }

    private static SvgChartGeometryAnimationOptions CreateGeometryAnimationOptions( SvgChartGeometryAnimationOptions options )
    {
        return new()
        {
            Enabled = options?.Enabled ?? true,
            Duration = options?.Duration,
            Delay = options?.Delay,
            Easing = options?.Easing,
            AnimateOnLoad = options?.AnimateOnLoad,
            AnimateOnUpdate = options?.AnimateOnUpdate,
            AnimatePosition = options?.AnimatePosition ?? true,
            AnimateSize = options?.AnimateSize ?? true
        };
    }

    private static SvgChartOpacityAnimationOptions CreateOpacityAnimationOptions( SvgChartOpacityAnimationOptions options )
    {
        return new()
        {
            Enabled = options?.Enabled ?? true,
            Duration = options?.Duration,
            Delay = options?.Delay,
            Easing = options?.Easing,
            AnimateOnLoad = options?.AnimateOnLoad,
            AnimateOnUpdate = options?.AnimateOnUpdate,
            From = options?.From ?? 0
        };
    }

    private static SvgChartStrokeAnimationOptions CreateStrokeAnimationOptions( SvgChartStrokeAnimationOptions options )
    {
        return new()
        {
            Enabled = options?.Enabled == true,
            Duration = options?.Duration,
            Delay = options?.Delay,
            Easing = options?.Easing,
            AnimateOnLoad = options?.AnimateOnLoad,
            AnimateOnUpdate = options?.AnimateOnUpdate,
            AnimateWidth = options?.AnimateWidth ?? true,
            AnimateDashPattern = options?.AnimateDashPattern ?? true
        };
    }

    private static SvgChartTransformAnimationOptions CreateTransformAnimationOptions( SvgChartTransformAnimationOptions options )
    {
        return new()
        {
            Enabled = options?.Enabled == true,
            Duration = options?.Duration,
            Delay = options?.Delay,
            Easing = options?.Easing,
            AnimateOnLoad = options?.AnimateOnLoad,
            AnimateOnUpdate = options?.AnimateOnUpdate,
            ScaleFrom = options?.ScaleFrom ?? 0.95,
            ScaleTo = options?.ScaleTo ?? 1
        };
    }

    private static SvgChartPathAnimationOptions CreatePathAnimationOptions( SvgChartPathAnimationOptions options )
    {
        return new()
        {
            Enabled = options?.Enabled == true,
            Duration = options?.Duration,
            Delay = options?.Delay,
            Easing = options?.Easing,
            AnimateOnLoad = options?.AnimateOnLoad,
            AnimateOnUpdate = options?.AnimateOnUpdate,
            AnimateShape = options?.AnimateShape ?? true,
            AnimateLength = options?.AnimateLength ?? true
        };
    }

    private static bool IsAnyAnimationTargetEnabled( SvgChartAnimationOptions animation )
    {
        return IsAnimationTargetEnabled( animation.Geometry, animation )
            || IsAnimationTargetEnabled( animation.Opacity, animation )
            || IsAnimationTargetEnabled( animation.Stroke, animation )
            || IsAnimationTargetEnabled( animation.Transform, animation )
            || IsAnimationTargetEnabled( animation.Path, animation );
    }

    private static bool IsAnimationTargetEnabled( SvgChartAnimationTargetOptions target, SvgChartAnimationOptions animation )
    {
        return target?.Enabled == true && ( target.Duration ?? animation.Duration ) > TimeSpan.Zero;
    }

    private static string ResolveAnimationKeySplines( SvgChartAnimationEasing easing )
    {
        return easing switch
        {
            SvgChartAnimationEasing.Ease => "0.25 0.1 0.25 1",
            SvgChartAnimationEasing.EaseIn => "0.42 0 1 1",
            SvgChartAnimationEasing.EaseInOut => "0.42 0 0.58 1",
            SvgChartAnimationEasing.EaseOut => "0 0 0.58 1",
            _ => null
        };
    }

    private SvgChartLegendOptions ResolveLegend( SvgChartOptions options )
    {
        var legendComponent = legendComponents.LastOrDefault();
        var legendOptions = options.Legend ?? new();

        return legendComponent?.ResolveOptions( legendOptions ) ?? legendOptions;
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

    private static bool SupportsStreamingAnimation( SvgChartRenderModel model )
    {
        return model.Series.Any( x => !x.Hidden && x.Type is SvgChartType.Column or SvgChartType.Line or SvgChartType.Area );
    }

    private static int GetCategorySlotCount( SvgChartRenderModel model )
    {
        return Math.Max( model.CategorySlotCount > 0 ? model.CategorySlotCount : model.Labels.Count, 1 );
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

        ClearTooltip();
    }

    private void ShowTooltip( SvgChartPointEventArgs point, string color, bool pinned )
    {
        var tooltipKey = CreateTooltipKey( point, pinned );

        if ( activeTooltip is not null && string.Equals( activeTooltipKey, tooltipKey, StringComparison.Ordinal ) )
            return;

        var model = BuildModel();
        var options = model.Options;
        var tooltip = model.Tooltip;

        if ( !tooltip.Enabled )
            return;

        activeTooltip = BuildTooltipContext( point, color, model, tooltip );
        activeTooltipKey = tooltipKey;
        activeTooltipPinned = pinned;
    }

    private void ClearTooltip()
    {
        activeTooltip = null;
        activeTooltipKey = null;
        activeTooltipPinned = false;
    }

    private static string CreateTooltipKey( SvgChartPointEventArgs point, bool pinned )
    {
        return $"{point.SeriesIndex}:{point.PointIndex}:{pinned}";
    }

    private static SvgChartTooltipContext BuildTooltipContext( SvgChartPointEventArgs point, string color, SvgChartRenderModel model, SvgChartTooltipOptions tooltip )
    {
        var options = model.Options;
        var anchorX = point.Bounds.X + point.Bounds.Width / 2;
        var anchorY = point.Bounds.Y;
        var items = ResolveTooltipItems( point, color, model, tooltip.InteractionMode );
        var width = Math.Max( 1, tooltip.Width );
        var height = Math.Max( Math.Max( 1, tooltip.Height ), items.Count > 1 ? 28 + items.Count * 18 : 1 );
        var x = Math.Min( Math.Max( anchorX + tooltip.OffsetX, 0 ), Math.Max( options.Width - width, 0 ) );
        var y = Math.Min( Math.Max( anchorY - height - tooltip.OffsetY, 0 ), Math.Max( options.Height - height, 0 ) );

        return new()
        {
            SeriesName = point.SeriesName,
            SeriesIndex = point.SeriesIndex,
            PointIndex = point.PointIndex,
            Category = point.Category,
            Value = point.Value,
            InteractionMode = tooltip.InteractionMode,
            Items = items,
            Bounds = point.Bounds,
            Color = color,
            Text = GetPointLabel( point, model ),
            X = x,
            Y = y,
            Width = width,
            Height = height,
            Point = point
        };
    }

    private static IReadOnlyList<SvgChartTooltipItemContext> ResolveTooltipItems( SvgChartPointEventArgs point, string color, SvgChartRenderModel model, SvgChartInteractionMode mode )
    {
        return mode switch
        {
            SvgChartInteractionMode.Index => ResolveIndexTooltipItems( point, model ),
            SvgChartInteractionMode.Dataset => ResolveDatasetTooltipItems( point, model ),
            _ => [CreateTooltipItem( point, color )]
        };
    }

    private static IReadOnlyList<SvgChartTooltipItemContext> ResolveIndexTooltipItems( SvgChartPointEventArgs point, SvgChartRenderModel model )
    {
        if ( point.PointIndex < 0 )
            return [CreateTooltipItem( point, null )];

        return model.Series.Where( x => !x.Hidden && point.PointIndex < ResolveTooltipValues( x ).Count )
            .Select( series =>
            {
                var values = ResolveTooltipValues( series );
                var value = values[point.PointIndex];
                var seriesIndex = model.Series.IndexOf( series );

                return value.HasValue
                    ? CreateTooltipItem( point, series, seriesIndex, point.PointIndex, value.Value, ResolveTooltipCategory( model, point.PointIndex, point.Category ) )
                    : null;
            } )
            .Where( x => x is not null )
            .ToList();
    }

    private static IReadOnlyList<SvgChartTooltipItemContext> ResolveDatasetTooltipItems( SvgChartPointEventArgs point, SvgChartRenderModel model )
    {
        var series = model.Series.ElementAtOrDefault( point.SeriesIndex );

        if ( series is null )
            return [CreateTooltipItem( point, null )];

        var values = ResolveTooltipValues( series );
        var result = new List<SvgChartTooltipItemContext>();

        for ( var i = 0; i < values.Count; i++ )
        {
            var value = values[i];

            if ( value.HasValue )
                result.Add( CreateTooltipItem( point, series, point.SeriesIndex, i, value.Value, ResolveTooltipCategory( model, i, point.Category ) ) );
        }

        return result;
    }

    private static IReadOnlyList<double?> ResolveTooltipValues( SvgChartRenderSeries series )
    {
        return IsPointChart( series.Type ) ? series.YValues : series.Values;
    }

    private static SvgChartTooltipItemContext CreateTooltipItem( SvgChartPointEventArgs point, string color )
    {
        return new()
        {
            SeriesName = point.SeriesName,
            SeriesIndex = point.SeriesIndex,
            PointIndex = point.PointIndex,
            Category = point.Category,
            Value = point.Value,
            Color = color ?? "currentColor",
            Point = point
        };
    }

    private static object ResolveTooltipCategory( SvgChartRenderModel model, int pointIndex, object fallback )
    {
        return pointIndex >= 0 && pointIndex < model.Labels.Count ? model.Labels[pointIndex] : fallback;
    }

    private static SvgChartTooltipItemContext CreateTooltipItem( SvgChartPointEventArgs sourcePoint, SvgChartRenderSeries series, int seriesIndex, int pointIndex, double value, object category )
    {
        return new()
        {
            SeriesName = series.Name,
            SeriesIndex = seriesIndex,
            PointIndex = pointIndex,
            Category = category,
            Value = value,
            Color = ResolvePointColor( series, pointIndex ),
            Point = new()
            {
                SeriesName = series.Name,
                SeriesIndex = seriesIndex,
                PointIndex = pointIndex,
                Category = category,
                Value = value,
                Bounds = sourcePoint.Bounds
            }
        };
    }

    private static string GetPointLabel( SvgChartPointEventArgs point, SvgChartRenderModel model )
    {
        return $"{FormatCategory( model, point.Category, point.PointIndex )}, {point.Value}. {point.SeriesName}.";
    }

    private static string FormatCategory( SvgChartRenderModel model, object value, int index )
    {
        return model.CategoryValueFormatter?.Invoke( new()
        {
            Value = value,
            Index = index,
            CategoryAxis = true,
            AxisId = model.CategoryAxis?.Id
        } ) ?? SvgChartRenderHelpers.FormatDataLabelValue( value );
    }

    private static string ResolvePointColor( SvgChartRenderSeries series, int pointIndex )
    {
        return pointIndex >= 0 && pointIndex < ( series.PointColors?.Count ?? 0 ) && !string.IsNullOrWhiteSpace( series.PointColors[pointIndex] )
            ? series.PointColors[pointIndex]
            : series.RenderColor;
    }

    private static string ResolveTooltipStyle( SvgChartOptions options, SvgChartTooltipContext context )
    {
        var font = options?.Font;
        var color = SvgChartRenderHelpers.IsDefaultColor( font?.Color )
            ? "var(--b-tooltip-color,#fff)"
            : SvgChartRenderHelpers.ResolveFontColor( font );
        var fontSize = font?.Size is null
            ? "var(--b-tooltip-font-size,.875rem)"
            : $"{Format( font.Size.Value )}px";
        var fontFamily = string.IsNullOrWhiteSpace( font?.Family )
            ? null
            : $"font-family:{font.Family};";

        return $"height:100%;box-sizing:border-box;overflow:hidden;border-left:3px solid {context.Color};background:rgba(var(--b-tooltip-background-color-r,33),var(--b-tooltip-background-color-g,37),var(--b-tooltip-background-color-b,41),var(--b-tooltip-background-opacity,.94));color:{color};border-radius:var(--b-tooltip-border-radius,.375rem);padding:var(--b-tooltip-padding,.5rem .75rem);font-size:{fontSize};{fontFamily}box-shadow:0 .35rem 1rem rgba(0,0,0,.18);";
    }

    private static string Format( double value )
    {
        return SvgChartRenderHelpers.Format( value );
    }

    private static string FormatDuration( TimeSpan value )
    {
        return SvgChartRenderHelpers.FormatDuration( value );
    }

    private string ResolveStreamingAnimationStyle( SvgChartStreamingAnimation animation )
    {
        return "transform:translateX(0px);transition:none;will-change:transform;";
    }

    private static void AddStreamingAnimationAttributes( RenderTreeBuilder builder, ref int sequence, SvgChartStreamingAnimation animation )
    {
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-animation", "true" );
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-version", animation.Version.ToString( CultureInfo.InvariantCulture ) );
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-offset", SvgChartRenderHelpers.Format( animation.OffsetX ) );
        builder.AddAttribute( sequence++, "data-svg-chart-streaming-duration", SvgChartRenderHelpers.FormatDuration( animation.Duration ) );
    }

    private string GetPlotClipPathId()
    {
        return $"{ElementId}-plot-clip";
    }

    private string GetCategoryAxisLabelsClipPathId()
    {
        return $"{ElementId}-category-axis-labels-clip";
    }

    private SvgChartStreamingAnimation ResolveStreamingAnimation( SvgChartRenderModel model, SvgChartPlotArea plot )
    {
        var streaming = ResolveStreaming();

        if ( streaming is null
             || !streaming.Enabled
             || !SvgChartStreamingResolver.IsAnimationEnabled( streaming )
             || !streaming.VisibleDataPoints.HasValue
             || streaming.VisibleDataPoints.Value <= 0
             || model.Labels.Count == 0
             || !SupportsStreamingAnimation( model ) )
            return new( false, 0, TimeSpan.Zero, streamingAnimationVersion, null );

        var duration = ResolveStreamingAnimationDuration( streaming );

        if ( duration <= TimeSpan.Zero )
            return new( false, 0, TimeSpan.Zero, streamingAnimationVersion, null );

        var categoryWidth = GetCategoryWidth( plot, model );
        var offsetX = SvgChartStreamingResolver.IsReversed( streaming )
            ? categoryWidth
            : -categoryWidth;

        if ( streaming.IndexAxis == SvgChartIndexAxis.Y )
        {
            offsetX = -offsetX;
        }

        var animation = new SvgChartStreamingAnimation( true, offsetX, duration, streamingAnimationVersion, null );

        return animation with { Style = ResolveStreamingAnimationStyle( animation ) };
    }

    private static TimeSpan ResolveStreamingAnimationDuration( SvgChartStreamingOptions streaming )
    {
        if ( streaming.Animation?.Duration > TimeSpan.Zero )
            return streaming.Animation.Duration;

        if ( streaming.RefreshInterval > TimeSpan.Zero )
            return streaming.RefreshInterval;

        return TimeSpan.FromMilliseconds( 500 );
    }

    private SvgChartPlotArea BuildCurrentPlotArea( SvgChartRenderModel model )
    {
        var options = model.Options;
        var legend = ResolveLegend( options );
        var title = ResolveTitleOptions( options, titleComponents );
        var subtitle = ResolveSubtitleOptions( options, titleComponents );

        return BuildPlotArea( options, title, subtitle, legend.Visible && legend.Position == SvgChartLegendPosition.Top, legend.Visible && legend.Position == SvgChartLegendPosition.Bottom, model );
    }

    private static bool IsInsidePlot( double x, double y, SvgChartPlotArea plot )
    {
        return x >= plot.Left && x <= plot.Right && y >= plot.Top && y <= plot.Bottom;
    }

    private bool ShouldRenderPanViewport( SvgChartViewport viewport )
    {
        if ( AreViewportsEquivalent( panRenderedViewport, viewport ) )
            return false;

        var now = DateTimeOffset.UtcNow;

        return now - lastPanRender >= PanRenderInterval;
    }

    private static bool AreViewportsEquivalent( SvgChartViewport first, SvgChartViewport second )
    {
        if ( ReferenceEquals( first, second ) )
            return true;

        if ( first is null || second is null )
            return false;

        return AreViewportValuesEquivalent( first.XMin, second.XMin )
            && AreViewportValuesEquivalent( first.XMax, second.XMax )
            && AreViewportValuesEquivalent( first.YMin, second.YMin )
            && AreViewportValuesEquivalent( first.YMax, second.YMax );
    }

    private static bool AreViewportValuesEquivalent( double? first, double? second )
    {
        if ( !first.HasValue || !second.HasValue )
            return first.HasValue == second.HasValue;

        return Math.Abs( first.Value - second.Value ) < 0.000001;
    }

    private async Task ApplyViewport( SvgChartViewport previousViewport, SvgChartViewport viewport )
    {
        internalViewport = CloneViewport( viewport );
        ClearTooltip();

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

        var fullModel = BuildModel( false, false );

        panning = true;
        panStartClientX = eventArgs.ClientX;
        panStartClientY = eventArgs.ClientY;
        panFullViewport = ResolveFullViewport( fullModel );
        panPlot = plot;
        panZoom = zoom;
        panStartViewport = ResolveEffectiveViewport( model, panFullViewport );
        panLastViewport = CloneViewport( panStartViewport );
        panRenderedViewport = CloneViewport( panStartViewport );
        lastPanRender = DateTimeOffset.MinValue;
    }

    private async Task HandleChartMouseMove( MouseEventArgs eventArgs )
    {
        if ( !panning )
            return;

        if ( panZoom?.Enabled != true || !panZoom.Pan || panStartViewport is null || panFullViewport is null || panPlot is null )
            return;

        var nextViewport = PanViewport( panStartViewport, panFullViewport, panZoom, panPlot, eventArgs.ClientX - panStartClientX, eventArgs.ClientY - panStartClientY );

        panLastViewport = CloneViewport( nextViewport );

        if ( !ShouldRenderPanViewport( nextViewport ) )
            return;

        var previousViewport = CloneViewport( panRenderedViewport ?? panStartViewport );

        panRenderedViewport = CloneViewport( nextViewport );
        lastPanRender = DateTimeOffset.UtcNow;

        await ApplyViewport( previousViewport, nextViewport );
    }

    private async Task HandleChartMouseUp( MouseEventArgs eventArgs )
    {
        if ( !panning )
            return;

        panning = false;

        if ( panStartViewport is not null && panLastViewport is not null )
        {
            if ( !AreViewportsEquivalent( panRenderedViewport, panLastViewport ) )
            {
                var previousViewport = CloneViewport( panRenderedViewport ?? panStartViewport );

                panRenderedViewport = CloneViewport( panLastViewport );
                await ApplyViewport( previousViewport, panLastViewport );
            }

            await NotifyPanned( panStartViewport, panLastViewport );
        }

        panStartViewport = null;
        panLastViewport = null;
        panRenderedViewport = null;
        panFullViewport = null;
        panPlot = null;
        panZoom = null;
        lastPanRender = DateTimeOffset.MinValue;
    }

    private async Task HandleChartMouseLeave( MouseEventArgs eventArgs )
    {
        await HandleChartMouseUp( eventArgs );
    }

    /// <summary>
    /// Replaces the chart data and refreshes the chart.
    /// </summary>
    /// <param name="data">The chart data to render.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SetData( SvgChartData<double?> data )
    {
        internalChartData = NormalizeData( data );
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Replaces the chart options and refreshes the chart.
    /// </summary>
    /// <param name="options">The chart options to apply.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SetOptions( SvgChartOptions options )
    {
        Options = options;
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a category label to the chart data.
    /// </summary>
    /// <param name="label">The category label to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddLabel( object label )
    {
        var data = EnsureInternalData();
        data.Labels.Add( label );
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a data series to the chart.
    /// </summary>
    /// <param name="series">The series to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddSeries( SvgChartSeriesData<double?> series )
    {
        if ( series is null )
            return Task.CompletedTask;

        var data = EnsureInternalData();
        data.Series.Add( NormalizeSeries( series ) );
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a data series by name.
    /// </summary>
    /// <param name="name">The series name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RemoveSeries( string name )
    {
        var data = EnsureInternalData();
        data.Series.RemoveAll( x => x.Name == name );
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a value to an existing data series.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddValue( string seriesName, double? value )
    {
        var data = EnsureInternalData();
        var series = data.Series.FirstOrDefault( x => x.Name == seriesName );

        if ( series is not null )
        {
            NormalizeSeries( series );
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
            NormalizeSeries( series );

            while ( series.Values.Count < previousCount )
                series.Values.Add( null );
        }

        data.Labels.Add( label );

        foreach ( var series in data.Series )
            series.Values.Add( streamingValues.TryGetValue( series.Name, out var value ) ? value : null );

        if ( streaming.Enabled && SvgChartStreamingResolver.IsAnimationEnabled( streaming ) && streaming.VisibleDataPoints.HasValue )
        {
            streamingAnimationVersion++;
        }

        TrimStreamingData( data, streaming, label );

        await RefreshStreaming( streaming );

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

    /// <summary>
    /// Updates a value in an existing data series.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <param name="index">The zero-based value index.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SetValue( string seriesName, int index, double? value )
    {
        var series = EnsureInternalData().Series.FirstOrDefault( x => x.Name == seriesName );

        if ( series is not null )
        {
            NormalizeSeries( series );

            if ( index >= 0 && index < series.Values.Count )
            {
                series.Values[index] = value;
                StateHasChanged();
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Toggles the visibility of a data series.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ToggleSeries( string seriesName )
    {
        if ( !hiddenSeries.Add( seriesName ) )
            hiddenSeries.Remove( seriesName );

        ClearTooltip();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Shows a hidden data series.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ShowSeries( string seriesName )
    {
        hiddenSeries.Remove( seriesName );
        ClearTooltip();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Hides a visible data series.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task HideSeries( string seriesName )
    {
        hiddenSeries.Add( seriesName );
        ClearTooltip();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Toggles the visibility of a radial data point.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <param name="pointIndex">The zero-based data point index.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ToggleDataPoint( string seriesName, int pointIndex )
    {
        var key = GetDataPointKey( seriesName, pointIndex );

        if ( !hiddenDataPoints.Add( key ) )
            hiddenDataPoints.Remove( key );

        ClearTooltip();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Shows a hidden radial data point.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <param name="pointIndex">The zero-based data point index.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ShowDataPoint( string seriesName, int pointIndex )
    {
        hiddenDataPoints.Remove( GetDataPointKey( seriesName, pointIndex ) );
        ClearTooltip();
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Hides a visible radial data point.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <param name="pointIndex">The zero-based data point index.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task HideDataPoint( string seriesName, int pointIndex )
    {
        hiddenDataPoints.Add( GetDataPointKey( seriesName, pointIndex ) );
        ClearTooltip();
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

    /// <summary>
    /// Refreshes the chart.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Update()
    {
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Refreshes the chart after its container size changes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Resize()
    {
        StateHasChanged();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the active chart viewport.
    /// </summary>
    /// <param name="viewport">The viewport to apply.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetViewport( SvgChartViewport viewport )
    {
        var model = BuildModel();
        var fullViewport = ResolveFullViewport( BuildModel( false, false ) );
        var previousViewport = ResolveEffectiveViewport( model, fullViewport );
        var nextViewport = CloneViewport( viewport );

        await ApplyViewport( previousViewport, nextViewport );
    }

    /// <summary>
    /// Resets the chart zoom viewport.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ResetZoom()
    {
        var model = BuildModel();
        var fullViewport = ResolveFullViewport( BuildModel( false, false ) );
        var previousViewport = ResolveEffectiveViewport( model, fullViewport );

        await ApplyViewport( previousViewport, null );
        await NotifyZoomed( previousViewport, fullViewport, SvgChartZoomSource.Api );
    }

    /// <summary>
    /// Zooms into the chart from the center of the plot area.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ZoomIn()
    {
        await ZoomBy( 0.82 );
    }

    /// <summary>
    /// Zooms out from the center of the plot area.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ZoomOut()
    {
        await ZoomBy( 1.22 );
    }

    /// <summary>
    /// Clears chart labels, values, visibility state, tooltip state, viewport, and streaming state.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Clear()
    {
        var data = EnsureInternalData();

        data.Labels.Clear();

        foreach ( var series in data.Series )
        {
            NormalizeSeries( series );

            series.Values.Clear();
            series.XValues.Clear();
            series.YValues.Clear();
            series.RadiusValues.Clear();
            series.Colors.Clear();
        }

        hiddenSeries.Clear();
        hiddenDataPoints.Clear();
        ClearTooltip();
        internalViewport = null;
        panning = false;
        panStartViewport = null;
        panLastViewport = null;
        panRenderedViewport = null;
        panFullViewport = null;
        panPlot = null;
        panZoom = null;
        lastPanRender = DateTimeOffset.MinValue;
        streamingAnimationVersion = 0;
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

    /// <summary>
    /// Exports the chart root SVG markup as a string.
    /// </summary>
    /// <returns>A value task containing the SVG markup.</returns>
    public ValueTask<string> ToSvgString()
    {
        var model = BuildModel();
        var options = ResolveOptions();
        var title = ResolveTitleOptions( options, titleComponents );
        var role = HtmlEncoder.Default.Encode( ResolveAccessibilityRole() );
        var ariaLabel = HtmlEncoder.Default.Encode( ResolveAccessibilityLabel( title ) );
        var accessibilityTitle = HtmlEncoder.Default.Encode( AccessibilityTitle ?? string.Empty );
        var description = HtmlEncoder.Default.Encode( ResolveAccessibilityDescription( model ) );
        var tabIndex = TabIndex.HasValue ? $" tabindex=\"{TabIndex.Value}\"" : string.Empty;
        var titleMarkup = string.IsNullOrWhiteSpace( AccessibilityTitle ) ? string.Empty : $"<title>{accessibilityTitle}</title>";
        var descriptionMarkup = string.IsNullOrWhiteSpace( description ) ? string.Empty : $"<desc>{description}</desc>";

        return ValueTask.FromResult( $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {Format( options.Width )} {Format( options.Height )}\" role=\"{role}\" aria-label=\"{ariaLabel}\"{tabIndex}>{titleMarkup}{descriptionMarkup}</svg>" );
    }

    private void RenderAccessibilityText( RenderTreeBuilder builder, ref int sequence, SvgChartRenderModel model )
    {
        if ( !string.IsNullOrWhiteSpace( AccessibilityTitle ) )
        {
            builder.OpenElement( sequence++, "title" );
            builder.AddContent( sequence++, AccessibilityTitle );
            builder.CloseElement();
        }

        var description = ResolveAccessibilityDescription( model );

        if ( !string.IsNullOrWhiteSpace( description ) )
        {
            builder.OpenElement( sequence++, "desc" );
            builder.AddContent( sequence++, description );
            builder.CloseElement();
        }
    }

    private string ResolveAccessibilityRole()
    {
        return string.IsNullOrWhiteSpace( Role ) ? "img" : Role;
    }

    private string ResolveAccessibilityLabel( SvgChartTextOptions title )
    {
        if ( !string.IsNullOrWhiteSpace( AriaLabel ) )
            return AriaLabel;

        return IsTextVisible( title ) ? title.Text : "SVG chart";
    }

    private string ResolveAccessibilityDescription( SvgChartRenderModel model )
    {
        return AccessibilityDescription ?? $"{model.Series.Count} series, {model.Labels.Count} categories.";
    }

    private SvgChartData<double?> EnsureInternalData()
    {
        if ( internalChartData is not null )
            return NormalizeData( internalChartData );

        if ( Data is not null )
        {
            internalChartData = NormalizeData( Data );
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
                Colors = x.PointColors?.Select( color => (Color)color ).ToList() ?? [],
                Interpolation = x.Interpolation,
                Tension = x.Tension,
                Hidden = x.Hidden
            } ).ToList()
        };

        return NormalizeData( internalChartData );
    }

    private static SvgChartSeriesData<double?> EnsureSeries( SvgChartData<double?> data, string seriesName, int valueCount )
    {
        data = NormalizeData( data );

        var series = data.Series.FirstOrDefault( x => x.Name == seriesName );

        if ( series is null )
        {
            series = new()
            {
                Name = seriesName
            };

            data.Series.Add( series );
        }

        NormalizeSeries( series );

        while ( series.Values.Count < valueCount )
            series.Values.Add( null );

        return series;
    }

    private static SvgChartData<double?> NormalizeData( SvgChartData<double?> data )
    {
        data ??= new();
        data.Labels ??= [];
        data.Series ??= [];
        data.Series.RemoveAll( x => x is null );

        foreach ( var series in data.Series )
            NormalizeSeries( series );

        return data;
    }

    private static SvgChartSeriesData<double?> NormalizeSeries( SvgChartSeriesData<double?> series )
    {
        if ( series is null )
            return null;

        series.Values ??= [];
        series.XValues ??= [];
        series.YValues ??= [];
        series.RadiusValues ??= [];
        series.Colors ??= [];

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
        data = NormalizeData( data );

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
        if ( values is not null && index >= 0 && index < values.Count )
            values.RemoveAt( index );
    }

    private Task RefreshStreaming( SvgChartStreamingOptions streaming )
    {
        ClearTooltip();

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

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private BlazoriseLicenseChecker ComponentLicenseChecker { get; set; }

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
    /// Specifies chart animation options.
    /// </summary>
    [Parameter] public SvgChartAnimationOptions Animation { get; set; }

    /// <summary>
    /// Occurs when a rendered chart point is clicked.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when a rendered chart point is hovered.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Hovered { get; set; }

    /// <summary>
    /// Specifies the root SVG role used by assistive technologies.
    /// </summary>
    [Parameter] public string Role { get; set; } = "img";

    /// <summary>
    /// Specifies the accessible label for the root SVG element.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; }

    /// <summary>
    /// Specifies the optional SVG title element used by assistive technologies.
    /// </summary>
    [Parameter] public string AccessibilityTitle { get; set; }

    /// <summary>
    /// Specifies the optional SVG description element used by assistive technologies.
    /// </summary>
    [Parameter] public string AccessibilityDescription { get; set; }

    /// <summary>
    /// Specifies the root SVG tabindex when keyboard focus should be enabled.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Specifies the content to render inside the chart, including declarative axes, series, and options.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}