#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseValidation : BaseComponent
    {
        #region Members

        private object value;

        private BaseInputComponent inputComponent;

        public event ValidationSucceededEventHandler ValidationSucceeded;

        public event ValidationFailedEventHandler ValidationFailed;

        #endregion

        #region Methods

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentValidations != null )
                {
                    ParentValidations.ManualValidation -= OnManualValidation;
                }
            }

            base.Dispose( disposing );
        }

        protected override void OnAfterRender()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ManualValidation += OnManualValidation;
            }

            base.OnAfterRender();
        }

        private void OnManualValidation()
        {
            OnValidate();
        }

        internal void Hook( BaseInputComponent inputComponent )
        {
            this.inputComponent = inputComponent;
        }

        internal void InputValueChanged( object value )
        {
            // save last input value
            this.value = value;

            if ( Mode == ValidationMode.Auto )
                OnValidate();
        }

        private void OnValidate()
        {
            var handler = Validate;

            if ( handler != null )
            {
                var args = new ValidateEventArgs( value );

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
        [Parameter] protected Action<ValidateEventArgs> Validate { get; set; }

        [CascadingParameter] protected BaseValidations ParentValidations { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
