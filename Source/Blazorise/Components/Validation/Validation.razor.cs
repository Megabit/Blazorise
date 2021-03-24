#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise
{
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

        protected override Task OnInitializedAsync()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll += OnValidatingAll;
                ParentValidations.ClearingAll += OnClearingAll;

                ParentValidations.NotifyValidationInitialized( this );
            }

            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            if ( ParentValidations != null )
            {
                // To avoid leaking memory, it's important to detach any event handlers in Dispose()
                ParentValidations.ValidatingAll -= OnValidatingAll;
                ParentValidations.ClearingAll -= OnClearingAll;
                ParentValidations.NotifyValidationRemoved( this );
            }
        }

        internal async Task InitializeInput( IValidationInput inputComponent )
        {
            this.inputComponent = inputComponent;

            if ( Mode == ValidationMode.Auto && ValidateOnLoad )
                await ValidateAsync( inputComponent.ValidationValue );

            initialized = true;
        }

        internal async Task InitializeInputPattern<T>( string patternString, T value )
        {
            if ( !string.IsNullOrEmpty( patternString ) )
            {
                // We need to re-instantiate patternRegex only if the pattern has changed.
                // Otherwise it could get pretty slow for larger forms.
                if ( !hasPattern || this.patternString != patternString )
                {
                    this.patternString = patternString;
                    pattern = new Regex( patternString );

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

        internal async Task InitializeInputExpression<T>( Expression<Func<T>> expression )
        {
            // Data-Annotation validation can only work if parent validationa and expression are defined.
            if ( ( ParentValidations != null || EditContext != null ) && expression != null )
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

        internal async Task NotifyInputChanged<T>( T newExpressionValue, bool overrideNewValue = false )
        {
            var newValidationValue = overrideNewValue
                ? newExpressionValue
                : inputComponent.ValidationValue;

            if ( EditContext != null && hasFieldIdentifier )
            {
                EditContext.NotifyFieldChanged( fieldIdentifier );
            }

            if ( Mode == ValidationMode.Auto )
            {
                await ValidateAsync( newValidationValue );
            }
        }

        private async void OnValidatingAll( ValidatingAllEventArgs e )
        {
            var status = await ValidateAsync( inputComponent.ValidationValue );

            e.Cancel = status == ValidationStatus.Error;
        }

        private void OnClearingAll()
        {
            Clear();
        }

        /// <summary>
        /// Runs the validation process based on the last available value.
        /// </summary>
        public ValidationStatus Validate()
        {
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
                var validationHandlerType = DetermineHandlerType();

                if ( validationHandlerType != null )
                {
                    var validationHandler = ValidationHandlerFactory.Create( validationHandlerType );

                    validationHandler.Validate( this, newValidationValue );
                }
            }

            return Status;
        }

        /// <summary>
        /// Runs the asynchronous validation process based on the last available value.
        /// </summary>
        public Task<ValidationStatus> ValidateAsync()
        {
            return ValidateAsync( inputComponent.ValidationValue );
        }

        /// <summary>
        /// Runs the asynchronous validation process.
        /// </summary>
        /// <param name="newValidationValue">New validation value to validate.</param>
        /// <returns>Returns the validation result.</returns>
        public async Task<ValidationStatus> ValidateAsync( object newValidationValue )
        {
            if ( !inputComponent.Disabled )
            {
                if ( cancellationTokenSource != null )
                    cancellationTokenSource.Cancel();

                // Create a CTS for this request.
                cancellationTokenSource = new CancellationTokenSource();

                var cancellationToken = cancellationTokenSource.Token;

                try
                {
                    var validationHandlerType = DetermineHandlerType();

                    cancellationToken.ThrowIfCancellationRequested();

                    if ( validationHandlerType != null )
                    {
                        var validationHandler = ValidationHandlerFactory.Create( validationHandlerType );

                        cancellationToken.ThrowIfCancellationRequested();

                        await validationHandler.ValidateAsync( this, cancellationToken, newValidationValue );
                    }
                }
                catch ( OperationCanceledException )
                {
                }
            }

            return await Task.FromResult( Status );
        }

        /// <summary>
        /// Determines the validation handler based on the priority.
        /// </summary>
        /// <returns></returns>
        protected virtual Type DetermineHandlerType()
        {
            if ( HandlerType == null )
            {
                if ( Validator != null || AsyncValidator != null )
                {
                    return typeof( ValidatorValidationHandler );
                }
                else if ( UsePattern && hasPattern )
                {
                    return typeof( PatternValidationHandler );
                }
                else if ( EditContext != null && hasFieldIdentifier )
                {
                    return typeof( DataAnnotationValidationHandler );
                }
                else
                    throw new NotImplementedException( "Unable to determine the validator " );
            }

            return HandlerType;
        }

        /// <summary>
        /// Clears the validation status.
        /// </summary>
        public void Clear()
        {
            NotifyValidationStatusChanged( ValidationStatus.None );
        }

        /// <inheritdoc/>
        public void NotifyValidationStarted()
        {
            ValidationStarted?.Invoke();
        }

        /// <inheritdoc/>
        public void NotifyValidationStatusChanged( ValidationStatus status, IEnumerable<string> messages = null )
        {
            // raise events only if status or message is changed to prevent unnecessary re-renders
            if ( Status != status || ( Messages?.AreEqual( messages ) == false ) )
            {
                Status = status;
                Messages = messages;

                ValidationStatusChanged?.Invoke( this, new ValidationStatusChangedEventArgs( status, messages ) );
                InvokeAsync( () => StatusChanged.InvokeAsync( status ) );

                ParentValidations?.NotifyValidationStatusChanged( this );
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
        /// Forces the custom validation handler to be uses while validating the values.
        /// </summary>
        [Parameter] public Type HandlerType { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Validation"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}