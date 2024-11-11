#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules.JSOptions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.SignaturePad;

/// <summary>
/// The SignaturePad component allows for the creation of a digital signature pad, which allows users to draw signatures using a mouse, touchpad, or stylus. This component can be used in various applications such as electronic forms, contracts, and agreements.
/// </summary>
public partial class SignaturePad : BaseComponent, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var valueChanged = parameters.TryGetValue<byte[]>( nameof( Value ), out var paramValue ) && !Value.AreEqual( paramValue );
            var dotSizeChanged = parameters.TryGetValue<double>( nameof( DotSize ), out var paramDotSize ) && !DotSize.IsEqual( paramDotSize );
            var minLineWidthChanged = parameters.TryGetValue<double>( nameof( MinLineWidth ), out var paramMinWidth ) && MinLineWidth != paramMinWidth;
            var maxLineWidthChanged = parameters.TryGetValue<double>( nameof( MaxLineWidth ), out var paramMaxWidth ) && MaxLineWidth != paramMaxWidth;
            var throttleChanged = parameters.TryGetValue<int>( nameof( Throttle ), out var paramThrottle ) && Throttle != paramThrottle;
            var minDistanceChanged = parameters.TryGetValue<int>( nameof( MinDistance ), out var paramMinDistance ) && MinDistance != paramMinDistance;
            var backgroundColorChanged = parameters.TryGetValue<string>( nameof( BackgroundColor ), out var paramBgColor ) && !BackgroundColor.IsEqual( paramBgColor );
            var penColorChanged = parameters.TryGetValue<string>( nameof( PenColor ), out var paramPenColor ) && !PenColor.IsEqual( paramPenColor );
            var velocityFilterWeightChanged = parameters.TryGetValue<double>( nameof( VelocityFilterWeight ), out var paramVelocityFilterWeight ) && VelocityFilterWeight != paramVelocityFilterWeight;
            var imageTypeChanged = parameters.TryGetValue<SignaturePadImageType>( nameof( ImageType ), out var paramImageType ) && ImageType != paramImageType;
            var imageQualityChanged = parameters.TryGetValue<double?>( nameof( ImageQuality ), out var paramImageQuality ) && ImageQuality != paramImageQuality;
            var includeImageBackgroundColorChanged = parameters.TryGetValue<bool>( nameof( IncludeImageBackgroundColor ), out var paramIncludeImageBackgroundColor ) && IncludeImageBackgroundColor != paramIncludeImageBackgroundColor;
            var readOnlyChanged = parameters.TryGetValue<bool>( nameof( ReadOnly ), out var paramReadOnly ) && ReadOnly != paramReadOnly;

            if ( valueChanged
                || dotSizeChanged
                || minLineWidthChanged
                || maxLineWidthChanged
                || throttleChanged
                || minDistanceChanged
                || backgroundColorChanged
                || penColorChanged
                || velocityFilterWeightChanged
                || imageTypeChanged
                || imageQualityChanged
                || includeImageBackgroundColorChanged
                || readOnlyChanged )
            {
                ExecuteAfterRender(async () => await JSModule.UpdateOptions(ElementRef, ElementId, new SignaturePadUpdateJSOptions
                {
                    DataUrl = new JSOptionChange<string>(valueChanged, GetDataUrl(paramValue, paramImageType)),
                    DotSize = new JSOptionChange<double>(dotSizeChanged, paramDotSize),
                    MinLineWidth = new JSOptionChange<double>(minLineWidthChanged, paramMinWidth),
                    MaxLineWidth = new JSOptionChange<double>(maxLineWidthChanged, paramMaxWidth),
                    Throttle = new JSOptionChange<int>(throttleChanged, paramThrottle),
                    MinDistance = new JSOptionChange<int>(minDistanceChanged, paramMinDistance),
                    BackgroundColor = new JSOptionChange<string>(backgroundColorChanged, paramBgColor),
                    PenColor = new JSOptionChange<string>(penColorChanged, paramPenColor),
                    VelocityFilterWeight = new JSOptionChange<double>(velocityFilterWeightChanged, paramVelocityFilterWeight),
                    ImageType = new JSOptionChange<string>(imageTypeChanged, GetImageTypeString(paramImageType)),
                    ImageQuality = new JSOptionChange<double?>(imageQualityChanged, paramImageQuality),
                    IncludeImageBackgroundColor = new JSOptionChange<bool>(includeImageBackgroundColorChanged, paramIncludeImageBackgroundColor),
                    ReadOnly = new JSOptionChange<bool>(readOnlyChanged, paramReadOnly)
                }));

            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            DotNetObjectRef ??= DotNetObjectReference.Create( this );

            JSModule = new JSSignaturePadModule( JSRuntime, VersionProvider, BlazoriseOptions );

            await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, new SignaturePadInitializeJSOptions
            {
                DataUrl = GetDataUrl(Value, ImageType),
                DotSize = DotSize,
                MinLineWidth = MinLineWidth,
                MaxLineWidth = MaxLineWidth,
                Throttle = Throttle,
                MinDistance = MinDistance,
                BackgroundColor = BackgroundColor,
                PenColor = PenColor,
                VelocityFilterWeight = VelocityFilterWeight,
                ImageType = GetImageTypeString(ImageType),
                ImageQuality = ImageQuality,
                IncludeImageBackgroundColor = IncludeImageBackgroundColor,
                ReadOnly = ReadOnly
            });
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            if ( DotNetObjectRef is not null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Clears the content of a signature canvas.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Clear()
    {
        if ( JSModule is not null )
        {
            await JSModule.Clear( ElementRef, ElementId );
        }
    }

    /// <summary>
    /// Undos the last stroke if there is any.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Undo()
    {
        if ( JSModule is not null )
        {
            var dataUrl = await JSModule.Undo( ElementRef, ElementId );
            var data = ParseDataFromDataUrl( dataUrl );

            await ValueChanged.InvokeAsync( data );
        }
    }

    /// <summary>
    /// This method is called by JavaScript when a new stroke has begun in the signature pad. It invokes the BeginStroke
    /// event asynchronously to notify any subscribers of the event.
    /// </summary>
    [JSInvokable]
    public async Task NotifyBeginStroke( double offsetX, double offsetY )
    {
        await BeginStroke.InvokeAsync( new SignaturePadBeginStrokeEventArgs( offsetX, offsetY ) );
    }

    /// <summary>
    /// This method is called by JavaScript when a stroke has ended in the signature pad. It takes the encoded image
    /// from the signature pad, converts it to bytes and sets the Value property of the component to the image data.
    /// It then invokes the ValueChanged and EndStroke events asynchronously to notify any subscribers of the change.
    /// </summary>
    [JSInvokable]
    public async Task NotifyEndStroke( string dataUrl, double offsetX, double offsetY )
    {
        var data = ParseDataFromDataUrl( dataUrl );

        await ValueChanged.InvokeAsync( data );
        await EndStroke.InvokeAsync( new SignaturePadEndStrokeEventArgs( data, dataUrl, offsetX, offsetY ) );
    }

    /// <summary>
    /// Gets the string representation of the <see cref="ImageType"/>.
    /// </summary>
    /// <param name="imageType">Image type value.</param>
    /// <returns>String representation of the <see cref="ImageType"/>.</returns>
    private static string GetImageTypeString( SignaturePadImageType imageType ) => imageType switch
    {
        SignaturePadImageType.Jpeg => "jpeg",
        SignaturePadImageType.Svg => "svg",
        SignaturePadImageType.Png => "png",
        _ => "png",
    };

    /// <summary>
    /// Gets the mime type of the <see cref="ImageType"/>.
    /// </summary>
    /// <param name="imageType">Image type value.</param>
    /// <returns>Mime type of the <see cref="ImageType"/>.</returns>
    private static string GetImageMimeType( SignaturePadImageType imageType ) => imageType switch
    {
        SignaturePadImageType.Jpeg => "image/jpeg",
        SignaturePadImageType.Svg => "image/svg+xml",
        SignaturePadImageType.Png => "image/png",
        _ => "image/png",
    };

    /// <summary>
    /// Gets the data url based on the image type and the data array.
    /// </summary>
    /// <param name="data">Byte array that holds the image data.</param>
    /// <param name="imageType">Image type.</param>
    /// <returns>Data url.</returns>
    public static string GetDataUrl( byte[] data, SignaturePadImageType imageType = SignaturePadImageType.Png )
    {
        if ( data is null )
            return null;

        var mimeType = GetImageMimeType( imageType );
        var base64 = Convert.ToBase64String( data );

        return $"data:{mimeType};base64,{base64}";
    }

    /// <summary>
    /// Parses image data from the encoded data URL.
    /// </summary>
    /// <param name="dataUrl">Data URL containing the encoded data.</param>
    /// <returns>Byte array containing the image data.</returns>
    private static byte[] ParseDataFromDataUrl( string dataUrl )
    {
        if ( string.IsNullOrEmpty( dataUrl ) )
            return null;

        var valueParts = dataUrl.Split( ";base64," );

        if ( valueParts.Length > 1 )
        {
            var base64 = valueParts[1].Trim();
            return Convert.FromBase64String( base64 );
        }

        return null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    protected DotNetObjectReference<SignaturePad> DotNetObjectRef { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="JSSignaturePadModule"/> instance.
    /// </summary>
    protected JSSignaturePadModule JSModule { get; private set; }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    ///<summary>
    /// Gets or sets value for the signature pad.
    /// </summary>
    [Parameter] public byte[] Value { get; set; }

    /// <summary>
    /// Gets or sets the event that is triggered when the signature pad value changes. The event provides the new signature as a byte array.
    /// </summary>
    [Parameter] public EventCallback<byte[]> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the event that is triggered when a new stroke begins on the signature pad. The event provides information about the starting point of the stroke.
    /// </summary>
    [Parameter] public EventCallback<SignaturePadBeginStrokeEventArgs> BeginStroke { get; set; }

    /// <summary>
    /// Gets or sets the event that is triggered when a stroke ends on the signature pad. The event provides the signature pad's current image data as a Data URL.
    /// </summary>
    [Parameter] public EventCallback<SignaturePadEndStrokeEventArgs> EndStroke { get; set; }

    /// <summary>
    /// The radius of a single dot. Also the width of the start of a mark.
    /// </summary>
    [Parameter] public double DotSize { get; set; }

    /// <summary>
    /// The minimum width of a line.
    /// </summary>
    /// <value>The minimum width.</value>
    [Parameter] public double MinLineWidth { get; set; }

    /// <summary>
    /// The maximum width of a line.
    /// </summary>
    /// <value>The maximum width.</value>
    [Parameter] public double MaxLineWidth { get; set; }

    /// <summary>
    /// The time in milliseconds to throttle drawing. Set to 0 to turn off throttling.
    /// </summary>
    [Parameter] public int Throttle { get; set; }

    /// <summary>
    /// Add the next point only if the previous one is farther than 'n' pixels. Defaults to 5.
    /// </summary>
    [Parameter] public int MinDistance { get; set; }

    /// <summary>
    /// The color used to define the background color of the signature pad. Can be any color format; including HEX, or rgb.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; }

    /// <summary>
    /// The color used to define the lines color of the signature pad. Can be any color format; including HEX, or rgb.
    /// </summary>
    [Parameter] public string PenColor { get; set; }

    /// <summary>
    /// The weight used to modify new velocity based on the previous velocity.
    /// </summary>
    [Parameter] public double VelocityFilterWeight { get; set; }

    /// <summary>
    /// The image type [png, jpeg, svg] to get from the canvas element.
    /// </summary>
    [Parameter] public SignaturePadImageType ImageType { get; set; } = SignaturePadImageType.Png;

    /// <summary>
    /// The encoder options for image type [png, jpeg] to get from the canvas element. Accepted range is from 0 to 1, where 1 means best quality.
    /// </summary>
    [Parameter] public double? ImageQuality { get; set; }

    /// <summary>
    /// If true, the [svg] image returned from the canvas will include background color defined by the BackgroundColor parameter.
    /// </summary>
    [Parameter] public bool IncludeImageBackgroundColor { get; set; }

    /// <summary>
    /// If true, prevents the user interaction.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Defines the width, in pixels, of the underline canvas element.
    /// </summary>
    [Parameter] public int? CanvasWidth { get; set; }

    /// <summary>
    /// Defines the height, in pixels, of the underline canvas element.
    /// </summary>
    [Parameter] public int? CanvasHeight { get; set; }

    #endregion
}