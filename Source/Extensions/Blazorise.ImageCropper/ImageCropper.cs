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
        private JSObjectUrl createdObjectUrl;
        private ImageCropperInstance cropper;

        [Parameter] public double Ratio { get; set; } = 1.0;
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
                if ( createdObjectUrl != null )
                {
                    await createdObjectUrl.DisposeAsync();
                }

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
            if ( firstRender )
            {
                cropper = await JSModule.CreateCropperAsync( ElementRef );
                await cropper.UpdateAsync( Ratio );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            if ( Rendered )
            {
                await cropper.UpdateAsync( Ratio );
            }

            await base.SetParametersAsync( parameters );
        }

        /// <inheritdoc/>
        protected override Task OnInitializedAsync()
        {
            JSModule ??= new JSCropperModule( JSRuntime, VersionProvider );

            return base.OnInitializedAsync();
        }

        public async Task<string> ExtractBase64Image( int width, int height )
        {
            var base64data = await cropper.CropImage( width, height );
            return base64data;
        }
    }
}