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

        public async ValueTask Initialize( DotNetObjectReference<ImageCropper> dotNetObjectReference, ElementReference elementRef, string elementId, JSCropperOptions options )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );
        }

        public async ValueTask Destroy( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
        }

        public async ValueTask UpdateOptions( ElementReference elementRef, string elementId, JSCropperOptions options )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "updateOptions", elementRef, elementId, options );
        }

        public async ValueTask<string> CropBase64( ElementReference elementRef, string elementId, CropOptions options )
        {
            var moduleInstance = await Module;

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

            return await moduleInstance.InvokeAsync<string>( "cropBase64", elementRef, elementId, cropOptions );
        }

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.ImageCropper/cropper.js?v={VersionProvider.Version}";

    }
}
