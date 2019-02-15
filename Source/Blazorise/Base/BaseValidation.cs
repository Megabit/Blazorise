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

        protected override void OnAfterRender()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll += OnValidatingAll;
            }

            base.OnAfterRender();
        }

        public void Dispose()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll -= OnValidatingAll;
            }
        }

        internal void InitInputValue( object value )
        {
            // save the input value
            this.value = value;

            if ( Mode == ValidationMode.Auto )
                Validate();
        }

        internal void UpdateInputValue( object value )
        {
            // save the last input value
            if ( this.value != value )
            {
                this.value = value;

                if ( Mode == ValidationMode.Auto )
                    Validate();
            }
        }

        private void OnValidatingAll()
        {
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

            // force the reload of all child components
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
