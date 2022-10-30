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

        public ValueTask<string> CropBase64( ElementReference elementRef, string elementId, CropOptions options )
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
                    ImageSmoothingQuality.Low => "low",
                    ImageSmoothingQuality.Medium => "medium",
                    ImageSmoothingQuality.High => "high",
                    _ => "low"
                }
            };

            return InvokeSafeAsync<string>( "cropBase64", elementRef, elementId, cropOptions );
        }

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.ImageCropper/cropper.js?v={VersionProvider.Version}";
    }
}
