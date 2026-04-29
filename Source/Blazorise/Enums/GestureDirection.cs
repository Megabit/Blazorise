﻿using System;

namespace Blazorise;

/// <summary>
/// Defines gesture movement directions.
/// </summary>
[Flags]
public enum GestureDirection
{
    /// <summary>
    /// No direction.
    /// </summary>
    None = 0,

    /// <summary>
    /// Left direction.
    /// </summary>
    Left = 1,

    /// <summary>
    /// Right direction.
    /// </summary>
    Right = 2,

    /// <summary>
    /// Up direction.
    /// </summary>
    Up = 4,

    /// <summary>
    /// Down direction.
    /// </summary>
    Down = 8,

    /// <summary>
    /// Horizontal directions.
    /// </summary>
    Horizontal = Left | Right,

    /// <summary>
    /// Vertical directions.
    /// </summary>
    Vertical = Up | Down,

    /// <summary>
    /// All directions.
    /// </summary>
    All = Horizontal | Vertical,
}