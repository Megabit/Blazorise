#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Data about the <see cref="Validation"/> status change event.
    /// </summary>
    public class ValidationStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets an empty status event.
        /// </summary>
        public static new readonly ValidationStatusChangedEventArgs Empty = new( ValidationStatus.None );

        /// <summary>
        /// A default constructor for <see cref="ValidationStatusChangedEventArgs"/>.
        /// </summary>
        /// <param name="status">Validation status.</param>
        /// <param name="messages">List of validation messages.</param>
        public ValidationStatusChangedEventArgs( ValidationStatus status, IEnumerable<string> messages = null )
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
        public IEnumerable<string> Messages { get; }
    }
}
