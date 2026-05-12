namespace Blazorise;

/// <summary>
/// Configures pointer gesture detection.
/// </summary>
public class GestureOptions
{
    /// <summary>
    /// Gets or sets whether gesture handling is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the directions that can produce swipe events.
    /// </summary>
    public GestureDirection Direction { get; set; } = GestureDirection.All;

    /// <summary>
    /// Gets or sets the minimum swipe distance in pixels.
    /// </summary>
    public double SwipeThreshold { get; set; } = 50;

    /// <summary>
    /// Gets or sets the minimum swipe velocity in pixels per millisecond.
    /// </summary>
    public double SwipeVelocityThreshold { get; set; } = 0.3;

    /// <summary>
    /// Gets or sets the maximum movement in pixels that can still be recognized as a tap.
    /// </summary>
    public double TapMaximumDistance { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum duration in milliseconds that can still be recognized as a tap.
    /// </summary>
    public int TapMaximumDuration { get; set; } = 300;

    /// <summary>
    /// Gets or sets the duration in milliseconds required to recognize a long press.
    /// </summary>
    public int LongPressDuration { get; set; } = 500;

    /// <summary>
    /// Gets or sets the movement tolerance in pixels before a pending long press is canceled.
    /// </summary>
    public double LongPressMoveTolerance { get; set; } = 10;

    /// <summary>
    /// Gets or sets the minimum interval, in milliseconds, between move callbacks.
    /// </summary>
    public int MoveThrottleInterval { get; set; } = 50;

    /// <summary>
    /// Gets or sets the browser touch-action behavior.
    /// </summary>
    public GestureTouchAction TouchAction { get; set; } = GestureTouchAction.Auto;

    /// <summary>
    /// Gets or sets whether native browser dragging is prevented inside the gesture area.
    /// </summary>
    public bool PreventNativeDrag { get; set; } = true;
}