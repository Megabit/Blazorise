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
    /// Show the snackbar on the bottom-start side of the screen.
    /// </summary>
    BottomStart,

    /// <summary>
    /// Show the snackbar on the bottom-end side of the screen.
    /// </summary>
    BottomEnd,

    /// <summary>
    /// Show the snackbar on the bottom-start side of the screen.
    /// </summary>
    [Obsolete( "Use BottomStart instead." )]
    Start,

    /// <summary>
    /// Show the snackbar on the bottom-end side of the screen.
    /// </summary>
    [Obsolete( "Use BottomEnd instead." )]
    End,

    /// <summary>
    /// Show the snackbar on the top side of the screen.
    /// </summary>
    Top,

    /// <summary>
    /// Show the snackbar on the top-start side of the screen.
    /// </summary>
    TopStart,

    /// <summary>
    /// Show the snackbar on the top-end side of the screen.
    /// </summary>
    TopEnd
}
