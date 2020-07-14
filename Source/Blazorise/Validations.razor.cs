#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Container for multiple validations.
    /// </summary>
    public partial class Validations : ComponentBase
    {
        #region Members

        /// <summary>
        /// Raises an intent to validate all validations inside of this container.
        /// </summary>
        public event ValidatingAllEventHandler ValidatingAll;

        public event ClearAllValidationsEventHandler ClearingAll;

        /// <summary>
        /// Event is fired whenever there is a change in validation status.
        /// </summary>
        internal event ValidationsStatusChangedEventHandler _StatusChanged;

        /// <summary>
        /// List of validations placed inside of this container.
        /// </summary>
        private List<IValidation> validations = new List<IValidation>();

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( Model != null )
            {
                EditContext = new EditContext( Model );
            }

            base.OnInitialized();
        }

        /// <summary>
        /// Runs the validation process for all validations and returns false if any is failed.
        /// </summary>
        public bool ValidateAll()
        {
            var result = TryValidateAll();

            if ( result )
            {
                RaiseStatusChanged( ValidationStatus.Success, null );

                ValidatedAll.InvokeAsync( null );
            }
            else if ( HasFailedValidations )
            {
                RaiseStatusChanged( ValidationStatus.Error, FailedValidations );
            }

            return result;
        }

        /// <summary>
        /// Clears all validation statuses.
        /// </summary>
        public void ClearAll()
        {
            ClearingAll?.Invoke();

            RaiseStatusChanged( ValidationStatus.None, null );
        }

        private bool TryValidateAll()
        {
            var validated = true;

            var handler = ValidatingAll;

            if ( handler != null )
            {
                var args = new ValidatingAllEventArgs( false );

                foreach ( ValidatingAllEventHandler subHandler in handler?.GetInvocationList() )
                {
                    subHandler( args );

                    if ( args.Cancel )
                    {
                        validated = false;
                    }
                }
            }

            return validated;
        }

        internal void NotifyValidationInitialized( IValidation validation )
        {
            if ( !validations.Contains( validation ) )
            {
                validations.Add( validation );
            }
        }

        internal void NotifyValidationStatusChanged( IValidation validation )
        {
            // Here we need to call ValidatedAll only when in Auto mode. Manuall call is already called through ValidateAll()
            if ( Mode == ValidationMode.Manual )
                return;

            // NOTE: there is risk of calling RaiseStatusChanged multiple times for every field error.
            // Try to come up with solution that StatusChanged will be called only once while it will
            // still provide all of the failed messages.

            if ( AllValidationsSuccessful )
            {
                RaiseStatusChanged( ValidationStatus.Success, null );

                ValidatedAll.InvokeAsync( null );
            }
            else if ( HasFailedValidations )
            {
                RaiseStatusChanged( ValidationStatus.Error, FailedValidations );
            }
            else
            {
                RaiseStatusChanged( ValidationStatus.None, null );
            }
        }

        private void RaiseStatusChanged( ValidationStatus status, IReadOnlyCollection<string> messages )
        {
            _StatusChanged?.Invoke( new ValidationsStatusChangedEventArgs( status, messages ) );

            StatusChanged.InvokeAsync( new ValidationsStatusChangedEventArgs( status, messages ) );
        }

        #endregion

        #region Properties

        protected EditContext EditContext { get; set; }

        /// <summary>
        /// Defines the validation mode for validations inside of this container.
        /// </summary>
        [Parameter] public ValidationMode Mode { get; set; } = ValidationMode.Auto;

        /// <summary>
        /// If set to true, and <see cref="Mode"/> is set to <see cref="ValidationMode.Auto"/>, validation will run on page load.
        /// </summary>
        [Parameter] public bool ValidateOnLoad { get; set; } = true;

        /// <summary>
        /// Specifies the top-level model object for the form. An edit context will be constructed for this model.
        /// </summary>
        [Parameter] public object Model { get; set; }

        /// <summary>
        /// Message that will be displayed if any of the validations does not have defined error message.
        /// </summary>
        [Parameter] public string MissingFieldsErrorMessage { get; set; }

        /// <summary>
        /// Event is fired only after all of the validation are successful.
        /// </summary>
        [Parameter] public EventCallback ValidatedAll { get; set; }

        /// <summary>
        /// Event is fired whenever there is a change in validation status.
        /// </summary>
        [Parameter] public EventCallback<ValidationsStatusChangedEventArgs> StatusChanged { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool AllValidationsSuccessful
            => validations.All( x => x.Status == ValidationStatus.Success );

        private bool HasFailedValidations
            => validations.Any( x => x.Status == ValidationStatus.Error );

        private IReadOnlyCollection<string> FailedValidations
        {
            get
            {
                return validations
                    .Where( x => x.Status == ValidationStatus.Error && !string.IsNullOrWhiteSpace( x.LastErrorMessage ) )
                    .Select( x => x.LastErrorMessage )
                    .Concat(
                        // In case there are some fields that do not have error message we need to combine them all under one message.
                        validations.Any( v => v.Status == ValidationStatus.Error
                            && string.IsNullOrWhiteSpace( v.LastErrorMessage )
                            && !validations.Where( v2 => v2.Status == ValidationStatus.Error && !string.IsNullOrWhiteSpace( v2.LastErrorMessage ) ).Contains( v ) )
                        ? new string[] { MissingFieldsErrorMessage ?? "One or more fields have an error. Please check and try again." }
                        : new string[] { } )
                    .ToList();
            }
        }

        #endregion
    }
}
