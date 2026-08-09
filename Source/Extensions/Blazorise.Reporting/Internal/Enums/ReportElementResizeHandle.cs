#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Defines which element handle is used by a designer resize interaction.
/// </summary>
[Flags]
internal enum ReportElementResizeHandle
{
    /// <summary>
    /// The top edge is resized.
    /// </summary>
    North = 1,

    /// <summary>
    /// The right edge is resized.
    /// </summary>
    East = 2,

    /// <summary>
    /// The bottom edge is resized.
    /// </summary>
    South = 4,

    /// <summary>
    /// The left edge is resized.
    /// </summary>
    West = 8,

    /// <summary>
    /// The top-right corner is resized.
    /// </summary>
    NorthEast = North | East,

    /// <summary>
    /// The bottom-right corner is resized.
    /// </summary>
    SouthEast = South | East,

    /// <summary>
    /// The bottom-left corner is resized.
    /// </summary>
    SouthWest = South | West,

    /// <summary>
    /// The top-left corner is resized.
    /// </summary>
    NorthWest = North | West
}