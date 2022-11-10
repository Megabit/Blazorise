#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.ImageCropper
{
    /// <summary>
    /// Blazorise Image Cropper component based on <see href="https://fengyuanchen.github.io/cropperjs/">CropperJS</see>.
    /// </summary>
    public partial class ImageCropper : BaseComponent, IAsyncDisposable
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
                var altChanged = parameters.TryGetValue<string>( nameof( Alt ), out var paramAlt ) && paramAlt != Alt;
                var aspectRatioChanged = parameters.TryGetValue<ImageCropperAspectRatio>( nameof( AspectRatio ), out var paramAspectRatio ) && paramAspectRatio != AspectRatio;
                var previewSelectorChanged = parameters.TryGetValue<string>( nameof( PreviewSelector ), out var paramPreviewSelector ) && paramPreviewSelector != PreviewSelector;
                var enabledChanged = parameters.TryGetValue<bool>( nameof( Enabled ), out var paramEnabled ) && paramEnabled != Enabled;

                if ( sourceChanged
                    || altChanged
                    || aspectRatioChanged
                    || previewSelectorChanged
                    || enabledChanged )
                {
                    ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
                    {
                        Source = new { Changed = sourceChanged, Value = paramSource },
                        Alt = new { Changed = altChanged, Value = paramAlt },
                        AspectRatio = new { Changed = aspectRatioChanged, Value = paramAspectRatio.Value },
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
                    Alt,
                    AspectRatio = AspectRatio.Value,
                    Preview = PreviewSelector,
                    Enabled,
                    ShowBackground,
                    Movable,
                    Resizable,
                    Zoomable,
                    Keyboard,
                    Outlined
                } );
            }
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            //builder.Append( "b-image-cropper-source" );
            builder.Append( "b-cropper-container" );

            base.BuildClasses( builder );
        }

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

        /// <summary>
        /// Moves the image.
        /// </summary>
        /// <param name="x">The moving distance in the horizontal direction.</param>
        /// <param name="y">The moving distance in the vertical direction.</param>
        public ValueTask Move( int x, int y )
            => JSModule.Move( ElementRef, ElementId, x, y );

        /// <summary>
        /// Moves the image to a specific position.
        /// </summary>
        /// <param name="x">The new position in the horizontal direction.</param>
        /// <param name="y">The new position in the vertical direction.</param>
        public ValueTask MoveTo( int x, int y )
            => JSModule.MoveTo( ElementRef, ElementId, x, y );

        /// <summary>
        /// Zooms the image.
        /// </summary>
        /// <param name="scale">The zoom factor. Positive numbers for zooming in, and negative numbers for zooming out.</param>
        public ValueTask Zoom( double scale )
            => JSModule.Zoom( ElementRef, ElementId, scale );

        /// <summary>
        /// Rotates the image.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        public ValueTask Rotate( double angle )
            => JSModule.Rotate( ElementRef, ElementId, angle );

        /// <summary>
        /// Scale the image.
        /// </summary>
        /// <param name="x">The scaling factor in the horizontal direction.</param>
        /// <param name="y">The scaling factor in the vertical direction.</param>
        /// <returns></returns>
        public ValueTask Scale( int x, int y )
            => JSModule.Scale( ElementRef, ElementId, x, y );


        internal async Task NotifyCropStart()
        {
            if ( CropStarted is not null )
                await CropStarted.Invoke();
        }

        internal async Task NotifyCropMove()
        {
            if ( CropMoved is not null )
                await CropMoved.Invoke();
        }

        internal async Task NotifyCropEnd()
        {
            if ( CropEnded is not null )
                await CropEnded.Invoke();
        }

        internal async Task NotifyCrop()
        {
            if ( Cropped is not null )
                await Cropped.Invoke();
        }

        internal async Task NotifyZoom()
        {
            if ( Zoomed is not null )
                await Zoomed.Invoke();
        }

        #endregion

        #region Properties

        private JSCropperModule JSModule { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private IVersionProvider VersionProvider { get; set; }

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
        [Parameter] public Func<Task> CropStarted { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box is changing.
        /// </summary>
        [Parameter] public Func<Task> CropMoved { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box stops changing.
        /// </summary>
        [Parameter] public Func<Task> CropEnded { get; set; }

        /// <summary>
        /// This event fires when the canvas (image wrapper) or the crop box changes.
        /// </summary>
        [Parameter] public Func<Task> Cropped { get; set; }

        /// <summary>
        /// This event fires when a cropper instance starts to zoom in or zoom out its canvas (image wrapper).
        /// </summary>
        [Parameter] public Func<Task> Zoomed { get; set; }

        /// <summary>
        /// Indicates whether this element is disabled.
        /// </summary>
        [Parameter] public bool Enabled { get; set; } = true;

        /// <summary>
        /// Indicates whether this element has a grid background.
        /// </summary>
        [Parameter] public bool ShowBackground { get; set; } = true;

        /// <summary>
        /// Indicates the aspect ratio of the selection, must a positive number.
        /// </summary>
        [Parameter] public ImageCropperAspectRatio AspectRatio { get; set; } = ImageCropperAspectRatio.Is1x1;

        /// <summary>
        /// Indicates the initial aspect ratio of the selection, must a positive number.
        /// </summary>
        [Parameter] public ImageCropperAspectRatio InitialAspectRatio { get; set; } = ImageCropperAspectRatio.Is1x1;

        /// <summary>
        /// Indicates whether the selection is movable.
        /// </summary>
        [Parameter] public bool Movable { get; set; } = true;

        /// <summary>
        /// Indicates whether the selection is resizable.
        /// </summary>
        [Parameter] public bool Resizable { get; set; } = true;

        /// <summary>
        /// Indicates whether the selection is zoomable.
        /// </summary>
        [Parameter] public bool Zoomable { get; set; } = true;

        /// <summary>
        /// Indicates whether keyboard control is supported.
        /// </summary>
        [Parameter] public bool Keyboard { get; set; } = true;

        /// <summary>
        /// Indicates whether show the outlined or not.
        /// </summary>
        [Parameter] public bool Outlined { get; set; } = true;

        #endregion
    }
}