#region Using directives
#endregion

using System;

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
    TopEnd
}

/// <summary>
/// Defines the snackbar stack location.
/// </summary>
public enum SnackbarStackLocation
{
    /// <summary>
    /// Default behavior.
    /// </summary>
    [Obsolete( "Use Bottom instead" )]
    Center,

    Bottom,
    /// <summary>
    /// Show the snackbar stack on the left side of the screen.
    /// </summary>
    [Obsolete( "Use BottomStart instead" )]
    Start,

    BottomStart,
    /// <summary>
    /// Show the snackbar stack on the right side of the screen.
    /// </summary>
    [Obsolete( "Use BottomEnd instead" )]
    End,

    BottomEnd,
    /// <summary>
    /// Show the snackbar stack on the top side of the screen.
    /// </summary>
    Top,

    /// <summary>
    /// Show the snackbar stack on the top-right side of the screen.
    /// </summary>
    TopStart,

    /// <summary>
    /// Show the snackbar stack on the top-left side of the screen.
    /// </summary>
    TopEnd
}

/// <summary>
/// Predefined set of contextual colors.
/// </summary>
public enum SnackbarColor
{
    /// <summary>
    /// No color will be applied to an element.
    /// </summary>
    Default,

    /// <summary>
    /// Primary color.
    /// </summary>
    Primary,

    /// <summary>
    /// Secondary color.
    /// </summary>
    Secondary,

    /// <summary>
    /// Success color.
    /// </summary>
    Success,

    /// <summary>
    /// Danger color.
    /// </summary>
    Danger,

    /// <summary>
    /// Warning color.
    /// </summary>
    Warning,

    /// <summary>
    /// Info color.
    /// </summary>
    Info,

    /// <summary>
    /// Light color.
    /// </summary>
    Light,

    /// <summary>
    /// Dark color.
    /// </summary>
    Dark,
}

/// <summary>
/// Specifies the reason that a snackbar was closed.
/// </summary>
public enum SnackbarCloseReason
{
    /// <summary>
    /// Snackbar is closed automatically by internal timer or by other unknown reason.
    /// </summary>
    None,

    /// <summary>
    /// Snackbar is closed by the user.
    /// </summary>
    UserClosed,
}