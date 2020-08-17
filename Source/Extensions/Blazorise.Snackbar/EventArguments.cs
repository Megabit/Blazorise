#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Snackbar
{
    /// <summary>
    /// Provides information about <see cref="Snackbar.Closed"/> event.
    /// </summary>
    public class SnackbarClosedEventArgs : EventArgs
    {
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
}
