#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
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
        /// Define the view mode of the cropper
        /// </summary>
        [Parameter] public ViewMode ViewMode { get; set; } = ViewMode.Mode0;

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

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box starts to change.
        /// </summary>
        [Parameter] public EventCallback CropStart { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box is changing.
        /// </summary>
        [Parameter] public EventCallback CropMove { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box stops changing.
        /// </summary>
        [Parameter] public EventCallback CropEnd { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box changes.
        /// </summary>
        [Parameter] public EventCallback Crop { get; set; }

        /// <summary>
        /// This event fires when a cropper instance starts to zoom in or zoom out its canvas (image wrapper).
        /// </summary>
        [Parameter] public EventCallback Zoom { get; set; }

        /// <summary>
        /// The cropper radius
        /// </summary>
        [Parameter] public int? Radius { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private IVersionProvider VersionProvider { get; set; }

        private JSCropperModule JSModule { get; set; }
        private DotNetObjectReference<ImageCropperAdapter> adapter;
        private JSCropperOptions appliedOptions;

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

                if ( adapter != null )
                {
                    adapter.Dispose();
                    adapter = null;
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
                GetOptions( out var options );
                await JSModule.Initialize( adapter, ElementRef, ElementId, options );
            }
        }

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( Rendered )
            {
                if ( GetOptions( out var options ) )
                {
                    await JSModule.UpdateOptions( ElementRef, ElementId, options );
                }
            }
        }

        private bool GetOptions( out JSCropperOptions options )
        {
            options = new()
            {
                AspectRatio = Ratio.Value,
                Preview = PreviewSelector,
                ViewMode = (int)ViewMode,
                Radius = Radius
            };

            if ( appliedOptions != options )
            {
                appliedOptions = options;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override Task OnInitializedAsync()
        {
            JSModule ??= new JSCropperModule( JSRuntime, VersionProvider );
            adapter ??= DotNetObjectReference.Create( new ImageCropperAdapter( this ) );

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

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-image-cropper-source" );
            base.BuildClasses( builder );
        }

        internal async ValueTask NotifyCropStart() => await CropStart.InvokeAsync();
        internal async ValueTask NotifyCropMove() => await CropMove.InvokeAsync();
        internal async ValueTask NotifyCropEnd() => await CropEnd.InvokeAsync();
        internal async ValueTask NotifyCrop() => await Crop.InvokeAsync();
        internal async ValueTask NotifyZoom() => await Zoom.InvokeAsync();
    }
}