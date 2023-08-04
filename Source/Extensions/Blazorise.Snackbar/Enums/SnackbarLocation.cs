#region Using directives
using System;
#endregion

namespace Blazorise.Snackbar;

/// <summary>
/// Defines the snackbar location.
/// </summary>
public enum SnackbarLocation
{
    /// <summary>
    /// Default behavior.
    /// </summary>
    [Obsolete( "Use Bottom instead." )]
    Default,

    /// <summary>
    /// Show the snackbar on the bottom side of the screen.
    /// </summary>
    Bottom,

    /// <summary>
    /// Show the snackbar on the bottom-left side of the screen.
    /// </summary>
    BottomStart,

    /// <summary>
    /// Show the snackbar on the bottom-right side of the screen.
    /// </summary>
    BottomEnd,

    /// <summary>
    /// Show the snackbar on the left side of the screen.
    /// </summary>
    [Obsolete( "Use BottomStart instead." )]
    Start,

    /// <summary>
    /// Show the snackbar on the right side of the screen.
    /// </summary>
    [Obsolete( "Use BottomEnd instead." )]
    End,

    /// <summary>
    /// Show the snackbar on the top side of the screen.
    /// </summary>
    Top,

    /// <summary>
    /// Show the snackbar on the top-left side of the screen.
    /// </summary>
    TopStart,

    /// <summary>
    /// Show the snackbar on the top-right side of the screen.
    /// </summary>
    TopEnd,
}
