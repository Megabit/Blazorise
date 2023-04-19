#region Using directives
using System.Reflection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Linq;
using Blazorise.Extensions;

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
            var dotSizeChanged = parameters.TryGetValue<double>( nameof( DotSize ), out var paramDotSize ) && !DotSize.IsEqual( paramDotSize );
            var minWidthChanged = parameters.TryGetValue<double>( nameof( MinWidth ), out var paramMinWidth ) && MinWidth != paramMinWidth;
            var maxWidthChanged = parameters.TryGetValue<double>( nameof( MaxWidth ), out var paramMaxWidth ) && MaxWidth != paramMaxWidth;
            var throttleChanged = parameters.TryGetValue<int>( nameof( Throttle ), out var paramThrottle ) && Throttle != paramThrottle;
            var minDistanceChanged = parameters.TryGetValue<int>( nameof( MinDistance ), out var paramMinDistance ) && MinDistance != paramMinDistance;
            var backgroundColorChanged = parameters.TryGetValue<string>( nameof( BackgroundColor ), out var paramBgColor ) && !BackgroundColor.IsEqual( paramBgColor );
            var penColorChanged = parameters.TryGetValue<string>( nameof( PenColor ), out var paramPenColor ) && !PenColor.IsEqual( paramPenColor );
            var velocityFilterWeightChanged = parameters.TryGetValue<double>( nameof( VelocityFilterWeight ), out var paramVelocityFilterWeight ) && VelocityFilterWeight != paramVelocityFilterWeight;

            if ( dotSizeChanged || minWidthChanged || maxWidthChanged || throttleChanged || minDistanceChanged || backgroundColorChanged || penColorChanged || velocityFilterWeightChanged )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
                {
                    DotSize = new { Changed = dotSizeChanged, Value = paramDotSize },
                    MinWidth = new { Changed = minWidthChanged, Value = paramMinWidth },
                    MaxWidth = new { Changed = maxWidthChanged, Value = paramMaxWidth },
                    Throttle = new { Changed = throttleChanged, Value = paramThrottle },
                    MinDistance = new { Changed = minDistanceChanged, Value = paramMinDistance },
                    BackgroundColor = new { Changed = backgroundColorChanged, Value = paramBgColor },
                    PenColor = new { Changed = penColorChanged, Value = paramPenColor },
                    VelocityFilterWeight = new { Changed = velocityFilterWeightChanged, Value = paramVelocityFilterWeight }
                } ) );
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

            JSModule = new JSSignaturePadModule( JSRuntime, VersionProvider );

            await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, new
            {
                Value,
                DotSize,
                MinWidth,
                MaxWidth,
                Throttle,
                MinDistance,
                BackgroundColor,
                PenColor,
                VelocityFilterWeight
            } );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.Destroy( ElementRef, ElementId );

            await JSModule.DisposeAsync();

            if ( DotNetObjectRef != null )
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
        if ( JSModule != null )
        {
            await JSModule.Clear( ElementRef, ElementId );
        }
    }

    /// <summary>
    /// This method is called by JavaScript when a stroke has ended in the signature pad. It takes the encoded image
    /// from the signature pad, converts it to bytes and sets the Value property of the component to the image data.
    /// It then invokes the ValueChanged and EndStroke events asynchronously to notify any subscribers of the change.
    /// </summary>
    [JSInvokable]
    public async Task NotifyEndStroke( string value )
    {
        var encodedImage = value.Split( ',' )[1];
        Value = Convert.FromBase64String( encodedImage );

        await ValueChanged.InvokeAsync( Value );
        await EndStroke.InvokeAsync( Value );
    }

    /// <summary>
    /// This method is called by JavaScript when a new stroke has begun in the signature pad. It invokes the BeginStroke
    /// event asynchronously to notify any subscribers of the event.
    /// </summary>
    [JSInvokable]
    public async Task NotifyBeginStroke()
    {
        await BeginStroke.InvokeAsync();
    }

    #endregion

    #region Properties

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    ///<summary>
    /// Gets or sets value for the signature pad.
    /// </summary>
    [Parameter] public byte[] Value { get; set; }

    /// <summary>
    /// Gets or sets the event that is triggered when the signature pad value changes. The event provides the new signature as a byte array.
    /// </summary>
    [Parameter] public EventCallback<byte[]> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the event that is triggered when a stroke ends on the signature pad. The event provides the signature pad's current image data as a PNG-encoded Data URL.
    /// </summary>
    [Parameter] public EventCallback EndStroke { get; set; }

    /// <summary>
    /// Gets or sets the event that is triggered when a new stroke begins on the signature pad. The event provides information about the starting point of the stroke.
    /// </summary>
    [Parameter] public EventCallback BeginStroke { get; set; }

    /// <summary>
    /// The radius of a single dot. Also the width of the start of a mark.
    /// </summary>
    /// <value>The dot size.</value>
    [Parameter] public double DotSize { get; set; }

    /// <summary>
    /// The minimum width of a line.
    /// </summary>
    /// <value>The minimum width.</value>
    [Parameter] public double MinWidth { get; set; }

    /// <summary>
    /// The maximum width of a line.
    /// </summary>
    /// <value>The maximum width.</value>
    [Parameter] public double MaxWidth { get; set; }

    /// <summary>
    /// The time in milliseconds to throttle drawing. Set to 0 to turn off throttling.
    /// </summary>
    /// <value>The throttle time in milliseconds.</value>
    [Parameter] public int Throttle { get; set; }

    /// <summary>
    /// The minimum distance between two points to add a new point to the signature.
    /// </summary>
    /// <value>The minimum distance.</value>
    [Parameter] public int MinDistance { get; set; }

    /// <summary>
    /// The color used to clear the background. Can be any color format accepted by context.fillStyle.
    /// </summary>
    /// <value>The background color.</value>
    [Parameter] public string BackgroundColor { get; set; }

    /// <summary>
    /// The color used to draw the lines. Can be any color format accepted by context.fillStyle.
    /// </summary>
    /// <value>The pen color.</value>
    [Parameter] public string PenColor { get; set; }

    /// <summary>
    /// The weight used to modify new velocity based on the previous velocity.
    /// </summary>
    /// <value>The velocity filter weight.</value>
    [Parameter] public double VelocityFilterWeight { get; set; }

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    protected DotNetObjectReference<SignaturePad> DotNetObjectRef { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="JSSignaturePadModule"/> instance.
    /// </summary>
    protected JSSignaturePadModule JSModule { get; private set; }

    #endregion
}