﻿#region Using directives
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        /// <inheritdoc/>
        public async Task ValidateAsync( IValidation validation, CancellationToken cancellationToken, object newValidationValue )
        {
            cancellationToken.ThrowIfCancellationRequested();

            validation.NotifyValidationStarted();

            var validatorEventArgs = new ValidatorEventArgs( newValidationValue );

            cancellationToken.ThrowIfCancellationRequested();

            if ( validation.AsyncValidator != null )
                await validation.AsyncValidator( validatorEventArgs, cancellationToken );
            else
                validation.Validator?.Invoke( validatorEventArgs );

            var matchMessages = validatorEventArgs.Status == ValidationStatus.Error && !string.IsNullOrEmpty( validatorEventArgs.ErrorText )
                ? new string[] { validatorEventArgs.ErrorText }
                : null;

            cancellationToken.ThrowIfCancellationRequested();

            validation.NotifyValidationStatusChanged( validatorEventArgs.Status, matchMessages );
        }
    }
}
