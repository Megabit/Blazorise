#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Detects pointer-based gestures on the wrapped content.
/// </summary>
public partial class Gestures : BaseComponent, IDisposable, IAsyncDisposable
{
    #region Members

    private DotNetObjectReference<Gestures> dotNetObjectRef;

    private bool jsInitialized;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef = CreateDotNetObjectRef( this );

        await JSGesturesModule.Initialize( dotNetObjectRef, ElementRef, ElementId, CreateOptions() );

        jsInitialized = true;

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if ( Rendered && jsInitialized )
        {
            ExecuteAfterRender( async () => await JSGesturesModule.UpdateOptions( ElementRef, ElementId, CreateOptions() ) );
        }

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( jsInitialized )
            {
                jsInitialized = false;
                _ = JSGesturesModule.Destroy( ElementRef, ElementId );
            }

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( jsInitialized )
            {
                jsInitialized = false;
                await JSGesturesModule.Destroy( ElementRef, ElementId );
            }

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles a gesture start notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture start information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnGestureStarted( GestureEventArgs eventArgs )
        => GestureStarted.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a gesture move notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture move information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnGestureMoved( GestureEventArgs eventArgs )
        => GestureMoved.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a gesture end notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture end information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnGestureEnded( GestureEventArgs eventArgs )
        => GestureEnded.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a swipe notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Swipe information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnSwiped( SwipeEventArgs eventArgs )
        => Swiped.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a tap notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Tap information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnTapped( TapEventArgs eventArgs )
        => Tapped.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a long-press notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Long-press information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnLongPressed( LongPressEventArgs eventArgs )
        => LongPressed.InvokeAsync( eventArgs );

    private GesturesJSOptions CreateOptions()
        => new GesturesJSOptions()
        {
            Enabled = Enabled,
            Direction = Direction,
            SwipeThreshold = SwipeThreshold,
            SwipeVelocityThreshold = SwipeVelocityThreshold,
            TapMaximumDistance = TapMaximumDistance,
            TapMaximumDuration = TapMaximumDuration,
            LongPressDuration = LongPressDuration,
            LongPressMoveTolerance = LongPressMoveTolerance,
            MoveThrottleInterval = MoveThrottleInterval,
            TouchAction = TouchAction,
            NotifyGestureStarted = GestureStarted.HasDelegate,
            NotifyGestureMoved = GestureMoved.HasDelegate,
            NotifyGestureEnded = GestureEnded.HasDelegate,
            NotifySwiped = Swiped.HasDelegate,
            NotifyTapped = Tapped.HasDelegate,
            NotifyLongPressed = LongPressed.HasDelegate,
        };

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Specifies the JS gesture module.
    /// </summary>
    [Inject] public IJSGesturesModule JSGesturesModule { get; set; }

    /// <summary>
    /// Gets or sets the content that will be observed for gestures.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Enables or disables gesture handling.
    /// </summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the directions that can produce swipe events.
    /// </summary>
    [Parameter] public GestureDirection Direction { get; set; } = GestureDirection.All;

    /// <summary>
    /// Gets or sets the minimum swipe distance in pixels.
    /// </summary>
    [Parameter] public double SwipeThreshold { get; set; } = 50;

    /// <summary>
    /// Gets or sets the minimum swipe velocity in pixels per millisecond.
    /// </summary>
    [Parameter] public double SwipeVelocityThreshold { get; set; } = 0.3;

    /// <summary>
    /// Gets or sets the maximum movement in pixels that can still be recognized as a tap.
    /// </summary>
    [Parameter] public double TapMaximumDistance { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum duration in milliseconds that can still be recognized as a tap.
    /// </summary>
    [Parameter] public int TapMaximumDuration { get; set; } = 300;

    /// <summary>
    /// Gets or sets the duration in milliseconds required to recognize a long press.
    /// </summary>
    [Parameter] public int LongPressDuration { get; set; } = 500;

    /// <summary>
    /// Gets or sets the movement tolerance in pixels before a pending long press is canceled.
    /// </summary>
    [Parameter] public double LongPressMoveTolerance { get; set; } = 10;

    /// <summary>
    /// Gets or sets the minimum interval, in milliseconds, between <see cref="GestureMoved"/> callbacks.
    /// </summary>
    [Parameter] public int MoveThrottleInterval { get; set; } = 50;

    /// <summary>
    /// Gets or sets the browser touch-action behavior applied to the gestures element.
    /// </summary>
    [Parameter] public GestureTouchAction TouchAction { get; set; } = GestureTouchAction.Auto;

    /// <summary>
    /// Raised when a gesture starts.
    /// </summary>
    [Parameter] public EventCallback<GestureEventArgs> GestureStarted { get; set; }

    /// <summary>
    /// Raised while an active gesture moves. The callback is throttled by <see cref="MoveThrottleInterval"/>.
    /// </summary>
    [Parameter] public EventCallback<GestureEventArgs> GestureMoved { get; set; }

    /// <summary>
    /// Raised when a gesture ends or is canceled.
    /// </summary>
    [Parameter] public EventCallback<GestureEventArgs> GestureEnded { get; set; }

    /// <summary>
    /// Raised when a completed gesture is recognized as a swipe.
    /// </summary>
    [Parameter] public EventCallback<SwipeEventArgs> Swiped { get; set; }

    /// <summary>
    /// Raised when a completed gesture is recognized as a tap.
    /// </summary>
    [Parameter] public EventCallback<TapEventArgs> Tapped { get; set; }

    /// <summary>
    /// Raised when an active gesture is held long enough to be recognized as a long press.
    /// </summary>
    [Parameter] public EventCallback<LongPressEventArgs> LongPressed { get; set; }

    #endregion
}