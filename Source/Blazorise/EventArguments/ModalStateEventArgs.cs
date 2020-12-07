#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the modal visiblity state.
    /// </summary>
    public class ModalStateEventArgs : EventArgs
    {
        public ModalStateEventArgs( bool visible )
        {
            Visible = visible;
        }

        /// <summary>
        /// Gets that flag that indicates if the modal is opened.
        /// </summary>
        public bool Visible { get; }
    }
}
