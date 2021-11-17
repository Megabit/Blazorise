#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the dropdown state.
    /// </summary>
    public class DropdownStateEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="DropdownStateEventArgs"/> constructor.
        /// </summary>
        /// <param name="visible">Visibility flag.</param>
        public DropdownStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the dropdown is opened.
        /// </summary>
        public bool Visible { get; }
    }
}
