using System;

namespace Blazorise;

/// <summary>
/// Defines the side on which to apply the gap spacing.
/// </summary>
public enum GapSide
{
    /// <summary>
    /// No side.
    /// </summary>
    None,

    /// <summary>
    /// Left and right side.
    /// </summary>
    X,

    /// <summary>
    /// Top and bottom side.
    /// </summary>
    Y,

    /// <summary>
    /// All 4 sides of the element.
    /// </summary>
    All,
}