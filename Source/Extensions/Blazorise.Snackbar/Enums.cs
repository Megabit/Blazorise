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
        /// Default behavior.
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

    /// <summary>
    /// Predefined set of contextual colors.
    /// </summary>
    public enum SnackbarColor
    {
        /// <summary>
        /// No color will be applied to an element.
        /// </summary>
        None,

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
}
