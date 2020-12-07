#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ValidationStatusChangedEventArgs : EventArgs
    {
        public static new readonly ValidationStatusChangedEventArgs Empty = new ValidationStatusChangedEventArgs( ValidationStatus.None );

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
