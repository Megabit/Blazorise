#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the bar-dropdown state.
    /// </summary>
    public class BarDropdownStateEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="BarDropdownStateEventArgs"/> constructor.
        /// </summary>
        /// <param name="visible">Visibility flag.</param>
        public BarDropdownStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the dropdown is opened.
        /// </summary>
        public bool Visible { get; }
    }
}
