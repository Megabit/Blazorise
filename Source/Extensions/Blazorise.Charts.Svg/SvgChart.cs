#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
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
        var title = ResolveTitleOptions( options, titleComponents );
        var subtitle = ResolveSubtitleOptions( options, titleComponents );
        var hasTopLegend = legend.Visible && legend.Position == SvgChartLegendPosition.Top;
        var hasBottomLegend = legend.Visible && legend.Position == SvgChartLegendPosition.Bottom;
        var plot = BuildPlotArea( options, title, subtitle, hasTopLegend, hasBottomLegend );
        var streamingAnimation = ResolveStreamingAnimation( model, plot );
        var pluginContext = CreatePluginRenderContext( model, plot );
        var seriesRendererContext = new SvgChartSeriesRendererContext( pluginContext );
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

        SvgChartRenderHelpers.AddFontFamilyAttribute( builder, ref sequence, options.Font?.Family );

        RenderFocusStyles( builder, ref sequence );
        RenderPlugins( builder, ref sequence, pluginContext, SvgChartRenderLayer.Background );

        SvgChartTextRenderer.Render( builder, ref sequence, options, title, subtitle );

        if ( hasTopLegend )
            SvgChartLegendRenderer.Render( builder, ref sequence, model, options, 48, this, ToggleSeries, ToggleDataPoint, IsDataPointHidden );

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
            SvgChartLegendRenderer.Render( builder, ref sequence, model, options, options.Height - 30, this, ToggleSeries, ToggleDataPoint, IsDataPointHidden );

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
            hiddenSeries ).Build( applyStreamingViewport, applyZoomViewport );
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
        return value.ToString( "0.###", CultureInfo.InvariantCulture );
    }

    private static string FormatDuration( TimeSpan value )
    {
        return $"{value.TotalSeconds.ToString( "0.###", CultureInfo.InvariantCulture )}s";
    }

    private string ResolveStreamingAnimationStyle( SvgChartStreamingAnimation animation )
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

        return BuildPlotArea( options, title, subtitle, legend.Visible && legend.Position == SvgChartLegendPosition.Top, legend.Visible && legend.Position == SvgChartLegendPosition.Bottom );
    }

    private static bool IsInsidePlot( double x, double y, SvgChartPlotArea plot )
    {
        return x >= plot.Left && x <= plot.Right && y >= plot.Top && y <= plot.Bottom;
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

        if ( streaming.Enabled && SvgChartStreamingResolver.IsAnimationEnabled( streaming ) && streaming.VisibleDataPoints.HasValue )
        {
            streamingAnimationVersion++;
            streamingAnimationActive = false;
        }

        TrimStreamingData( data, streaming, label );

        await RefreshStreaming( streaming );

        if ( streaming.Enabled && SvgChartStreamingResolver.IsAnimationEnabled( streaming ) && streaming.VisibleDataPoints.HasValue )
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
        var title = ResolveTitleOptions( options, titleComponents );
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

    [Inject] private IJSRuntime JSRuntime { get; set; }

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

    #endregion
}