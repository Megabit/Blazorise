#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Maps;

/// <summary>
/// Displays an interactive map with provider-neutral layers and shapes.
/// </summary>
public partial class Map : BaseComponent, IAsyncDisposable
{
    #region Members

    private readonly Dictionary<string, MapLayer> layers = new( StringComparer.Ordinal );

    private DotNetObjectReference<MapAdapter> adapter;

    private bool initialized;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        JSModule ??= new JSMapModule( JSRuntime, VersionProvider, BlazoriseOptions );
        adapter ??= DotNetObjectReference.Create( new MapAdapter( this ) );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender )
        {
            await JSModule.Initialize( adapter, ElementRef, ElementId, CreateJSOptions() );

            initialized = true;

            if ( Ready.HasDelegate )
                await Ready.InvokeAsync( new MapReadyEventArgs( this ) );
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-map" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            if ( adapter is not null )
            {
                adapter.Dispose();
                adapter = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    internal void RegisterLayer( MapLayer layer )
    {
        if ( layer is null || string.IsNullOrWhiteSpace( layer.Id ) )
            return;

        layers[layer.Id] = layer;

        if ( initialized )
            ExecuteAfterRender( async () => await SetLayer( layer ) );
    }

    internal void UnregisterLayer( MapLayer layer )
    {
        if ( layer is null || string.IsNullOrWhiteSpace( layer.Id ) )
            return;

        layers.Remove( layer.Id );

        if ( initialized )
            ExecuteAfterRender( async () => await JSModule.RemoveLayer( ElementRef, ElementId, layer.Id ) );
    }

    internal void NotifyLayerChanged( MapLayer layer )
    {
        if ( layer is null || !initialized )
            return;

        ExecuteAfterRender( async () => await SetLayer( layer ) );
    }

    internal async ValueTask NotifyClicked( MapMouseEventArgs eventArgs )
    {
        if ( Clicked.HasDelegate )
            await Clicked.InvokeAsync( eventArgs );
    }

    internal async ValueTask NotifyDoubleClicked( MapMouseEventArgs eventArgs )
    {
        if ( DoubleClicked.HasDelegate )
            await DoubleClicked.InvokeAsync( eventArgs );
    }

    internal async ValueTask NotifyContextMenu( MapMouseEventArgs eventArgs )
    {
        if ( ContextMenu.HasDelegate )
            await ContextMenu.InvokeAsync( eventArgs );
    }

    internal async ValueTask NotifyViewChanged( MapViewChangedEventArgs eventArgs )
    {
        View = eventArgs.View;

        if ( ViewChanged.HasDelegate )
            await ViewChanged.InvokeAsync( eventArgs.View );

        if ( ViewChangedDetailed.HasDelegate )
            await ViewChangedDetailed.InvokeAsync( eventArgs );
    }

    internal async ValueTask NotifyMarkerClicked( string layerId, string itemId, MapMouseEventArgs mouseEventArgs )
    {
        if ( layers.TryGetValue( layerId, out var layer ) )
            await layer.NotifyClicked( itemId, mouseEventArgs );
    }

    internal async ValueTask NotifyMarkerDragged( string layerId, string itemId, MapCoordinate coordinate )
    {
        if ( layers.TryGetValue( layerId, out var layer ) )
            await layer.NotifyDragged( itemId, coordinate );
    }

    private ValueTask SetLayer( MapLayer layer )
        => JSModule.SetLayer( ElementRef, ElementId, layer.ToDefinition() );

    private MapJSOptions CreateJSOptions()
    {
        return new()
        {
            Version = VersionProvider.Version,
            View = View ?? new MapView(),
            Options = Options ?? new MapOptions(),
            Layers = layers.Values.Select( x => x.ToDefinition() ).ToList(),
        };
    }

    /// <summary>
    /// Sets the map view to the specified center and zoom.
    /// </summary>
    /// <param name="center">The center coordinate.</param>
    /// <param name="zoom">The zoom level.</param>
    /// <param name="options">Optional animation options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask SetView( MapCoordinate center, double zoom, MapAnimationOptions options = null )
    {
        View = new MapView
        {
            Center = center,
            Zoom = zoom,
        };

        return initialized
            ? JSModule.SetView( ElementRef, ElementId, View, options )
            : ValueTask.CompletedTask;
    }

    /// <summary>
    /// Pans the map to the specified center coordinate.
    /// </summary>
    /// <param name="center">The center coordinate.</param>
    /// <param name="options">Optional animation options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask PanTo( MapCoordinate center, MapAnimationOptions options = null )
    {
        View ??= new MapView();
        View.Center = center;

        return initialized
            ? JSModule.PanTo( ElementRef, ElementId, center, options )
            : ValueTask.CompletedTask;
    }

    /// <summary>
    /// Fits the map view to the specified bounds.
    /// </summary>
    /// <param name="bounds">The bounds to fit.</param>
    /// <param name="options">Optional fit bounds options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask FitBounds( MapBounds bounds, MapFitBoundsOptions options = null )
        => initialized
            ? JSModule.FitBounds( ElementRef, ElementId, bounds, options )
            : ValueTask.CompletedTask;

    /// <summary>
    /// Increases the current zoom level.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask ZoomIn()
        => initialized
            ? JSModule.ZoomIn( ElementRef, ElementId )
            : ValueTask.CompletedTask;

    /// <summary>
    /// Decreases the current zoom level.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask ZoomOut()
        => initialized
            ? JSModule.ZoomOut( ElementRef, ElementId )
            : ValueTask.CompletedTask;

    /// <summary>
    /// Invalidates the map size after its container size changes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask InvalidateSize()
        => initialized
            ? JSModule.InvalidateSize( ElementRef, ElementId )
            : ValueTask.CompletedTask;

    /// <summary>
    /// Reads the current center, zoom level, and visible bounds from the rendered map.
    /// </summary>
    /// <returns>The current map view.</returns>
    public ValueTask<MapView> GetView()
        => initialized
            ? JSModule.GetView( ElementRef, ElementId )
            : new( View );

    /// <summary>
    /// Reads the currently visible geographic bounds from the rendered map.
    /// </summary>
    /// <returns>The current visible bounds.</returns>
    public ValueTask<MapBounds> GetBounds()
        => initialized
            ? JSModule.GetBounds( ElementRef, ElementId )
            : new( View?.Bounds ?? default );

    #endregion

    #region Properties

    internal JSMapModule JSModule { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Defines the map center, zoom level, and optional bounds.
    /// </summary>
    [Parameter] public MapView View { get; set; } = new();

    /// <summary>
    /// Notifies when the bound map view changes.
    /// </summary>
    [Parameter] public EventCallback<MapView> ViewChanged { get; set; }

    /// <summary>
    /// Configures map interaction, provider, zoom limits, and controls.
    /// </summary>
    [Parameter] public MapOptions Options { get; set; } = new();

    /// <summary>
    /// Runs after the map provider has initialized the rendered map.
    /// </summary>
    [Parameter] public EventCallback<MapReadyEventArgs> Ready { get; set; }

    /// <summary>
    /// Reports view changes with bounds and the reason for the change.
    /// </summary>
    [Parameter] public EventCallback<MapViewChangedEventArgs> ViewChangedDetailed { get; set; }

    /// <summary>
    /// Handles single-click interaction on the map surface.
    /// </summary>
    [Parameter] public EventCallback<MapMouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Handles double-click interaction on the map surface.
    /// </summary>
    [Parameter] public EventCallback<MapMouseEventArgs> DoubleClicked { get; set; }

    /// <summary>
    /// Handles context-menu interaction on the map surface.
    /// </summary>
    [Parameter] public EventCallback<MapMouseEventArgs> ContextMenu { get; set; }

    /// <summary>
    /// Defines tile layers, markers, shapes, and other map content rendered inside the map.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}