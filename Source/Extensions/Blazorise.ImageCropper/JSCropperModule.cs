using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.ImageCropper
{
    public class JSCropperModule : BaseJSModule
    {
        public JSCropperModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public async ValueTask<ImageCropperInstance> CreateCropperAsync( ElementReference image )
        {
            var moduleInstance = await Module;

            var cropper = await moduleInstance.InvokeAsync<IJSObjectReference>( "createCropper", image );
            return new ImageCropperInstance( cropper );
        }

        public async ValueTask<JSObjectUrl> GetInputImageAsync( string inputElementId )
        {
            var moduleInstance = await Module;

            var url = await moduleInstance.InvokeAsync<string>( "getImageUrl", inputElementId );
            return new JSObjectUrl( url, JSRuntime );
        }

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.ImageCropper/cropper.js?v={VersionProvider.Version}";

        public ValueTask Destroy( ElementReference elementRef, string elementId )
        {
            throw new System.NotImplementedException();
        }
    }
}
