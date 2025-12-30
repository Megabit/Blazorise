#region Using directives
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

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

        _ = validation.NotifyValidationStatusChanged( matchStatus );
    }

    /// <inheritdoc/>
    public async Task ValidateAsync( IValidation validation, object newValidationValue, CancellationToken cancellationToken = default )
    {
        cancellationToken.ThrowIfCancellationRequested();

        validation.NotifyValidationStarted();

        var matchStatus = validation.Pattern.IsMatch( newValidationValue?.ToString() ?? string.Empty )
            ? ValidationStatus.Success
            : ValidationStatus.Error;

        await validation.NotifyValidationStatusChanged( matchStatus );

        cancellationToken.ThrowIfCancellationRequested();
    }
}