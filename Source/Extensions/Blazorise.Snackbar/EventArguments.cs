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
        public SnackbarClosedEventArgs( SnackbarCloseReason closeReason )
        {
            CloseReason = closeReason;
        }

        /// <summary>
        /// Gets a value that indicates why the snackbar is being closed.
        /// </summary>
        public SnackbarCloseReason CloseReason { get; }
    }
}
