#region Using directives
#endregion

using System.Threading.Tasks;

namespace Blazorise
{
    /// <summary>
    /// Default handler implementation to validate <see cref="IValidation"/> using the regex pattern.
    /// </summary>
    public class PatternValidationHandler : IValidationHandler
    {
        /// <inheritdoc/>
        public Task Validate( IValidation validation, object newValidationValue )
        {
            validation.NotifyValidationStarted();

            var matchStatus = validation.Pattern.IsMatch( newValidationValue?.ToString() ?? string.Empty )
                ? ValidationStatus.Success
                : ValidationStatus.Error;

            validation.NotifyValidationStatusChanged( matchStatus );

            return Task.CompletedTask;
        }
    }
}
