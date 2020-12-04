#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ValidationsStatusChangedEventArgs : EventArgs
    {
        public static new readonly ValidationsStatusChangedEventArgs Empty = new ValidationsStatusChangedEventArgs( ValidationStatus.None, null );

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
