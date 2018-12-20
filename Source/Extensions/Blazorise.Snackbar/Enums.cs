#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Snackbar
{
    /// <summary>
    /// Defines the snackbar location.
    /// </summary>
    public enum SnackbarLocation
    {
        /// <summary>
        /// Default behaviour.
        /// </summary>
        None,

        /// <summary>
        /// Show the snackbar on the left side of the screen.
        /// </summary>
        Left,

        /// <summary>
        /// Show the snackbar on the right side of the screen.
        /// </summary>
        Right,
    }
}
