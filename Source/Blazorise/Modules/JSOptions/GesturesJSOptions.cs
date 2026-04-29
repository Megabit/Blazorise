﻿﻿namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options specific to gesture handling.
/// </summary>
public class GesturesJSOptions
{
    /// <summary>
    /// Gets or sets whether gesture handling is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the directions that can produce swipe events.
    /// </summary>
    public GestureDirection Direction { get; set; }

    /// <summary>
    /// Gets or sets the minimum swipe distance in pixels.
    /// </summary>
    public double SwipeThreshold { get; set; }

    /// <summary>
    /// Gets or sets the minimum swipe velocity in pixels per millisecond.
    /// </summary>
    public double SwipeVelocityThreshold { get; set; }

    /// <summary>
    /// Gets or sets the maximum movement in pixels that can still be recognized as a tap.
    /// </summary>
    public double TapMaximumDistance { get; set; }

    /// <summary>
    /// Gets or sets the maximum duration in milliseconds that can still be recognized as a tap.
    /// </summary>
    public int TapMaximumDuration { get; set; }

    /// <summary>
    /// Gets or sets the duration in milliseconds required to recognize a long press.
    /// </summary>
    public int LongPressDuration { get; set; }

    /// <summary>
    /// Gets or sets the movement tolerance in pixels before a pending long press is canceled.
    /// </summary>
    public double LongPressMoveTolerance { get; set; }

    /// <summary>
    /// Gets or sets the minimum interval, in milliseconds, between move callbacks.
    /// </summary>
    public int MoveThrottleInterval { get; set; }

    /// <summary>
    /// Gets or sets the browser touch-action behavior.
    /// </summary>
    public GestureTouchAction TouchAction { get; set; }

    /// <summary>
    /// Gets or sets whether gesture start notifications are enabled.
    /// </summary>
    public bool NotifyGestureStarted { get; set; }

    /// <summary>
    /// Gets or sets whether gesture move notifications are enabled.
    /// </summary>
    public bool NotifyGestureMoved { get; set; }

    /// <summary>
    /// Gets or sets whether gesture end notifications are enabled.
    /// </summary>
    public bool NotifyGestureEnded { get; set; }

    /// <summary>
    /// Gets or sets whether swipe notifications are enabled.
    /// </summary>
    public bool NotifySwiped { get; set; }

    /// <summary>
    /// Gets or sets whether tap notifications are enabled.
    /// </summary>
    public bool NotifyTapped { get; set; }

    /// <summary>
    /// Gets or sets whether long-press notifications are enabled.
    /// </summary>
    public bool NotifyLongPressed { get; set; }
}