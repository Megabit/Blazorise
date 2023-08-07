namespace Blazorise.Snackbar;

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