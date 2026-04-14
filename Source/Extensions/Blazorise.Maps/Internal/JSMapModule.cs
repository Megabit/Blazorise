#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Maps;

internal class JSMapModule : BaseJSModule, IJSDestroyableModule
{
    public JSMapModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public ValueTask Initialize( DotNetObjectReference<MapAdapter> adapterReference, ElementReference elementRef, string elementId, MapJSOptions options )
        => InvokeSafeVoidAsync( "initialize", adapterReference, elementRef, elementId, options );

    public ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    public ValueTask SetView( ElementReference elementRef, string elementId, MapView view, MapAnimationOptions options )
        => InvokeSafeVoidAsync( "setView", elementRef, elementId, view, options );

    public ValueTask PanTo( ElementReference elementRef, string elementId, MapCoordinate center, MapAnimationOptions options )
        => InvokeSafeVoidAsync( "panTo", elementRef, elementId, center, options );

    public ValueTask FitBounds( ElementReference elementRef, string elementId, MapBounds bounds, MapFitBoundsOptions options )
        => InvokeSafeVoidAsync( "fitBounds", elementRef, elementId, bounds, options );

    public ValueTask ZoomIn( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "zoomIn", elementRef, elementId );

    public ValueTask ZoomOut( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "zoomOut", elementRef, elementId );

    public ValueTask InvalidateSize( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "invalidateSize", elementRef, elementId );

    public ValueTask SetLayer( ElementReference elementRef, string elementId, MapLayerDefinition layer )
        => InvokeSafeVoidAsync( "setLayer", elementRef, elementId, layer );

    public ValueTask RemoveLayer( ElementReference elementRef, string elementId, string layerId )
        => InvokeSafeVoidAsync( "removeLayer", elementRef, elementId, layerId );

    public ValueTask<MapView> GetView( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<MapView>( "getView", elementRef, elementId );

    public ValueTask<MapBounds> GetBounds( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<MapBounds>( "getBounds", elementRef, elementId );

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Maps/blazorise.maps.js?v={VersionProvider.Version}";
}