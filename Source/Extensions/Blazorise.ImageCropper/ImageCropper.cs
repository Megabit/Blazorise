#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.ImageCropper
{
    public class ImageCropper : BaseComponent, IAsyncDisposable
    {
        private JSImageCropper cropper;

        [Parameter] public AspectRatio Ratio { get; set; } = AspectRatio.Ratio1_1;
        [Parameter] public string Source { get; set; }
        [Parameter] public string Alt { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private IVersionProvider VersionProvider { get; set; }
        private JSCropperModule JSModule { get; set; }

        /// <inheritdoc/>
        protected override void BuildRenderTree( RenderTreeBuilder builder ) => builder
            .OpenElement( "img" )
            .Id( ElementId )
            .Attribute( "src", Source )
            .Attribute( "alt", Alt )
            .Class( ClassNames )
            .Style( StyleNames )
            .Attributes( Attributes )
            .ElementReferenceCapture( capturedRef => ElementRef = capturedRef )
            .CloseElement();

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
                if ( cropper != null )
                {
                    await cropper.DisposeAsync();
                }

                await JSModule.SafeDisposeAsync();
            }

            await base.DisposeAsync( disposing );

        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            await base.OnAfterRenderAsync( firstRender );

            if ( firstRender )
            {
                cropper = await JSModule.CreateCropperAsync( ElementRef );
                await RefreshCropperAsync();
            }
        }

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            await RefreshCropperAsync();
        }

        private async Task RefreshCropperAsync()
        {
            if ( Rendered )
            {
                await cropper.UpdateAsync( new() { AspectRatio = Ratio.Value } );
            }
        }

        /// <inheritdoc/>
        protected override Task OnInitializedAsync()
        {
            JSModule ??= new JSCropperModule( JSRuntime, VersionProvider );

            return base.OnInitializedAsync();
        }

        public async Task<string> CropAsync( int width, int height )
        {
            return await cropper.CropImage( width, height );
        }
    }
}