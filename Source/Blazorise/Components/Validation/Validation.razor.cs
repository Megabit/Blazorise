#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise;

/// <summary>
/// Container for input component that can check for different kind of validations.
/// </summary>
public partial class Validation : ComponentBase, IValidation, IDisposable
{
    #region Members

    /// <summary>
    /// Reference to the validation input.
    /// </summary>
    private IValidationInput inputComponent;

    /// <summary>
    /// Pattern that is being applied for the validation.
    /// </summary>
    private string patternString;

    /// <summary>
    /// Regex pattern used to override the validator handler.
    /// </summary>
    private Regex pattern;

    /// <summary>
    /// Flag that indicates pattern validation has being applied.
    /// </summary>
    private bool hasPattern;

    /// <summary>
    /// Flag that indicates validation has already being initialized.
    /// </summary>
    private bool initialized;

    /// <summary>
    /// Field identifier for the bound value.
    /// </summary>
    private FieldIdentifier fieldIdentifier;

    /// <summary>
    /// Flag that indicates field value has being bound.
    /// </summary>
    private bool hasFieldIdentifier;

    /// <summary>
    /// Raises an event that the validation has started.
    /// </summary>
    public event ValidationStartedEventHandler ValidationStarted;

    /// <summary>
    /// Raises every time a validation state has changed.
    /// </summary>
    public event EventHandler<ValidationStatusChangedEventArgs> ValidationStatusChanged;

    /// <summary>
    /// Define the cancellation token.
    /// </summary>
    private CancellationTokenSource cancellationTokenSource;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( ParentValidations is not null )
        {
            ParentValidations.ClearingAll += OnClearingAll;

            ParentValidations.NotifyValidationInitialized( this );
        }

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if ( ParentValidations is not null )
        {
            // To avoid leaking memory, it's important to detach any event handlers in Dispose()
            ParentValidations.ClearingAll -= OnClearingAll;
            ParentValidations.NotifyValidationRemoved( this );
        }

        cancellationTokenSource?.Dispose();
        cancellationTokenSource = null;
    }

    /// <summary>
    /// Initializes the input component with the specified validation input. It performs an initial validation
    /// if the mode is set to auto validation and validation on load is enabled.
    /// </summary>
    /// <param name="inputComponent">The validation input component to initialize.</param>
    /// <remarks>
    /// This method sets the input component and, based on the configuration, may asynchronously validate
    /// the component's validation value. It also marks the component as initialized.
    /// </remarks>
    public async Task InitializeInput( IValidationInput inputComponent )
    {
        this.inputComponent = inputComponent;

        if ( Mode == ValidationMode.Auto && ValidateOnLoad )
            await ValidateAsync( inputComponent.ValidationValue );

        initialized = true;
    }

    /// <summary>
    /// Initializes or updates the input validation pattern. If the pattern string changes,
    /// a new regex pattern is created, and the input is re-validated if applicable.
    /// </summary>
    /// <typeparam name="T">The type of the value to validate against the pattern.</typeparam>
    /// <param name="patternString">The regex pattern string for validation.</param>
    /// <param name="value">The current value of the input to validate against the new pattern.</param>
    /// <remarks>
    /// This method optimizes performance by avoiding re-instantiation of the regex pattern if it has not changed.
    /// It ensures that validation is only re-triggered if the component has been initialized and validation conditions are met.
    /// </remarks>
    public async Task InitializeInputPattern<T>( string patternString, T value )
    {
        if ( !string.IsNullOrEmpty( patternString ) )
        {
            // We need to re-instantiate patternRegex only if the pattern has changed.
            // Otherwise it could get pretty slow for larger forms.
            if ( !hasPattern || this.patternString != patternString )
            {
                this.patternString = patternString;
                pattern = new( patternString );

                // Re-run validation based on the new value for the new pattern,
                // but ONLY if validation has being previously initialized!
                if ( hasPattern && Mode == ValidationMode.Auto && ValidateOnLoad && initialized )
                {
                    await NotifyInputChanged( value, true );
                }
            }

            hasPattern = true;
        }
    }

    /// <summary>
    /// Initializes or updates the input based on a specified expression. This is primarily used for data-annotation validation.
    /// </summary>
    /// <typeparam name="T">The type of the model the expression evaluates to.</typeparam>
    /// <param name="expression">The expression used to identify the field for validation.</param>
    /// <remarks>
    /// This method is designed to work with models and edit contexts, allowing for dynamic validation based on expressions.
    /// It ensures that validation is only re-triggered if the component has been initialized and validation conditions are met.
    /// </remarks>
    public async Task InitializeInputExpression<T>( Expression<Func<T>> expression )
    {
        // Data-Annotation validation can only work if parent validationa and expression are defined.
        if ( ( ParentValidations is not null || EditContext is not null ) && expression is not null )
        {
            // We need to re-instantiate FieldIdentifier only if the model has changed.
            // Otherwise it could get pretty slow for larger forms.
            if ( !hasFieldIdentifier
                 || ( ParentValidations?.Model?.IsEqual( fieldIdentifier.Model ) == false )
                 || ( EditContext?.Model?.IsEqual( fieldIdentifier.Model ) == false ) )
            {
                fieldIdentifier = FieldIdentifier.Create( expression );

                // Re-run validation based on the new value for the new model,
                // but ONLY if validation has being previously initialized!
                if ( hasFieldIdentifier && Mode == ValidationMode.Auto && ValidateOnLoad && initialized )
                {
                    await NotifyInputChanged( expression.Compile().Invoke(), true );
                }

                hasFieldIdentifier = true;
            }
        }
        else
        {
            hasFieldIdentifier = false;
        }
    }

    /// <summary>
    /// Notifies that an input's value has changed and optionally re-validates the input.
    /// </summary>
    /// <typeparam name="T">The type of the new value.</typeparam>
    /// <param name="newExpressionValue">The new value of the expression.</param>
    /// <param name="overrideNewValue">Determines whether to use the new value for validation or the current input component's validation value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// If the edit context is set and a field identifier has been established, this method notifies the edit context
    /// of the field change. It then conditionally triggers validation based on the component's mode.
    /// </remarks>
    public Task NotifyInputChanged<T>( T newExpressionValue, bool overrideNewValue = false )
    {
        var newValidationValue = overrideNewValue
            ? newExpressionValue
            : inputComponent.ValidationValue;

        if ( EditContext is not null && hasFieldIdentifier )
        {
            EditContext.NotifyFieldChanged( fieldIdentifier );
        }

        if ( Mode == ValidationMode.Auto )
        {
            return TriggerValidation( newValidationValue );
        }

        return Task.CompletedTask;
    }

    private void OnClearingAll()
    {
        Clear();
    }

    /// <summary>
    /// Runs the validation process.
    /// </summary>
    /// <returns>Returns the validation result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input component is not assigned.</exception>
    public ValidationStatus Validate()
    {
        if ( inputComponent is null )
            throw new ArgumentNullException( nameof( inputComponent ), "Input component is not assigned." );

        return Validate( inputComponent.ValidationValue );
    }

    /// <summary>
    /// Runs the validation process.
    /// </summary>
    /// <param name="newValidationValue">New validation value to validate.</param>
    /// <returns>Returns the validation result.</returns>
    public ValidationStatus Validate( object newValidationValue )
    {
        if ( !inputComponent.Disabled )
        {
            var validationHandler = GetValidationHandler();

            validationHandler?.Validate( this, newValidationValue );
        }

        return Status;
    }

    /// <inheritdoc/>
    public Task<ValidationStatus> ValidateAsync()
    {
        if ( inputComponent is null )
            throw new ArgumentNullException( nameof( inputComponent ), "Input component is not assigned." );

        return TriggerValidation( inputComponent.ValidationValue );
    }

    /// <summary>
    /// Runs the asynchronous validation process.
    /// </summary>
    /// <param name="newValidationValue">New validation value to validate.</param>
    /// <returns>Returns the validation result.</returns>
    public Task<ValidationStatus> ValidateAsync( object newValidationValue )
        => TriggerValidation( newValidationValue );

    /// <summary>
    /// Runs the asynchronous validation process using the current input value. Can be used to manually retrigger validation.
    /// </summary>
    /// <returns>Returns the validation result.</returns>
    public Task<ValidationStatus> RetriggerValidation()
    {
        if ( inputComponent is null )
            throw new ArgumentNullException( nameof( inputComponent ), "Input component is not assigned." );

        return TriggerValidation( inputComponent.ValidationValue );
    }

    /// <summary>
    /// Runs the asynchronous validation process.
    /// </summary>
    /// <param name="newValidationValue">New validation value to validate.</param>
    /// <returns>Returns the validation result.</returns>
    private async Task<ValidationStatus> TriggerValidation( object newValidationValue )
    {
        if ( !inputComponent.Disabled )
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();

            // Create a CTS for this request.
            cancellationTokenSource = new();

            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                var validationHandler = GetValidationHandler();

                if ( validationHandler is not null )
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await validationHandler.ValidateAsync( this, newValidationValue, cancellationToken );
                }
            }
            catch ( OperationCanceledException )
            {
            }
        }

        return await Task.FromResult( Status );
    }

    /// <summary>
    /// Gets the ValidationHandler for this context.
    /// </summary>
    /// <returns></returns>
    private IValidationHandler GetValidationHandler()
    {
        var validationHandlerType = DetermineHandlerType();

        return ( validationHandlerType is not null )
            ? ValidationHandlerFactory.Create( validationHandlerType )
            : null;
    }

    /// <summary>
    /// Determines the validation handler based on the priority.
    /// </summary>
    /// <returns></returns>
    protected virtual Type DetermineHandlerType()
    {
        if ( HandlerType is null )
        {
            if ( Validator is not null || AsyncValidator is not null )
            {
                return ValidationHandlerType.Validator;
            }
            else if ( UsePattern && hasPattern )
            {
                return ValidationHandlerType.Pattern;
            }
            else if ( EditContext is not null && hasFieldIdentifier )
            {
                // In case we have a handler defined on a parent validation we will use that.
                if ( ParentValidations?.HandlerType is not null )
                {
                    return ParentValidations.HandlerType;
                }

                return ValidationHandlerType.DataAnnotation;
            }
            else
                throw new NotImplementedException( "Unable to determine the validator type." );
        }

        return HandlerType;
    }

    /// <summary>
    /// Clears the validation status.
    /// </summary>
    public void Clear()
    {
        _ = NotifyValidationStatusChanged( ValidationStatus.None );
    }

    /// <inheritdoc/>
    public void NotifyValidationStarted()
    {
        ValidationStarted?.Invoke();
    }

    /// <inheritdoc/>
    public async Task NotifyValidationStatusChanged( ValidationStatus status, IEnumerable<string> messages = null )
    {
        // raise events only if status or message is changed to prevent unnecessary re-renders
        if ( Status != status || ( Messages?.AreEqual( messages ) == false ) )
        {
            Status = status;
            Messages = messages;

            ValidationStatusChanged?.Invoke( this, new( status, messages ) );
            await InvokeAsync( () => StatusChanged.InvokeAsync( status ) );

            if ( ParentValidations is not null )
                await ParentValidations.NotifyValidationStatusChanged( this );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the validation mode.
    /// </summary>
    private ValidationMode Mode => ParentValidations?.Mode ?? ValidationMode.Auto;

    /// <summary>
    /// Gets the activation mode when in auto mode.
    /// </summary>
    private bool ValidateOnLoad => ParentValidations?.ValidateOnLoad ?? true;

    /// <inheritdoc/>
    public IEnumerable<string> Messages { get; private set; }

    /// <inheritdoc/>
    public FieldIdentifier FieldIdentifier => fieldIdentifier;

    /// <inheritdoc/>
    public Regex Pattern => pattern;

    /// <summary>
    /// Gets or sets the DI reference for the <see cref="IValidationHandlerFactory"/>.
    /// </summary>
    [Inject] protected IValidationHandlerFactory ValidationHandlerFactory { get; set; }

    /// <inheritdoc/>
    [Parameter] public ValidationStatus Status { get; set; }

    /// <summary>
    /// Occurs each time that validation status changed.
    /// </summary>
    [Parameter] public EventCallback<ValidationStatus> StatusChanged { get; set; }

    /// <inheritdoc/>
    [Parameter] public Action<ValidatorEventArgs> Validator { get; set; }

    /// <inheritdoc/>
    [Parameter] public Func<ValidatorEventArgs, CancellationToken, Task> AsyncValidator { get; set; }

    /// <inheritdoc/>
    [Parameter] public Func<string, IEnumerable<string>, string> MessageLocalizer { get; set; }

    /// <inheritdoc/>
    [CascadingParameter] public Validations ParentValidations { get; protected set; }

    /// <inheritdoc/>
    [CascadingParameter] public EditContext EditContext { get; protected set; }

    /// <summary>
    /// Forces validation to use regex pattern matching instead of default validator handler.
    /// </summary>
    [Parameter] public bool UsePattern { get; set; }

    /// <summary>
    /// Forces the custom validation handler to be used while validating the values.
    /// </summary>
    [Parameter] public Type HandlerType { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Validation"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}