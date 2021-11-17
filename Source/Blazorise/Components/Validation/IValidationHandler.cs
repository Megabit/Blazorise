#region Using directives
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Main interface used to handle the validation for the given <see cref="IValidation"/> and it's value.
    /// </summary>
    public interface IValidationHandler
    {
        /// <summary>
        /// Validates the <see cref="IValidation"/> component.
        /// </summary>
        /// <param name="validation">Reference to the validation component.</param>
        /// <param name="newValidationValue">Value to validate.</param>
        void Validate( IValidation validation, object newValidationValue );

        /// <summary>
        /// Asynchronously validates the <see cref="IValidation"/> component.
        /// </summary>
        /// <param name="validation">Reference to the validation component.</param>
        /// <param name="newValidationValue">Value to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ValidateAsync( IValidation validation, object newValidationValue, CancellationToken cancellationToken = default );
    }
}
