#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Provides data for the <see cref="Validations.StatusChanged"/> event.
    /// </summary>
    public class ValidationsStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the default <see cref="ValidationsStatusChangedEventArgs"/>.
        /// </summary>
        public static new readonly ValidationsStatusChangedEventArgs Empty = new( ValidationStatus.None, null );

        /// <summary>
        /// A default <see cref="ValidationsStatusChangedEventArgs"/> constructor.
        /// </summary>
        public ValidationsStatusChangedEventArgs( ValidationStatus status, IReadOnlyCollection<string> messages )
        {
            Status = status;
            Messages = messages;
        }

        /// <summary>
        /// Gets the validation result.
        /// </summary>
        public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets the custom validation message.
        /// </summary>
        public IReadOnlyCollection<string> Messages { get; }
    }
}
