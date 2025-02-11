#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Cropper;

/// <summary>
/// Blazorise image cropper component based on <see href="https://fengyuanchen.github.io/cropperjs/">CropperJS</see>.
/// </summary>
public partial class Cropper : BaseComponent, IAsyncDisposable
{
    #region Members

    private DotNetObjectReference<CropperAdapter> adapter;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var sourceChanged = parameters.TryGetValue<string>( nameof( Source ), out var paramSource ) && paramSource != Source;
            var altChanged = parameters.TryGetValue<string>( nameof( Alt ), out var paramAlt ) && paramAlt != Alt;
            var crossoriginChanged = parameters.TryGetValue<string>( nameof( CrossOrigin ), out var paramCrossOrigin ) && paramCrossOrigin != CrossOrigin;
            var imageOptionsChanged = parameters.TryGetValue<CropperImageOptions>( nameof( ImageOptions ), out var paramImageOptions ) && paramImageOptions != ImageOptions;
            var selectionOptionsChanged = parameters.TryGetValue<CropperSelectionOptions>( nameof( SelectionOptions ), out var paramSelectionOptions ) && paramSelectionOptions != SelectionOptions;
            var gridOptionsChanged = parameters.TryGetValue<CropperGridOptions>( nameof( GridOptions ), out var paramGridOptions ) && paramGridOptions != GridOptions;
            var enabledChanged = parameters.TryGetValue<bool>( nameof( Enabled ), out var paramEnabled ) && paramEnabled != Enabled;
            if ( sourceChanged
                || altChanged
                || crossoriginChanged
                || imageOptionsChanged
                || selectionOptionsChanged
                || gridOptionsChanged
                || enabledChanged )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new()
                {
                    Source = new( sourceChanged, paramSource ),
                    Alt = new( altChanged, paramAlt ),
                    CrossOrigin = new( crossoriginChanged, paramCrossOrigin ),
                    Image = new( imageOptionsChanged, new CropperImageOptions
                    {
                        Rotatable = paramImageOptions?.Rotatable ?? true,
                        Scalable = paramImageOptions?.Scalable ?? true,
                        Skewable = paramImageOptions?.Skewable ?? true,
                        Translatable = paramImageOptions?.Translatable ?? true,
                    } ),
                    Selection = new( selectionOptionsChanged, new CropperSelectionJSOptions
                    {
                        AspectRatio = paramSelectionOptions?.AspectRatio.Value,
                        InitialAspectRatio = paramSelectionOptions?.InitialAspectRatio.Value,
                        InitialCoverage = paramSelectionOptions?.InitialCoverage.Value,
                        Movable = paramSelectionOptions?.Movable ?? false,
                        Resizable = paramSelectionOptions?.Resizable ?? false,
                        Zoomable = paramSelectionOptions?.Zoomable ?? false,
                        Keyboard = paramSelectionOptions?.Keyboard ?? false,
                        Outlined = paramSelectionOptions?.Outlined ?? false
                    } ),
                    Grid = new( gridOptionsChanged, new CropperGridOptions
                    {
                        Rows = paramGridOptions?.Rows ?? 3,
                        Columns = paramGridOptions?.Columns ?? 3,
                        Bordered = paramGridOptions?.Bordered ?? false,
                        Covered = paramGridOptions?.Covered ?? false,
                    } ),
                    Enabled = new( enabledChanged, paramEnabled )
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
            JSModule ??= new JSCropperModule( JSRuntime, VersionProvider, BlazoriseOptions );
            adapter ??= DotNetObjectReference.Create( new CropperAdapter( this ) );

            await JSModule.Initialize( adapter, ElementRef, ElementId, new()
            {
                Source = Source,
                Alt = Alt,
                Enabled = Enabled,
                ShowBackground = ShowBackground,
                Image = new()
                {
                    Rotatable = ImageOptions?.Rotatable ?? true,
                    Scalable = ImageOptions?.Scalable ?? true,
                    Skewable = ImageOptions?.Skewable ?? true,
                    Translatable = ImageOptions?.Translatable ?? true,
                },
                Selection = new()
                {
                    AspectRatio = SelectionOptions?.AspectRatio.Value,
                    InitialAspectRatio = SelectionOptions?.InitialAspectRatio.Value,
                    InitialCoverage = SelectionOptions?.InitialCoverage,
                    Movable = SelectionOptions?.Movable ?? false,
                    Resizable = SelectionOptions?.Resizable ?? false,
                    Zoomable = SelectionOptions?.Zoomable ?? false,
                    Keyboard = SelectionOptions?.Keyboard ?? false,
                    Outlined = SelectionOptions?.Outlined ?? false
                },
                Grid = new()
                {
                    Rows = GridOptions?.Rows ?? 3,
                    Columns = GridOptions?.Columns ?? 3,
                    Bordered = GridOptions?.Bordered ?? false,
                    Covered = GridOptions?.Covered ?? false,
                }
            } );

            if ( CropperState is not null )
            {
                await CropperState.CropperInitialized.InvokeCallbackAsync( this );
            }
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
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
    /// <param name="options">the cropping options.</param>
    /// <returns>the cropped image.</returns>
    public ValueTask<string> CropAsBase64ImageAsync( CropperCropOptions options )
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

    /// <summary>
    /// Center the image.
    /// </summary>
    /// <param name="size">The size factor: null, contain or cover.</param>
    /// <returns></returns>
    public ValueTask Center( string size )
        => JSModule.Center( ElementRef, ElementId, size );

    /// <summary>
    /// Resets the selection to its initial position and size.
    /// </summary>
    /// <returns></returns>
    public ValueTask ResetSelection()
        => JSModule.ResetSelection( ElementRef, ElementId );

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

    internal async Task NotifyCrop( double startX, double startY, double endX, double endY )
    {
        if ( Cropped is not null )
            await Cropped.Invoke( new CropperCroppedEventArgs( startX, startY, endX, endY ) );
    }

    internal async Task NotifyZoom( double scale )
    {
        if ( Zoomed is not null )
            await Zoomed.Invoke( new CropperZoomedEventArgs( scale ) );
    }

    internal async Task NotifySelectionChanged( double x, double y, double width, double height )
    {
        if ( SelectionChanged is not null )
            await SelectionChanged.Invoke( new CropperSelectionChangedEventArgs( x, y, width, height ) );
    }

    internal async Task NotifyImageReady()
    {
        if ( ImageReady is not null )
            await ImageReady.Invoke();
    }

    internal async Task NotifyImageLoadingFailed( string errorMessage )
    {
        if ( ImageLoadingFailed is not null )
            await ImageLoadingFailed.Invoke( errorMessage );
    }

    #endregion

    #region Properties

    internal JSCropperModule JSModule { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }
    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// The source of the image.
    /// </summary>
    [Parameter, EditorRequired] public string Source { get; set; }

    /// <summary>
    /// The alt text of the image.
    /// </summary>
    [Parameter] public string Alt { get; set; }

    /// <summary>
    /// The cross-origin attribute of the image.
    /// </summary>
    [Parameter] public string CrossOrigin { get; set; }

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
    [Parameter] public Func<CropperCroppedEventArgs, Task> Cropped { get; set; }

    /// <summary>
    /// This event fires when a cropper instance starts to zoom in or zoom out its canvas (image wrapper).
    /// </summary>
    [Parameter] public Func<CropperZoomedEventArgs, Task> Zoomed { get; set; }

    /// <summary>
    /// The event is fired when the position or size of the selection is going to change.
    /// </summary>
    [Parameter] public Func<CropperSelectionChangedEventArgs, Task> SelectionChanged { get; set; }

    /// <summary>
    /// This event fires when the image is ready / loaded.
    /// </summary>
    [Parameter] public Func<Task> ImageReady { get; set; }

    /// <summary>
    /// This event fires when the image cannot be loaded. Usually because of 404 or <see cref="Source"/> being null. Returns an error message as a parameter.
    /// </summary>
    [Parameter] public Func<string, Task> ImageLoadingFailed { get; set; }

    /// <summary>
    /// Indicates whether this element is disabled.
    /// </summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// Indicates whether this element has a grid background.
    /// </summary>
    [Parameter] public bool ShowBackground { get; set; } = true;

    /// <summary>
    /// Provides properties for manipulating the layout and presentation of image elements.
    /// </summary>
    [Parameter] public CropperImageOptions ImageOptions { get; set; } = new CropperImageOptions();

    /// <summary>
    /// Provides properties for manipulating the layout and presentation.
    /// </summary>
    [Parameter] public CropperSelectionOptions SelectionOptions { get; set; } = new CropperSelectionOptions();

    /// <summary>
    /// Provides properties for manipulating the layout and presentation of selection grid elements.
    /// </summary>
    [Parameter] public CropperGridOptions GridOptions { get; set; } = new CropperGridOptions();

    /// <summary>
    /// Provides a shared state and syncronization context between the cropper and cropper viewer.
    /// </summary>
    [Parameter] public CropperState CropperState { get; set; }

    #endregion
}