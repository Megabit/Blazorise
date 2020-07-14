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
    public partial class Validation : ComponentBase, IValidation
    {
        #region Members

        /// <summary>
        /// Reference to the validation input.
        /// </summary>
        private IValidationInput inputComponent;

        /// <summary>
        /// Holds the last input value.
        /// </summary>
        private object lastKnownValue;

        /// <summary>
        /// Regex pattern used to override the validator handler.
        /// </summary>
        private Regex patternRegex;

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
            }
        }

        internal void InitializeInput( IValidationInput inputComponent )
        {
            this.inputComponent = inputComponent;

            // save the input value
            lastKnownValue = inputComponent.ValidationValue;

            if ( Mode == ValidationMode.Auto && ValidateOnLoad )
                Validate();
        }

        internal void InitializeInputPattern( string pattern )
        {
            if ( !string.IsNullOrEmpty( pattern ) )
                this.patternRegex = new Regex( pattern );
        }

        internal void InitializeInputExpression<T>( Expression<Func<T>> expression )
        {
            if ( expression == null )
                return;

            fieldIdentifier = FieldIdentifier.Create( expression );
            hasFieldIdentifier = true;
        }

        internal void NotifyInputChanged()
        {
            if ( inputComponent.ValidationValue is Array )
            {
                if ( !Comparers.AreEqual( this.lastKnownValue as Array, inputComponent.ValidationValue as Array ) )
                {
                    this.lastKnownValue = inputComponent.ValidationValue;

                    if ( EditContext != null && hasFieldIdentifier )
                    {
                        EditContext.NotifyFieldChanged( fieldIdentifier );
                    }

                    if ( Mode == ValidationMode.Auto )
                        Validate();
                }
            }
            else
            {
                if ( this.lastKnownValue != inputComponent.ValidationValue )
                {
                    this.lastKnownValue = inputComponent.ValidationValue;

                    if ( EditContext != null && hasFieldIdentifier )
                    {
                        EditContext.NotifyFieldChanged( fieldIdentifier );
                    }

                    if ( Mode == ValidationMode.Auto )
                        Validate();
                }
            }
        }

        private void OnValidatingAll( ValidatingAllEventArgs e )
        {
            e.Cancel = Validate() == ValidationStatus.Error;
        }

        private void OnClearingAll()
        {
            Clear();
        }

        /// <summary>
        /// Runs the validation process.
        /// </summary>
        public ValidationStatus Validate()
        {
            if ( UsePattern && patternRegex != null )
            {
                var matchStatus = patternRegex.IsMatch( inputComponent.ValidationValue?.ToString() ?? string.Empty )
                    ? ValidationStatus.Success
                    : ValidationStatus.Error;

                if ( Status != matchStatus )
                {
                    Status = matchStatus;

                    NotifyValidationStatusChanged( Status );
                }
            }
            else if ( EditContext != null && hasFieldIdentifier )
            {
                ValidationStarted?.Invoke();

                var messages = new ValidationMessageStore( EditContext );

                EditContext.ValidateField( messages, fieldIdentifier );

                Status = messages[fieldIdentifier].Any() ? ValidationStatus.Error : ValidationStatus.Success;
                LastErrorMessage = Status == ValidationStatus.Error ? string.Join( "; ", messages[fieldIdentifier] ) : null;

                NotifyValidationStatusChanged( Status, LastErrorMessage );
            }
            else
            {
                var validatorHandler = Validator;

                if ( validatorHandler != null )
                {
                    ValidationStarted?.Invoke();

                    var validatorEventArgs = new ValidatorEventArgs( inputComponent.ValidationValue );

                    validatorHandler( validatorEventArgs );

                    if ( Status != validatorEventArgs.Status )
                    {
                        Status = validatorEventArgs.Status;
                        LastErrorMessage = Status == ValidationStatus.Error ? validatorEventArgs.ErrorText : null;

                        NotifyValidationStatusChanged( Status, LastErrorMessage );
                    }
                }
            }

            return Status;
        }

        /// <summary>
        /// Clears the validation status.
        /// </summary>
        public void Clear()
        {
            Status = ValidationStatus.None;
            NotifyValidationStatusChanged( Status );
        }

        private void NotifyValidationStatusChanged( ValidationStatus status, string message = null )
        {
            ValidationStatusChanged?.Invoke( this, new ValidationStatusChangedEventArgs( status, message ) );

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
        /// Gets or sets the current validation status.
        /// </summary>
        [Parameter] public ValidationStatus Status { get; set; }

        /// <summary>
        /// Gets the last error message.
        /// </summary>
        public string LastErrorMessage { get; private set; }

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

        [CascadingParameter] protected EditContext EditContext { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }

    public interface IValidationInput
    {
        /// <summary>
        /// Gets the input value prepared for the validation check.
        /// </summary>
        /// <remarks>
        /// This is mostly used to handle special inputs where there can be more than one
        /// value types. For example a Select component can have single-value and multi-value.
        /// </remarks>
        object ValidationValue { get; }
    }
}
