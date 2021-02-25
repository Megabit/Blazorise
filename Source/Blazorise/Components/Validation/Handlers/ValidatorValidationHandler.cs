#region Using directives
using System.Linq;
using Blazorise.Extensions;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default handler implementation to validate <see cref="IValidation"/> using the <see cref="ValidationRule"/> methods.
    /// </summary>
    public class ValidatorValidationHandler : IValidationHandler
    {
        /// <inheritdoc/>
        public void Validate( IValidation validation, object newValidationValue )
        {
            validation.NotifyValidationStarted();

            var validatorEventArgs = new ValidatorEventArgs( newValidationValue );

            validation.Validator?.Invoke( validatorEventArgs );

            var matchMessages = validatorEventArgs.Status == ValidationStatus.Error && !string.IsNullOrEmpty( validatorEventArgs.ErrorText )
                ? new string[] { validatorEventArgs.ErrorText }
                : null;

            validation.NotifyValidationStatusChanged( validatorEventArgs.Status, matchMessages );
        }
    }
}
