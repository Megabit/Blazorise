﻿using System;

namespace Blazorise;

/// <summary>
/// Supplies information about a pointer gesture.
/// </summary>
public class GestureEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the current gesture direction.
    /// </summary>
    public GestureDirection Direction { get; set; }

    /// <summary>
    /// Gets or sets the starting client X coordinate.
    /// </summary>
    public double StartClientX { get; set; }

    /// <summary>
    /// Gets or sets the starting client Y coordinate.
    /// </summary>
    public double StartClientY { get; set; }

    /// <summary>
    /// Gets or sets the current or ending client X coordinate.
    /// </summary>
    public double ClientX { get; set; }

    /// <summary>
    /// Gets or sets the current or ending client Y coordinate.
    /// </summary>
    public double ClientY { get; set; }

    /// <summary>
    /// Gets or sets the horizontal movement from the starting point.
    /// </summary>
    public double DeltaX { get; set; }

    /// <summary>
    /// Gets or sets the vertical movement from the starting point.
    /// </summary>
    public double DeltaY { get; set; }

    /// <summary>
    /// Gets or sets the total movement distance from the starting point.
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Gets or sets the gesture duration in milliseconds.
    /// </summary>
    public double Duration { get; set; }

    /// <summary>
    /// Gets or sets the movement velocity in pixels per millisecond.
    /// </summary>
    public double Velocity { get; set; }

    /// <summary>
    /// Gets or sets the pointer type reported by the browser.
    /// </summary>
    public string PointerType { get; set; }

    /// <summary>
    /// Gets or sets whether the gesture was canceled.
    /// </summary>
    public bool Canceled { get; set; }
}