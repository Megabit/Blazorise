#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the alert visibility state.
    /// </summary>
    public class AlertStateEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="AlertStateEventArgs"/> constructor.
        /// </summary>
        /// <param name="visible">Visibility flag.</param>
        public AlertStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the alert is visible.
        /// </summary>
        public bool Visible { get; }
    }
}
