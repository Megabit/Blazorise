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
    /// <summary>
    /// Blazorise Image Cropper component based on <see href="https://fengyuanchen.github.io/cropperjs/">CropperJS</see>.
    /// </summary>
    public class ImageCropper : BaseComponent, IAsyncDisposable
    {
        /// <summary>
        /// The aspect ratio of the image cropper
        /// </summary>
        [Parameter] public AspectRatio Ratio { get; set; } = AspectRatio.Ratio1_1;

        /// <summary>
        /// The original image source
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// The alt text of the image
        /// </summary>
        [Parameter] public string Alt { get; set; }

        /// <summary>
        /// The alt text of the image
        /// </summary>
        [Parameter] public string PreviewSelector { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private IVersionProvider VersionProvider { get; set; }

        private JSCropperModule JSModule { get; set; }
        private DotNetObjectReference<ImageCropper> DotNetObjectRef { get; set; }

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
                await JSModule.SafeDestroy( ElementRef, ElementId );

                await JSModule.SafeDisposeAsync();

                if ( DotNetObjectRef != null )
                {
                    DotNetObjectRef.Dispose();
                    DotNetObjectRef = null;
                }
            }

            await base.DisposeAsync( disposing );
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            await base.OnAfterRenderAsync( firstRender );

            if ( firstRender )
            {
                await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, GetOptions() );
            }
        }

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( Rendered )
            {
                await JSModule.UpdateOptions( ElementRef, ElementId, GetOptions() );
            }
        }

        private JSCropperOptions GetOptions() => new()
        {
            AspectRatio = Ratio.Value,
            Preview = PreviewSelector
        };

        /// <inheritdoc/>
        protected override Task OnInitializedAsync()
        {
            JSModule ??= new JSCropperModule( JSRuntime, VersionProvider );
            DotNetObjectRef ??= DotNetObjectReference.Create( this );

            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Get the cropped image as Base64 image.
        /// </summary>
        /// <param name="options">the cropping options</param>
        /// <returns>the cropped image</returns>
        public async Task<string> CropAsBase64ImageAsync( CropOptions options )
        {
            return await JSModule.CropBase64( ElementRef, ElementId, options );
        }
    }
}