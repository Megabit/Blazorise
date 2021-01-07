#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
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

        /// <summary>
        /// Gets the collection of member names that indicate which fields have validation errors.
        /// </summary>
        public IEnumerable<string> MemberNames { get; set; }
    }
}
