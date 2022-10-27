using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.ImageCropper
{
    internal class JSCropperModule : BaseJSModule
    {
        public JSCropperModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public async ValueTask<JSImageCropper> CreateCropperAsync( ElementReference image )
        {
            var moduleInstance = await Module;

            var cropper = await moduleInstance.InvokeAsync<IJSObjectReference>( "createCropper", image );
            return new JSImageCropper( JSRuntime, cropper );
        }

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.ImageCropper/cropper.js?v={VersionProvider.Version}";

        public ValueTask Destroy( ElementReference elementRef, string elementId )
        {
            throw new System.NotImplementedException();
        }
    }
}
