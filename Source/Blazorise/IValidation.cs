#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base interface for validation component.
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// Gets the last validation status.
        /// </summary>
        ValidationStatus Status { get; }

        /// <summary>
        /// Gets the last error messages.
        /// </summary>
        IEnumerable<string> Messages { get; }
    }
}
