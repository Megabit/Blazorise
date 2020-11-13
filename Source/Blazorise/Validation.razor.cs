#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazorise.Utils;
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
        /// Holds the last input value.
        /// </summary>
        private object lastValidationValue;

        /// <summary>
        /// Regex pattern used to override the validator handler.
        /// </summary>
        private Regex patternRegex;

        // Flag that indicates validation has already being initialized.
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

        internal void InitializeInput( IValidationInput inputComponent )
        {
            this.inputComponent = inputComponent;

            // save the input value
            lastValidationValue = inputComponent.ValidationValue;

            if ( Mode == ValidationMode.Auto && ValidateOnLoad )
                Validate( inputComponent.ValidationValue );

            initialized = true;
        }

        internal void InitializeInputPattern( string pattern )
        {
            if ( !string.IsNullOrEmpty( pattern ) )
                patternRegex = new Regex( pattern );
        }

        internal void InitializeInputExpression<T>( Expression<Func<T>> expression )
        {
            if ( expression != null )
            {
                // We need to re-instantiate FieldIdentifier only if the model has changed.
                // Otherwise it could get pretty slow for larger forms.
                if ( !hasFieldIdentifier || ParentValidations?.Model != fieldIdentifier.Model )
                {
                    fieldIdentifier = FieldIdentifier.Create( expression );

                    // Re-run validation based on the new value for the new model,
                    // but ONLY if validation has being previously initialized!
                    if ( hasFieldIdentifier && Mode == ValidationMode.Auto && ValidateOnLoad && initialized )
                    {
                        NotifyInputChanged( expression.Compile().Invoke(), true );
                    }

                    hasFieldIdentifier = true;
                }
            }
        }

        internal void NotifyInputChanged( object newExpressionValue = null, bool overrideNewValue = false )
        {
            var newValidationValue = overrideNewValue
                ? newExpressionValue
                : inputComponent.ValidationValue;

            var valueChanged = newValidationValue is Array newArrayValue
                ? !Comparers.AreArraysEqual( lastValidationValue as Array, newArrayValue )
                : lastValidationValue != newValidationValue;

            if ( valueChanged )
            {
                lastValidationValue = newValidationValue;

                if ( EditContext != null && hasFieldIdentifier )
                {
                    EditContext.NotifyFieldChanged( fieldIdentifier );
                }

                if ( Mode == ValidationMode.Auto )
                    Validate( newValidationValue );
            }
        }

        private void OnValidatingAll( ValidatingAllEventArgs e )
        {
            e.Cancel = Validate( inputComponent.ValidationValue ) == ValidationStatus.Error;
        }

        private void OnClearingAll()
        {
            Clear();
        }

        /// <summary>
        /// Runs the validation process.
        /// </summary>
        public ValidationStatus Validate( object newValidationValue )
        {
            if ( Validator != null )
            {
                ValidateUsingValidator( Validator, newValidationValue );
            }
            else if ( UsePattern && patternRegex != null )
            {
                ValidateUsingPattern( newValidationValue );
            }
            else if ( EditContext != null && hasFieldIdentifier )
            {
                ValidateUsingDataAnnotation( newValidationValue );
            }

            return Status;
        }

        protected virtual void ValidateUsingValidator( Action<ValidatorEventArgs> validatorHandler, object newValidationValue )
        {
            ValidationStarted?.Invoke();

            var validatorEventArgs = new ValidatorEventArgs( newValidationValue );

            validatorHandler( validatorEventArgs );

            var matchMessages = Status == ValidationStatus.Error
                ? new string[] { validatorEventArgs.ErrorText }
                : null;

            if ( Status != validatorEventArgs.Status || !Comparers.AreEqual( Messages?.ToArray(), matchMessages ) )
            {
                Status = validatorEventArgs.Status;
                Messages = matchMessages;

                NotifyValidationStatusChanged( Status, Messages );
            }
        }

        protected virtual void ValidateUsingDataAnnotation( object newValidationValue )
        {
            ValidationStarted?.Invoke();

            var messages = new ValidationMessageStore( EditContext );

            EditContextValidator.ValidateField( EditContext, messages, fieldIdentifier, MessageLocalizer ?? Options.ValidationMessageLocalizer );

            var matchStatus = messages[fieldIdentifier].Any()
                ? ValidationStatus.Error
                : ValidationStatus.Success;

            var matchMessages = matchStatus == ValidationStatus.Error
                ? messages[fieldIdentifier]
                : null;

            // Sometime status will stay the same and error message will change
            // eg. StringLength > empty string > Required
            if ( Status != matchStatus || !Comparers.AreEqual( Messages?.ToArray(), matchMessages?.ToArray() ) )
            {
                Status = matchStatus;
                Messages = matchMessages;

                NotifyValidationStatusChanged( Status, Messages );
            }
        }

        protected virtual void ValidateUsingPattern( object newValidationValue )
        {
            var matchStatus = patternRegex.IsMatch( newValidationValue?.ToString() ?? string.Empty )
                      ? ValidationStatus.Success
                      : ValidationStatus.Error;

            if ( Status != matchStatus )
            {
                Status = matchStatus;

                NotifyValidationStatusChanged( Status );
            }
        }

        /// <summary>
        /// Clears the validation status.
        /// </summary>
        public void Clear()
        {
            Status = ValidationStatus.None;
            NotifyValidationStatusChanged( Status );
        }

        private void NotifyValidationStatusChanged( ValidationStatus status, IEnumerable<string> messages = null )
        {
            ValidationStatusChanged?.Invoke( this, new ValidationStatusChangedEventArgs( status, messages ) );
            StatusChanged.InvokeAsync( status );

            ParentValidations?.NotifyValidationStatusChanged( this );
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

        /// <summary>
        /// Gets the list of last validation messages.
        /// </summary>
        public IEnumerable<string> Messages { get; private set; }

        /// <summary>
        /// Overrides the message that is going to be shown on the <see cref="ValidationError"/> or <see cref="ValidationSuccess"/>.
        /// </summary>
        [Parameter] public Func<string, IEnumerable<string>, string> MessageLocalizer { get; set; }

        /// <summary>
        /// Injects a default or custom EditContext validator.
        /// </summary>
        [Inject] protected IEditContextValidator EditContextValidator { get; set; }

        /// <summary>
        /// Global blazorise options.
        /// </summary>
        [Inject] protected BlazoriseOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the current validation status.
        /// </summary>
        [Parameter] public ValidationStatus Status { get; set; }

        /// <summary>
        /// Occurs each time that validation status changed.
        /// </summary>
        [Parameter] public EventCallback<ValidationStatus> StatusChanged { get; set; }

        /// <summary>
        /// Validates the input value after it has being changed.
        /// </summary>
        [Parameter] public Action<ValidatorEventArgs> Validator { get; set; }

        /// <summary>
        /// Forces validation to use regex pattern matching instead of default validator handler.
        /// </summary>
        [Parameter] public bool UsePattern { get; set; }

        /// <summary>
        /// Parent validation group.
        /// </summary>
        [CascadingParameter] protected Validations ParentValidations { get; set; }

        /// <summary>
        /// Parent validation edit context.
        /// </summary>
        [CascadingParameter] protected EditContext EditContext { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Validation"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}