#region Using directives
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for the validation component.
/// </summary>
public interface IValidation
{
    /// <summary>
    /// Gets the last received validation status.
    /// </summary>
    ValidationStatus Status { get; }

    /// <summary>
    /// Gets the list of last received validation messages.
    /// </summary>
    IEnumerable<string> Messages { get; }

    /// <summary>
    /// Gets the validator handler attached to this validation.
    /// </summary>
    Action<ValidatorEventArgs> Validator { get; }

    /// <summary>
    /// Gets the asynchronous validator handler attached to this validation.
    /// </summary>
    Func<ValidatorEventArgs, CancellationToken, Task> AsyncValidator { get; }

    /// <summary>
    /// Gets the pattern regex attached to this validation.
    /// </summary>
    Regex Pattern { get; }

    /// <summary>
    /// Gets the field identifier for the validation, if data-annotations is used.
    /// </summary>
    FieldIdentifier FieldIdentifier { get; }

    /// <summary>
    /// Gets the parent validation group.
    /// </summary>
    Validations ParentValidations { get; }

    /// <summary>
    /// Gets the <see cref="EditContext"/> attached to the parent <see cref="Validations"/>.
    /// </summary>
    EditContext EditContext { get; }

    /// <summary>
    /// Overrides the message that is going to be shown on the <see cref="ValidationError"/> or <see cref="ValidationSuccess"/>.
    /// </summary>
    Func<string, IEnumerable<string>, string> MessageLocalizer { get; }

    /// <summary>
    /// Notifies the validation that validation process has being activated.
    /// </summary>
    void NotifyValidationStarted();

    /// <summary>
    /// Notifies the validation with the new status and messages.
    /// </summary>
    /// <param name="status">New <see cref="ValidationStatus"/>.</param>
    /// <param name="messages">New error or success message(s).</param>
    Task NotifyValidationStatusChanged( ValidationStatus status, IEnumerable<string> messages = null );

    /// <summary>
    /// Runs the asynchronous validation process based on the last available value.
    /// </summary>
    public Task<ValidationStatus> ValidateAsync();

    /// <summary>
    /// Retriggers the asynchronous validation process using the most recent value.
    /// </summary>
    Task<ValidationStatus> RetriggerValidation();
}