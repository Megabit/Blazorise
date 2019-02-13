#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseValidation : ComponentBase
    {
        #region Members

        /// <summary>
        /// Holds the last input value.
        /// </summary>
        private object value;

        /// <summary>
        /// Input component that is validated.
        /// </summary>
        private BaseInputComponent inputComponent;

        /// <summary>
        /// Raises an event that the validation has started.
        /// </summary>
        public event ValidatingEventHandler Validating;

        /// <summary>
        /// Raises an event after the validation has passes successfully.
        /// </summary>
        public event ValidationSucceededEventHandler ValidationSucceeded;

        /// <summary>
        /// Raises an event after the validation has failed.
        /// </summary>
        public event ValidationFailedEventHandler ValidationFailed;

        #endregion

        #region Methods

        public void Dispose()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll -= OnValidatingAll;
            }
        }

        protected override void OnAfterRender()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll += OnValidatingAll;
            }

            base.OnAfterRender();
        }

        private void OnValidatingAll()
        {
            Validate();
        }

        internal void Hook( BaseInputComponent inputComponent, object value )
        {
            this.inputComponent = inputComponent;

            InputValueChanged( value );
        }

        internal void InputValueChanged( object value )
        {
            // save the last input value
            this.value = value;

            if ( Mode == ValidationMode.Auto )
                Validate();
        }

        /// <summary>
        /// Runs the validation process.
        /// </summary>
        public void Validate()
        {
            var handler = Validator;

            if ( handler != null )
            {
                Validating?.Invoke();

                var args = new ValidatorEventArgs( value );

                handler( args );

                Status = args.Status;

                if ( args.Status == ValidationStatus.Success )
                    ValidationSucceeded?.Invoke( new ValidationSucceededEventArgs() );
                else if ( args.Status == ValidationStatus.Error )
                    ValidationFailed?.Invoke( new ValidationFailedEventArgs( args.ErrorText ) );
            }

            // input component can also change it's status classes
            inputComponent?.Dirty();

            StateHasChanged();
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
        [Parameter] protected internal ValidationStatus Status { get; set; }

        /// <summary>
        /// Validates the input value after it has being changed.
        /// </summary>
        [Parameter] protected Action<ValidatorEventArgs> Validator { get; set; }

        [CascadingParameter] protected BaseValidations ParentValidations { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
