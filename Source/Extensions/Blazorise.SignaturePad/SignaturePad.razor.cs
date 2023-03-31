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

public partial class SignaturePad : BaseComponent, IAsyncDisposable
{
    #region Methods

    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var dotSizeChanged = parameters.TryGetValue<float>( nameof( DotSize ), out var paramDotSize ) && !DotSize.IsEqual( paramDotSize );
            var minWidthChanged = parameters.TryGetValue<float>( nameof( MinWidth ), out var paramMinWidth ) && MinWidth != paramMinWidth;
            var maxWidthChanged = parameters.TryGetValue<float>( nameof( MaxWidth ), out var paramMaxWidth ) && MaxWidth != paramMaxWidth;
            var throttleChanged = parameters.TryGetValue<int>( nameof( Throttle ), out var paramThrottle ) && Throttle != paramThrottle;
            var minDistanceChanged = parameters.TryGetValue<int>( nameof( MinDistance ), out var paramMinDistance ) && MinDistance != paramMinDistance;
            var backgroundColorChanged = parameters.TryGetValue<string>( nameof( BackgroundColor ), out var paramBgColor ) && !BackgroundColor.IsEqual( paramBgColor );
            var penColorChanged = parameters.TryGetValue<string>( nameof( PenColor ), out var paramPenColor ) && !PenColor.IsEqual( paramPenColor );
            var velocityFilterWeightChanged = parameters.TryGetValue<float>( nameof( VelocityFilterWeight ), out var paramVelocityFilterWeight ) && VelocityFilterWeight != paramVelocityFilterWeight;

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
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            DotNetObjectRef ??= DotNetObjectReference.Create( this );

            JSModule = new JSSignaturePadModule( JSRuntime, VersionProvider );

            await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, new
            {
                Options,
                DataUrl,
                Value = Value,// != null ? Convert.ToBase64String( Value ) : null
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

    [JSInvokable]
    public Task NotifyValue( string value )
    {
        var encodedImage = value.Split( ',' )[1];
        Value = Convert.FromBase64String( encodedImage );

        return ValueChanged.InvokeAsync( Value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the data URL for the signature pad image.
    /// </summary>
    [Parameter]
    public string DataUrl { get; set; }

    /// <summary>
    /// Gets or sets the options for the signature pad.
    /// </summary>
    [Parameter]
    public object Options { get; set; }

    ///<summary>
    /// Gets or sets value for the signature pad.
    /// </summary>
    [Parameter]
    public byte[] Value { get; set; }

    /// <summary>
    /// Gets or sets the event that is triggered when the signature pad value changes. The event provides the new signature as a byte array.
    /// </summary>
    [Parameter]
    public EventCallback<byte[]> ValueChanged { get; set; }

    /// <summary>
    /// The radius of a single dot. Also the width of the start of a mark.
    /// </summary>
    /// <value>The dot size.</value>
    [Parameter]
    public float DotSize { get; set; }

    /// <summary>
    /// The minimum width of a line.
    /// </summary>
    /// <value>The minimum width.</value>
    [Parameter]
    public float MinWidth { get; set; }

    /// <summary>
    /// The maximum width of a line.
    /// </summary>
    /// <value>The maximum width.</value>
    [Parameter]
    public float MaxWidth { get; set; }

    /// <summary>
    /// The time in milliseconds to throttle drawing. Set to 0 to turn off throttling.
    /// </summary>
    /// <value>The throttle time in milliseconds.</value>
    [Parameter]
    public int Throttle { get; set; }

    /// <summary>
    /// The minimum distance between two points to add a new point to the signature.
    /// </summary>
    /// <value>The minimum distance.</value>
    [Parameter]
    public int MinDistance { get; set; }

    /// <summary>
    /// The color used to clear the background. Can be any color format accepted by context.fillStyle.
    /// </summary>
    /// <value>The background color.</value>
    [Parameter]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// The color used to draw the lines. Can be any color format accepted by context.fillStyle.
    /// </summary>
    /// <value>The pen color.</value>
    [Parameter]
    public string PenColor { get; set; }

    /// <summary>
    /// The weight used to modify new velocity based on the previous velocity.
    /// </summary>
    /// <value>The velocity filter weight.</value>
    [Parameter]
    public float VelocityFilterWeight { get; set; }

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    protected DotNetObjectReference<SignaturePad> DotNetObjectRef { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="JSVideoModule"/> instance.
    /// </summary>
    protected JSSignaturePadModule JSModule { get; private set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }
    #endregion
}