#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class FailedValidationsEventArgs : EventArgs
    {
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
