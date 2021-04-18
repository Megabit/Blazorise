#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the failed validation.
    /// </summary>
    public class FailedValidationsEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="FailedValidationsEventArgs"/> constructor.
        /// </summary>
        /// <param name="errorMessages">List of error messages.</param>
        public FailedValidationsEventArgs( IReadOnlyList<string> errorMessages )
        {
            ErrorMessages = errorMessages;
        }

        /// <summary>
        /// Gets the list of all error messages.
        /// </summary>
        public IReadOnlyList<string> ErrorMessages { get; }
    }
}
