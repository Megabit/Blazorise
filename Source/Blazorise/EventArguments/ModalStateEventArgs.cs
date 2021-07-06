#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the modal visibility state.
    /// </summary>
    public class ModalStateEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="ModalStateEventArgs"/> constructor.
        /// </summary>
        /// <param name="visible">Visibility flag.</param>
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
