#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Middleman between gesture JavaScript notifications and .NET callbacks.
/// </summary>
public class GestureAdapter
{
    #region Members

    private GestureEventHandlers eventHandlers;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="GestureAdapter"/>.
    /// </summary>
    /// <param name="eventHandlers">Gesture event handlers.</param>
    public GestureAdapter( GestureEventHandlers eventHandlers )
    {
        Update( eventHandlers );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Updates the active gesture event handlers.
    /// </summary>
    /// <param name="eventHandlers">Gesture event handlers.</param>
    public void Update( GestureEventHandlers eventHandlers )
    {
        this.eventHandlers = eventHandlers ?? GestureEventHandlers.Empty;
    }

    /// <summary>
    /// Handles a gesture start notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture start information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnGestureStarted( GestureEventArgs eventArgs )
        => eventHandlers.GestureStarted?.Invoke( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Handles a gesture move notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture move information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnGestureMoved( GestureEventArgs eventArgs )
        => eventHandlers.GestureMoved?.Invoke( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Handles a gesture end notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Gesture end information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnGestureEnded( GestureEventArgs eventArgs )
        => eventHandlers.GestureEnded?.Invoke( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Handles a swipe notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Swipe information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnSwiped( SwipeEventArgs eventArgs )
        => eventHandlers.Swiped?.Invoke( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Handles a tap notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Tap information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnTapped( TapEventArgs eventArgs )
        => eventHandlers.Tapped?.Invoke( eventArgs ) ?? Task.CompletedTask;

    /// <summary>
    /// Handles a long-press notification from the browser.
    /// </summary>
    /// <param name="eventArgs">Long-press information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task OnLongPressed( LongPressEventArgs eventArgs )
        => eventHandlers.LongPressed?.Invoke( eventArgs ) ?? Task.CompletedTask;

    #endregion
}