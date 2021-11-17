#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides data for the <see cref="Modal.Closing"/> event.
    /// </summary>
    public class ModalClosingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// A default <see cref="ModalClosingEventArgs"/> constructor.
        /// </summary>
        /// <param name="cancel">True if close event should be cancelled.</param>
        /// <param name="closeReason">Reason for closing.</param>
        public ModalClosingEventArgs( bool cancel, CloseReason closeReason )
            : base( cancel )
        {
            CloseReason = closeReason;
        }

        /// <summary>
        /// Gets a value that indicates why the modal is being closed.
        /// </summary>
        public CloseReason CloseReason { get; }
    }
}
