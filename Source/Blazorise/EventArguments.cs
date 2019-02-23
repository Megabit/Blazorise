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

    public class ValidatingAllEventArgs : CancelEventArgs
    {
        public ValidatingAllEventArgs( bool cancel )
            : base( cancel )
        {
        }
    }

    public class ValidatorEventArgs : EventArgs
    {
        public ValidatorEventArgs( object value )
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value to check for validation.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the validation custom error message.
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

    public class ValidatedEventArgs : EventArgs
    {
        public ValidatedEventArgs( ValidationStatus status, string errorText )
        {
            Status = status;
            ErrorText = errorText;
        }

        /// <summary>
        /// Gets the validation result.
        /// </summary>
        public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets the custom validation error message.
        /// </summary>
        public string ErrorText { get; }
    }
}
