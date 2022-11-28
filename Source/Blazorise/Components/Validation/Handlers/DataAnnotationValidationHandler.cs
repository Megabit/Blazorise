#region Using directives
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise;

/// <summary>
/// Default handler implementation to validate <see cref="IValidation"/> using the data-annotations.
/// </summary>
public class DataAnnotationValidationHandler : IValidationHandler
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="editContextValidator">Reference to the <see cref="IEditContextValidator"/> object.</param>
    /// <param name="options">Reference to the global blazorise options.</param>
    public DataAnnotationValidationHandler( IEditContextValidator editContextValidator,
        BlazoriseOptions options )
    {
        EditContextValidator = editContextValidator;
        Options = options;
    }

    /// <inheritdoc/>
    public void Validate( IValidation validation, object newValidationValue )
    {
        validation.NotifyValidationStarted();

        var messages = new ValidationMessageStore( validation.EditContext );

        EditContextValidator.ValidateField( validation.EditContext, messages, validation.FieldIdentifier, validation.MessageLocalizer ?? Options.ValidationMessageLocalizer );

        var matchStatus = messages[validation.FieldIdentifier].Any()
            ? ValidationStatus.Error
            : ValidationStatus.Success;

        var matchMessages = matchStatus == ValidationStatus.Error
            ? messages[validation.FieldIdentifier]
            : null;

        validation.NotifyValidationStatusChanged( matchStatus, matchMessages );
    }

    /// <inheritdoc/>
    public Task ValidateAsync( IValidation validation, object newValidationValue, CancellationToken cancellationToken = default )
    {
        cancellationToken.ThrowIfCancellationRequested();

        Validate( validation, newValidationValue );

        cancellationToken.ThrowIfCancellationRequested();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the reference to the <see cref="IEditContextValidator"/> object.
    /// </summary>
    public IEditContextValidator EditContextValidator { get; }

    /// <summary>
    /// Gets the reference to the global blazorise options.
    /// </summary>
    public BlazoriseOptions Options { get; }
}