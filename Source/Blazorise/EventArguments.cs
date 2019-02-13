#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public class ChangingEventArgs : CancelEventArgs
    {
        public ChangingEventArgs( string oldValue, string newValue )
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ChangingEventArgs( string oldValue, string newValue, bool cancel )
        {
            OldValue = oldValue;
            NewValue = newValue;
            Cancel = cancel;
        }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }

    public class ValidateEventArgs : EventArgs
    {
        public ValidateEventArgs( object value )
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value to check for validation.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets or sets the validation status.
        /// </summary>
        public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the custom validation error message.
        /// </summary>
        public string ErrorText { get; set; }
    }

    public class ValidationSucceededEventArgs : EventArgs
    {
    }

    public class ValidationFailedEventArgs : EventArgs
    {
        public ValidationFailedEventArgs( string errorText )
        {
            ErrorText = errorText;
        }

        /// <summary>
        /// Gets the custom validation error message.
        /// </summary>
        public string ErrorText { get; }
    }
}
