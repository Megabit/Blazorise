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

        public ValueTask Initialize( DotNetObjectReference<ImageCropperAdapter> adapterReference, ElementReference elementRef, string elementId, JSCropperOptions options )
            => InvokeSafeVoidAsync( "initialize", adapterReference, elementRef, elementId, options );

        public ValueTask Destroy( ElementReference elementRef, string elementId )
            => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

        public ValueTask UpdateOptions( ElementReference elementRef, string elementId, JSCropperOptions options )
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
                    ImageCropperImageSmoothingQuality.Low => "low",
                    ImageCropperImageSmoothingQuality.Medium => "medium",
                    ImageCropperImageSmoothingQuality.High => "high",
                    _ => "low"
                }
            };

            return InvokeSafeAsync<string>( "cropBase64", elementRef, elementId, cropOptions );
        }

        private ValueTask CallCropperAction( ElementReference elementRef, string elementId, string method, params object[] arguments )
        {
            object[] args = new object[] { elementRef, elementId, method }.Append( arguments ).ToArray();
            return InvokeSafeVoidAsync( "executeCropperAction", args );
        }

        public ValueTask Enable( ElementReference elementRef, string elementId, bool enabled ) => enabled
            ? CallCropperAction( elementRef, elementId, "enable" )
            : CallCropperAction( elementRef, elementId, "disable" );

        public ValueTask Replace( ElementReference elementRef, string elementId, string source )
            => CallCropperAction( elementRef, elementId, "replace", source );

        public ValueTask Move( ElementReference elementRef, string elementId, int offsetX, int offsetY )
            => CallCropperAction( elementRef, elementId, "move", offsetX, offsetY );

        public ValueTask MoveTo( ElementReference elementRef, string elementId, int x, int y )
            => CallCropperAction( elementRef, elementId, "moveTo", x, y );

        public ValueTask Zoom( ElementReference elementRef, string elementId, double ratio )
            => CallCropperAction( elementRef, elementId, "zoom", ratio );

        public ValueTask ZoomTo( ElementReference elementRef, string elementId, double ratio )
            => CallCropperAction( elementRef, elementId, "zoomTo", ratio );

        public ValueTask ZoomTo( ElementReference elementRef, string elementId, double ratio, int x, int y )
            => CallCropperAction( elementRef, elementId, "zoomTo", ratio, new { x, y } );

        public ValueTask Rotate( ElementReference elementRef, string elementId, int degree )
            => CallCropperAction( elementRef, elementId, "rotate", degree );

        public ValueTask RotateTo( ElementReference elementRef, string elementId, int degree )
            => CallCropperAction( elementRef, elementId, "rotateTo", degree );

        public ValueTask Scale( ElementReference elementRef, string elementId, int scaleX, int scaleY )
            => CallCropperAction( elementRef, elementId, "scale", scaleX, scaleY );

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.ImageCropper/blazorise.imagecropper.js?v={VersionProvider.Version}";
    }
}
