#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseValidation : ComponentBase
    {
        #region Members

        /// <summary>
        /// Holds the last input value.
        /// </summary>
        private object value;

        /// <summary>
        /// Regex pattern used to override the validator handler.
        /// </summary>
        private Regex patternRegex;

        /// <summary>
        /// Raises an event that the validation has started.
        /// </summary>
        public event ValidatingEventHandler Validating;

        /// <summary>
        /// Raises every time a validation state has changed.
        /// </summary>
        public event EventHandler<ValidationStatusChangedEventArgs> ValidationStatusChanged;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll += OnValidatingAll;
                ParentValidations.ClearingAll += OnClearingAll;
            }

            base.OnInitialized();
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

        internal void InitInputValue( object value )
        {
            // save the input value
            this.value = value;

            if ( Mode == ValidationMode.Auto )
                Validate();
        }

        internal void InitInputPattern( string pattern )
        {
            if ( !string.IsNullOrEmpty( pattern ) )
                this.patternRegex = new Regex( pattern );
        }

        internal void UpdateInputValue( object value )
        {
            if ( value is Array )
            {
                if ( !Comparers.AreEqual( this.value as Array, value as Array ) )
                {
                    this.value = value;

                    if ( Mode == ValidationMode.Auto )
                        Validate();
                }
            }
            else
            {
                if ( this.value != value )
                {
                    this.value = value;

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
                var matchStatus = patternRegex.IsMatch( value?.ToString() ?? string.Empty )
                    ? ValidationStatus.Success
                    : ValidationStatus.Error;

                if ( Status != matchStatus )
                {
                    Status = matchStatus;

                    ValidationStatusChanged?.Invoke( this, new ValidationStatusChangedEventArgs( Status ) );

                    //StateHasChanged();
                }
            }
            else
            {
                var validatorHandler = Validator;

                if ( validatorHandler != null )
                {
                    Validating?.Invoke();

                    var validatorEventArgs = new ValidatorEventArgs( value );

                    validatorHandler( validatorEventArgs );

                    if ( Status != validatorEventArgs.Status )
                    {
                        Status = validatorEventArgs.Status;

                        ValidationStatusChanged?.Invoke( this, new ValidationStatusChangedEventArgs( Status, Status == ValidationStatus.Error ? validatorEventArgs.ErrorText : null ) );

                        //StateHasChanged();
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
            ValidationStatusChanged?.Invoke( this, new ValidationStatusChangedEventArgs( Status ) );

            //StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the validation mode.
        /// </summary>
        private ValidationMode Mode => ParentValidations?.Mode ?? ValidationMode.Auto;

        /// <summary>
        /// Gets or sets the current validation status.
        /// </summary>
        [Parameter] public ValidationStatus Status { get; set; }

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
        [CascadingParameter] public BaseValidations ParentValidations { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
