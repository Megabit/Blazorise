#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Callback handlers for pointer gesture notifications.
/// </summary>
public class GestureEventHandlers
{
    /// <summary>
    /// Gets an empty gesture handlers instance.
    /// </summary>
    public static GestureEventHandlers Empty => new();

    /// <summary>
    /// Gets or sets the callback raised when a gesture starts.
    /// </summary>
    public Func<GestureEventArgs, Task> GestureStarted { get; set; }

    /// <summary>
    /// Gets or sets the callback raised while an active gesture moves.
    /// </summary>
    public Func<GestureEventArgs, Task> GestureMoved { get; set; }

    /// <summary>
    /// Gets or sets the callback raised when a gesture ends or is canceled.
    /// </summary>
    public Func<GestureEventArgs, Task> GestureEnded { get; set; }

    /// <summary>
    /// Gets or sets the callback raised when a completed gesture is recognized as a swipe.
    /// </summary>
    public Func<SwipeEventArgs, Task> Swiped { get; set; }

    /// <summary>
    /// Gets or sets the callback raised when a completed gesture is recognized as a tap.
    /// </summary>
    public Func<TapEventArgs, Task> Tapped { get; set; }

    /// <summary>
    /// Gets or sets the callback raised when an active gesture is held long enough to be recognized as a long press.
    /// </summary>
    public Func<LongPressEventArgs, Task> LongPressed { get; set; }
}