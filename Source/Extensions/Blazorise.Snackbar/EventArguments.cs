#region Using directives
using System;
#endregion

namespace Blazorise.Snackbar;

/// <summary>
/// Provides information about <see cref="Snackbar.Closed"/> event.
/// </summary>
public class SnackbarClosedEventArgs : EventArgs
{
    /// <summary>
    /// Captures the snackbar identity and the event that dismissed it.
    /// </summary>
    /// <param name="key">Application key assigned to the snackbar.</param>
    /// <param name="closeReason">Condition that caused dismissal.</param>
    public SnackbarClosedEventArgs( string key, SnackbarCloseReason closeReason )
    {
        Key = key;
        CloseReason = closeReason;
    }

    /// <summary>
    /// Gets the key associated with the closed snackbar.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets a value that indicates why the snackbar is being closed.
    /// </summary>
    public SnackbarCloseReason CloseReason { get; }
}