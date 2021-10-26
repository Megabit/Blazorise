#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides data for the <see cref="Modal.Opening"/> event.
    /// </summary>
    public class ModalOpeningEventArgs : CancelEventArgs
    {
        /// <summary>
        /// A default <see cref="ModalOpeningEventArgs"/> constructor.
        /// </summary>
        /// <param name="cancel">True if close event should be cancelled.</param>
        public ModalOpeningEventArgs( bool cancel )
            : base( cancel )
        {
        }
    }
}
