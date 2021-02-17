#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the alert visiblity state.
    /// </summary>
    public class AlertStateEventArgs : EventArgs
    {
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
