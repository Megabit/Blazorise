﻿#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default handler implementation to validate <see cref="IValidation"/> using the regex pattern.
    /// </summary>
    public class PatternValidationHandler : IValidationHandler
    {
        /// <inheritdoc/>
        public void Validate( IValidation validation, object newValidationValue )
        {
            validation.NotifyValidationStarted();

            var matchStatus = validation.Pattern.IsMatch( newValidationValue?.ToString() ?? string.Empty )
                ? ValidationStatus.Success
                : ValidationStatus.Error;

            validation.NotifyValidationStatusChanged( matchStatus );
        }
    }
}
