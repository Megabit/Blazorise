#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Cropper;

internal class JSCropperModule : BaseJSModule, IJSDestroyableModule
{
    public JSCropperModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public ValueTask Initialize( DotNetObjectReference<CropperAdapter> adapterReference, ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initialize", adapterReference, elementRef, elementId, options );

    public ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    public ValueTask UpdateOptions( ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "updateOptions", elementRef, elementId, options );

    public ValueTask InitializeViewer( ElementReference cropperElementRef, string cropperElementId, ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initializeViewer", cropperElementRef, cropperElementId, elementRef, elementId, options );

    public ValueTask<string> CropBase64( ElementReference elementRef, string elementId, CropperCropOptions options )
    {
        var cropOptions = new
        {
            width = options.Width,
            height = options.Height,
        };

        return InvokeSafeAsync<string>( "cropBase64", elementRef, elementId, cropOptions );
    }

    public ValueTask Move( ElementReference elementRef, string elementId, int x, int y )
        => InvokeSafeVoidAsync( "move", elementRef, elementId, x, y );

    public ValueTask MoveTo( ElementReference elementRef, string elementId, int x, int y )
        => InvokeSafeVoidAsync( "moveTo", elementRef, elementId, x, y );

    public ValueTask Zoom( ElementReference elementRef, string elementId, double scale )
        => InvokeSafeVoidAsync( "zoom", elementRef, elementId, scale );

    public ValueTask Rotate( ElementReference elementRef, string elementId, double angle )
        => InvokeSafeVoidAsync( "rotate", elementRef, elementId, angle );

    public ValueTask Scale( ElementReference elementRef, string elementId, int x, int y )
        => InvokeSafeVoidAsync( "scale", elementRef, elementId, x, y );

    public ValueTask Center( ElementReference elementRef, string elementId, string size )
        => InvokeSafeVoidAsync( "center", elementRef, elementId, size );

    public ValueTask ResetSelection( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "resetSelection", elementRef, elementId );

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Cropper/blazorise.cropper.js?v={VersionProvider.Version}";
}
