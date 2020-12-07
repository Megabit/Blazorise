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
