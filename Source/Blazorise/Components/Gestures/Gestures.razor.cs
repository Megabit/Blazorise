#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Detects pointer-based gestures on the wrapped content.
/// </summary>
public partial class Gestures : BaseComponent, IAsyncDisposable
{
    #region Members

    private IGestureSubscription gestureSubscription;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        gestureSubscription = await GestureService.Attach( ElementRef, ElementId, CreateOptions(), CreateEventHandlers() );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if ( Rendered && gestureSubscription is not null )
        {
            ExecuteAfterRender( async () => await gestureSubscription.Update( CreateOptions(), CreateEventHandlers() ) );
        }

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( gestureSubscription is not null )
            {
                await gestureSubscription.DisposeAsync();
                gestureSubscription = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles a gesture start notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture start information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OnGestureStarted( GestureEventArgs eventArgs )
        => GestureStarted.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a gesture move notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture move information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OnGestureMoved( GestureEventArgs eventArgs )
        => GestureMoved.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a gesture end notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture end information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OnGestureEnded( GestureEventArgs eventArgs )
        => GestureEnded.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a swipe notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Swipe information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OnSwiped( SwipeEventArgs eventArgs )
        => Swiped.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a tap notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Tap information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OnTapped( TapEventArgs eventArgs )
        => Tapped.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles a long-press notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Long-press information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task OnLongPressed( LongPressEventArgs eventArgs )
        => LongPressed.InvokeAsync( eventArgs );

    private GestureOptions CreateOptions()
        => new()
        {
            Disabled = Disabled,
            Direction = Direction,
            SwipeThreshold = SwipeThreshold,
            SwipeVelocityThreshold = SwipeVelocityThreshold,
            TapMaximumDistance = TapMaximumDistance,
            TapMaximumDuration = TapMaximumDuration,
            LongPressDuration = LongPressDuration,
            LongPressMoveTolerance = LongPressMoveTolerance,
            MoveThrottleInterval = MoveThrottleInterval,
            TouchAction = TouchAction,
            PreventNativeDrag = PreventNativeDrag,
        };

    private GestureEventHandlers CreateEventHandlers()
        => new()
        {
            GestureStarted = GestureStarted.HasDelegate ? OnGestureStarted : null,
            GestureMoved = GestureMoved.HasDelegate ? OnGestureMoved : null,
            GestureEnded = GestureEnded.HasDelegate ? OnGestureEnded : null,
            Swiped = Swiped.HasDelegate ? OnSwiped : null,
            Tapped = Tapped.HasDelegate ? OnTapped : null,
            LongPressed = LongPressed.HasDelegate ? OnLongPressed : null,
        };

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Specifies the gesture service.
    /// </summary>
    [Inject] public IGestureService GestureService { get; set; }

    /// <summary>
    /// Gets or sets the content that will be observed for gestures.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Disables gesture handling.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

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
    /// Gets or sets whether native browser dragging is prevented inside the gesture area.
    /// </summary>
    [Parameter] public bool PreventNativeDrag { get; set; } = true;

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