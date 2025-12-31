#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise;

/// <summary>
/// Container for multiple validations and an <see cref="EditContext"/>.
/// </summary>
public partial class Validations : ComponentBase
{
    #region Members

    /// <summary>
    /// Raises an intent that validations are going to be cleared.
    /// </summary>
    public event ClearAllValidationsEventHandler ClearingAll;

    /// <summary>
    /// Event is fired whenever there is a change in validation status.
    /// </summary>
    internal event ValidationsStatusChangedEventHandler StatusChangedInternal;

    /// <summary>
    /// List of validations placed inside of this container.
    /// </summary>
    private readonly List<IValidation> validations = new();

    private EditContext editContext;

    private bool hasSetEditContextExplicitly;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if ( hasSetEditContextExplicitly && Model is not null )
        {
            throw new InvalidOperationException( $"{nameof( Validations )} requires a {nameof( Model )} parameter, or an {nameof( EditContext )} parameter, but not both." );
        }

        // Update editContext if we don't have one yet, or if they are supplying a
        // potentially new EditContext, or if they are supplying a different Model
        if ( Model is not null && Model != editContext?.Model )
        {
            editContext = new( Model );
        }
    }

    /// <summary>
    /// Asynchronously runs the validation process for all validations and returns false if any is failed.
    /// </summary>
    public async Task<bool> ValidateAll()
    {
        var result = await TryValidateAll();

        if ( result )
        {
            RaiseStatusChanged( ValidationStatus.Success, null, null );

            await InvokeAsync( () => ValidatedAll.InvokeAsync() );
        }
        else if ( HasFailedValidations )
        {
            RaiseStatusChanged( ValidationStatus.Error, FailedValidations, null );
        }

        return result;
    }

    /// <summary>
    /// Clears all validation statuses.
    /// </summary>
    public Task ClearAll()
    {
        ClearingAll?.Invoke();

        RaiseStatusChanged( ValidationStatus.None, null, null );

        return Task.CompletedTask;
    }

    private async Task<bool> TryValidateAll()
    {
        var validated = true;

        foreach ( var validation in validations ?? Enumerable.Empty<IValidation>() )
        {
            if ( await validation.ValidateAsync() == ValidationStatus.Error )
                validated = false;
        }

        return validated;
    }

    /// <summary>
    /// Notifies the validation system that a new validation component has been initialized and adds it to the list of validations if not already present.
    /// </summary>
    /// <param name="validation">The validation component to add.</param>
    public void NotifyValidationInitialized( IValidation validation )
    {
        if ( !validations.Contains( validation ) )
        {
            validations.Add( validation );
        }
    }

    /// <summary>
    /// Notifies the validation system that a validation component is being removed and removes it from the list of validations if present.
    /// </summary>
    /// <param name="validation">The validation component to remove.</param>
    public void NotifyValidationRemoved( IValidation validation )
    {
        validations.Remove( validation );
    }

    /// <summary>
    /// Notifies the validation system that the status of a validation component has changed. This method handles the logic for updating the overall validation status based on the mode (Auto or Manual) and the current validation results.
    /// </summary>
    /// <param name="validation">The validation component whose status has changed.</param>
    /// <remarks>
    /// In Auto mode, this method triggers the aggregation of validation results and potentially raises a status changed event.
    /// It is designed to minimize the number of status changed events by aggregating validation results.
    /// Special consideration is needed to ensure that the status changed event is raised only once per validation cycle,
    /// even if multiple validations fail.
    /// </remarks>
    public void NotifyValidationStatusChanged( IValidation validation )
    {
        // Here we need to call ValidatedAll only when in Auto mode. Manual call is already called through ValidateAll()
        if ( Mode == ValidationMode.Manual )
            return;

        // NOTE: there is risk of calling RaiseStatusChanged multiple times for every field error.
        // Try to come up with solution that StatusChanged will be called only once while it will
        // still provide all of the failed messages.

        if ( AllValidationsSuccessful )
        {
            RaiseStatusChanged( ValidationStatus.Success, null, validation );

            ValidatedAll.InvokeAsync();
        }
        else if ( HasFailedValidations )
        {
            RaiseStatusChanged( ValidationStatus.Error, FailedValidations, validation );
        }
        else
        {
            RaiseStatusChanged( ValidationStatus.None, null, validation );
        }
    }

    private void RaiseStatusChanged( ValidationStatus status, IReadOnlyCollection<string> messages, IValidation validation )
    {
        var eventArgs = new ValidationsStatusChangedEventArgs( status, messages, validation );

        StatusChangedInternal?.Invoke( eventArgs );

        InvokeAsync( () => StatusChanged.InvokeAsync( eventArgs ) );

        _ = RaiseFailedValidationsChangedAsync( messages ?? Array.Empty<string>() );
    }

    private Task RaiseFailedValidationsChangedAsync( IReadOnlyCollection<string> messages )
    {
        if ( !FailedValidationsChanged.HasDelegate )
            return Task.CompletedTask;

        var errorMessages = messages?.ToList() ?? new List<string>();

        return InvokeAsync( () => FailedValidationsChanged.InvokeAsync( new FailedValidationsEventArgs( errorMessages ) ) );
    }

    /// <summary>
    /// Retriggers validations that match the given <see cref="FieldIdentifier"/> instances.
    /// </summary>
    /// <param name="fieldIdentifiers">Field identifiers for which validations should be retriggered.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RetriggerValidation( params FieldIdentifier[] fieldIdentifiers )
    {
        if ( fieldIdentifiers is null || fieldIdentifiers.Length == 0 )
            return Task.CompletedTask;

        var matchingValidations = validations?
            .Where( v => v.FieldIdentifier.Model is not null )
            .Where( v => fieldIdentifiers.Any( f => f.FieldName == v.FieldIdentifier.FieldName && f.Model == v.FieldIdentifier.Model ) )
            .ToList();

        return matchingValidations?.Count > 0
            ? Task.WhenAll( matchingValidations.Select( v => v.RetriggerValidation() ) )
            : Task.CompletedTask;
    }

    /// <summary>
    /// Retriggers validations for the specified model fields.
    /// </summary>
    /// <param name="expressions">Expressions representing the fields to revalidate.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RetriggerValidation( params Expression<Func<object>>[] expressions )
    {
        if ( expressions is null || expressions.Length == 0 )
            return Task.CompletedTask;

        var fieldIdentifiers = expressions
            .Select( FieldIdentifier.Create )
            .ToArray();

        return RetriggerValidation( fieldIdentifiers );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the validation mode for validations inside of this container.
    /// </summary>
    [Parameter] public ValidationMode Mode { get; set; } = ValidationMode.Auto;

    /// <summary>
    /// If set to true, and <see cref="Mode"/> is set to <see cref="ValidationMode.Auto"/>, validation will run automatically on page load.
    /// </summary>
    /// <remarks>
    /// When validation is placed inside of modal dialog, the behavior is a little different. 
    /// Modals are by definition always loaded and are always present in the DOM so no loading is ever happening again
    /// after the page that contains the modal is first initialized.  Their visibility is controlled by display: none;
    /// To workaround this, the actual "first load" for modals can be done by re-initializing <see cref="Model"/> parameter. 
    /// </remarks>
    [Parameter] public bool ValidateOnLoad { get; set; } = true;

    /// <summary>
    /// Supplies the edit context explicitly. If using this parameter, do not
    /// also supply <see cref="Model"/>, since the model value will be taken
    /// from the <see cref="EditContext.Model"/> property.
    /// </summary>
    [Parameter]
    public EditContext EditContext
    {
        get => editContext;
        set
        {
            editContext = value;

            hasSetEditContextExplicitly = value is not null;
        }
    }

    /// <summary>
    /// Specifies the top-level model object for the form. An edit context will be constructed for this model.
    /// If using this parameter, do not also supply a value for <see cref="EditContext"/>.
    /// </summary>
    [Parameter] public object Model { get; set; }

    /// <summary>
    /// Message that will be displayed if any of the validations does not have defined error message.
    /// </summary>
    [Parameter] public string MissingFieldsErrorMessage { get; set; }

    /// <summary>
    /// Defines the default handler type that will be used by the validation, unless it is overriden by <see cref="Validation.HandlerType"/> property.
    /// </summary>
    [Parameter] public Type HandlerType { get; set; }

    /// <summary>
    /// Event is fired only after all of the validation are successful.
    /// </summary>
    [Parameter] public EventCallback ValidatedAll { get; set; }

    /// <summary>
    /// Event is fired whenever there is a change in validation status.
    /// </summary>
    [Parameter] public EventCallback<ValidationsStatusChangedEventArgs> StatusChanged { get; set; }

    /// <summary>
    /// Event is fired whenever the failed validation messages change.
    /// </summary>
    [Parameter] public EventCallback<FailedValidationsEventArgs> FailedValidationsChanged { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Validations"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Indicates if there are any successful validation.
    /// </summary>
    public bool AllValidationsSuccessful
        => validations.All( x => x.Status == ValidationStatus.Success );

    /// <summary>
    /// Indicates if there are any failed validation.
    /// </summary>
    public bool HasFailedValidations
        => validations.Any( x => x.Status == ValidationStatus.Error );

    /// <summary>
    /// Gets the filtered list of failed validations.
    /// </summary>
    public IReadOnlyCollection<string> FailedValidations
    {
        get
        {
            return validations
                .Where( x => x.Status == ValidationStatus.Error && x.Messages?.Count() > 0 )
                .SelectMany( x => x.Messages )
                .Concat(
                    // In case there are some fields that do not have error message we need to combine them all under one message.
                    validations.Any( v => v.Status == ValidationStatus.Error
                                          && ( v.Messages is null || !v.Messages.Any() )
                                          && !validations.Where( v2 => v2.Status == ValidationStatus.Error && v2.Messages?.Count() > 0 ).Contains( v ) )
                        ? new string[] { MissingFieldsErrorMessage ?? "One or more fields have an error. Please check and try again." }
                        : Array.Empty<string>() )
                .ToList();
        }
    }

    #endregion
}