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
        /// <summary>
        /// A default <see cref="BarStateEventArgs"/> constructor.
        /// </summary>
        /// <param name="visible">Visibility flag.</param>
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
