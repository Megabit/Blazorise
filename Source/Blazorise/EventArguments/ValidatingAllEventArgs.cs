#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides data for the <see cref="Validations.ValidatingAll"/> cancellable event.
    /// </summary>
    public class ValidatingAllEventArgs : CancelEventArgs
    {
        /// <summary>
        /// A default <see cref="ValidatingAllEventArgs"/> constructor.
        /// </summary>
        public ValidatingAllEventArgs( bool cancel )
            : base( cancel )
        {
        }
    }
}
