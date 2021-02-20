#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base validation contract for all inputs that wants to have validation support.
    /// </summary>
    public interface IValidationInput
    {
        /// <summary>
        /// Gets the input value prepared for the validation check.
        /// </summary>
        /// <remarks>
        /// This is mostly used to handle special inputs where there can be more than one
        /// value types. For example a Select component can have single-value and multi-value.
        /// </remarks>
        object ValidationValue { get; }

        /// <summary>
        /// Returns true if input is disabled.
        /// </summary>
        bool Disabled { get; }
    }
}
