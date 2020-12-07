#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the bar state.
    /// </summary>
    public class BarStateEventArgs : EventArgs
    {
        public BarStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the bar is opened.
        /// </summary>
        public bool Visible { get; }
    }
}
