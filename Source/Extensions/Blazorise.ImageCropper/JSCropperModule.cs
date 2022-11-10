using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.ImageCropper
{
    internal class JSCropperModule : BaseJSModule, IJSDestroyableModule
    {
        public JSCropperModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public ValueTask Initialize( DotNetObjectReference<ImageCropperAdapter> adapterReference, ElementReference elementRef, string elementId, object options )
            => InvokeSafeVoidAsync( "initialize", adapterReference, elementRef, elementId, options );

        public ValueTask Destroy( ElementReference elementRef, string elementId )
            => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

        public ValueTask UpdateOptions( ElementReference elementRef, string elementId, object options )
            => InvokeSafeVoidAsync( "updateOptions", elementRef, elementId, options );

        public ValueTask<string> CropBase64( ElementReference elementRef, string elementId, ImageCropperCropOptions options )
        {
            var cropOptions = new
            {
                width = options.Width,
                height = options.Height,
                minWidth = options.MinWidth,
                minHeight = options.MinHeight,
                maxWidth = options.MaxWidth?.ToString() ?? "Infinity",
                maxHeight = options.MaxHeight?.ToString() ?? "Infinity",
                fillColor = options.FillColor,
                imageSmoothingEnabled = options.ImageSmoothingEnabled,
                imageSmoothingQuality = options.ImageSmoothingQuality switch
                {
                    ImageCropperSmoothingQuality.Low => "low",
                    ImageCropperSmoothingQuality.Medium => "medium",
                    ImageCropperSmoothingQuality.High => "high",
                    _ => "low"
                }
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

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.ImageCropper/blazorise.imagecropper.js?v={VersionProvider.Version}";
    }
}
