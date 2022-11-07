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
        #region Members

        private DotNetObjectReference<ImageCropperAdapter> adapter;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            if ( Rendered )
            {
                var sourceChanged = parameters.TryGetValue<string>( nameof( Source ), out var paramSource ) && paramSource != Source;
                var aspectRatioChanged = parameters.TryGetValue<ImageCropperAspectRatio>( nameof( AspectRatio ), out var paramAspectRatio ) && paramAspectRatio != AspectRatio;
                var viewModeChanged = parameters.TryGetValue<ImageCropperViewMode>( nameof( ViewMode ), out var paramViewMode ) && paramViewMode != ViewMode;
                var previewSelectorChanged = parameters.TryGetValue<string>( nameof( PreviewSelector ), out var paramPreviewSelector ) && paramPreviewSelector != PreviewSelector;
                var enabledChanged = parameters.TryGetValue<bool>( nameof( Enabled ), out var paramEnabled ) && paramEnabled != Enabled;

                if ( sourceChanged
                    || aspectRatioChanged
                    || viewModeChanged
                    || previewSelectorChanged
                    || enabledChanged )
                {
                    ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
                    {
                        Source = new { Changed = sourceChanged, Value = paramSource },
                        AspectRatio = new { Changed = aspectRatioChanged, Value = paramAspectRatio },
                        ViewMode = new { Changed = viewModeChanged, Value = paramViewMode },
                        Preview = new { Changed = previewSelectorChanged, Value = paramPreviewSelector },
                        Enabled = new { Changed = enabledChanged, Value = paramEnabled },
                    } ) );
                }
            }

            await base.SetParametersAsync( parameters );
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            await base.OnAfterRenderAsync( firstRender );

            if ( firstRender )
            {
                JSModule ??= new JSCropperModule( JSRuntime, VersionProvider );
                adapter ??= DotNetObjectReference.Create( new ImageCropperAdapter( this ) );

                await JSModule.Initialize( adapter, ElementRef, ElementId, new
                {
                    Source,
                    AspectRatio = AspectRatio.Value,
                    ViewMode = (int)ViewMode,
                    Preview = PreviewSelector,
                    Enabled
                } );
            }
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-image-cropper-source" );
            base.BuildClasses( builder );
        }

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

        /// <summary>
        /// Get the cropped image as Base64 image.
        /// </summary>
        /// <param name="options">the cropping options</param>
        /// <returns>the cropped image</returns>
        public ValueTask<string> CropAsBase64ImageAsync( ImageCropperCropOptions options )
            => JSModule.CropBase64( ElementRef, ElementId, options );

        /// <summary>Move the canvas (image wrapper) with relative offsets.</summary>
        /// <param name="offsetX">Moving size (px) in the horizontal direction.</param>
        /// <param name="offsetY">Moving size (px) in the vertical direction.</param>
        public async ValueTask Move( int offsetX, int offsetY )
            => await JSModule.Move( ElementRef, ElementId, offsetX, offsetY );

        /// <summary>
        /// Move the canvas (image wrapper) to an absolute point.
        /// </summary>
        /// <param name="x">The left value of the canvas</param>
        /// <param name="y">The top value of the canvas</param>
        public async ValueTask MoveTo( int x, int y )
            => await JSModule.MoveTo( ElementRef, ElementId, x, y );

        /// <summary>
        /// Zoom the canvas (image wrapper) with a relative ratio.
        /// </summary>
        /// <param name="ratio">Zoom in: requires a positive number (ratio &gt; 0), Zoom out: requires a negative number (ratio &lt; 0)</param>
        public async ValueTask Zoom( double ratio )
            => await JSModule.Zoom( ElementRef, ElementId, ratio );

        /// <summary>
        /// Zoom the canvas (image wrapper) to an absolute ratio.
        /// </summary>
        /// <param name="ratio">Requires a positive number (ratio &gt; 0)</param>
        /// <param name="x">The coordinate of the center point for zooming, base on the top left corner of the cropper container.</param>
        /// <param name="y">The coordinate of the center point for zooming, base on the top left corner of the cropper container.</param>
        public async ValueTask ZoomTo( double ratio, int x, int y )
            => await JSModule.ZoomTo( ElementRef, ElementId, ratio, x, y );

        /// <summary>
        /// Zoom the canvas (image wrapper) to an absolute ratio.
        /// </summary>
        /// <param name="ratio">Requires a positive number (ratio &gt; 0)</param>
        public async ValueTask ZoomTo( double ratio )
            => await JSModule.ZoomTo( ElementRef, ElementId, ratio );

        /// <summary>
        /// Rotate the image to a relative degree.
        /// </summary>
        /// <param name="degree">Rotate right: requires a positive number (degree &gt; 0), Rotate left: requires a negative number (degree &lt; 0)</param>
        public async ValueTask Rotate( int degree )
            => await JSModule.Rotate( ElementRef, ElementId, degree );

        /// <summary>
        /// Rotate the image to an absolute degree.
        /// </summary>
        /// <param name="degree">the absolute degree</param>
        public async ValueTask RotateTo( int degree )
            => await JSModule.RotateTo( ElementRef, ElementId, degree );

        /// <summary>
        /// Scale the image.
        /// </summary>
        /// <param name="scaleX">The scaling factor applies to the abscissa of the image.</param>
        /// <param name="scaleY">The scaling factor to apply on the ordinate of the image.</param>
        /// <returns></returns>
        public async ValueTask Scale( int scaleX, int scaleY )
            => await JSModule.Scale( ElementRef, ElementId, scaleX, scaleY );


        internal async ValueTask NotifyCropStart() => await CropStarted.InvokeAsync();

        internal async ValueTask NotifyCropMove() => await CropMoved.InvokeAsync();

        internal async ValueTask NotifyCropEnd() => await CropEnded.InvokeAsync();

        internal async ValueTask NotifyCrop() => await Cropped.InvokeAsync();

        internal async ValueTask NotifyZoom() => await Zoomed.InvokeAsync();

        #endregion

        #region Properties

        private JSCropperModule JSModule { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private IVersionProvider VersionProvider { get; set; }

        /// <summary>
        /// Defines the aspect ratio of the image cropper.
        /// </summary>
        [Parameter] public ImageCropperAspectRatio AspectRatio { get; set; } = ImageCropperAspectRatio.Is1x1;

        /// <summary>
        /// Defines the view mode of the cropper.
        /// </summary>
        [Parameter] public ImageCropperViewMode ViewMode { get; set; } = ImageCropperViewMode.Default;

        /// <summary>
        /// The original image source.
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// The alt text of the image.
        /// </summary>
        [Parameter] public string Alt { get; set; }

        /// <summary>
        /// The CSS selector the preview image.
        /// </summary>
        [Parameter] public string PreviewSelector { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box starts to change.
        /// </summary>
        [Parameter] public EventCallback CropStarted { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box is changing.
        /// </summary>
        [Parameter] public EventCallback CropMoved { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box stops changing.
        /// </summary>
        [Parameter] public EventCallback CropEnded { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box changes.
        /// </summary>
        [Parameter] public EventCallback Cropped { get; set; }

        /// <summary>
        /// This event fires when a cropper instance starts to zoom in or zoom out its canvas (image wrapper).
        /// </summary>
        [Parameter] public EventCallback Zoomed { get; set; }

        /// <summary>
        /// Is the cropper enabled.
        /// </summary>
        [Parameter] public bool Enabled { get; set; } = true;

        #endregion
    }
}