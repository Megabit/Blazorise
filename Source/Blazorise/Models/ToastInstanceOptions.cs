﻿using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Sets the options for Toast instance.
/// </summary>
public class ToastInstanceOptions
{
    /// <summary>
    /// Occurs after the toast has opened.
    /// </summary>
    public EventCallback? Opened { get; set; }

    /// <summary>
    /// Occurs after the toast has closed.
    /// </summary>
    public EventCallback? Closed { get; set; }

    /// <summary>
    /// Specifies whether the Toast should have an animated transition.
    /// </summary>
    public bool? Animated { get; set; }

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// </summary>
    public int? AnimationDuration { get; set; }

    /// <summary>
    /// Automatically hide the toast after the delay.
    /// </summary>
    public bool? Autohide { get; set; }

    /// <summary>
    /// Delay in milliseconds before hiding the toast.
    /// </summary>
    public double? AutohideDelay { get; set; }

    /// <summary>
    /// Creates the default toast options.
    /// </summary>
    /// <returns>Default toast options.</returns>
    public static ToastInstanceOptions Default => new()
    {
    };
}
