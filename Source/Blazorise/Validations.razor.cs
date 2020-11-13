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
    /// Container for multiple validations and an <see cref="EditContext"/>.
    /// </summary>
    public partial class Validations : ComponentBase
    {
        #region Members

        /// <summary>
        /// Raises an intent to validate all validations inside of this container.
        /// </summary>
        public event ValidatingAllEventHandler ValidatingAll;

        /// <summary>
        /// Raises an intent that validations are going to be cleared.
        /// </summary>
        public event ClearAllValidationsEventHandler ClearingAll;

        /// <summary>
        /// Event is fired whenever there is a change in validation status.
        /// </summary>
        internal event ValidationsStatusChangedEventHandler _StatusChanged;

        /// <summary>
        /// List of validations placed inside of this container.
        /// </summary>
        private List<IValidation> validations = new List<IValidation>();

        private EditContext editContext;

        private bool hasSetEditContextExplicitly;

        #endregion

        #region Methods

        protected override void OnParametersSet()
        {
            if ( hasSetEditContextExplicitly && Model != null )
            {
                throw new InvalidOperationException( $"{nameof( Validations )} requires a {nameof( Model )} parameter, or an {nameof( EditContext )} parameter, but not both." );
            }

            // Update editContext if we don't have one yet, or if they are supplying a
            // potentially new EditContext, or if they are supplying a different Model
            if ( Model != null && Model != editContext?.Model )
            {
                editContext = new EditContext( Model );
            }
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

        internal void NotifyValidationRemoved( IValidation validation )
        {
            if ( validations.Contains( validation ) )
            {
                validations.Remove( validation );
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

                hasSetEditContextExplicitly = value != null;
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
        /// Event is fired only after all of the validation are successful.
        /// </summary>
        [Parameter] public EventCallback ValidatedAll { get; set; }

        /// <summary>
        /// Event is fired whenever there is a change in validation status.
        /// </summary>
        [Parameter] public EventCallback<ValidationsStatusChangedEventArgs> StatusChanged { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Validations"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Indicates if there are any successful validation.
        /// </summary>
        private bool AllValidationsSuccessful
            => validations.All( x => x.Status == ValidationStatus.Success );

        /// <summary>
        /// Indicates if there are any failed validation.
        /// </summary>
        private bool HasFailedValidations
            => validations.Any( x => x.Status == ValidationStatus.Error );

        /// <summary>
        /// Gets the filtered list of failed validations.
        /// </summary>
        private IReadOnlyCollection<string> FailedValidations
        {
            get
            {
                return validations
                    .Where( x => x.Status == ValidationStatus.Error && x.Messages?.Count() > 0 )
                    .SelectMany( x => x.Messages )
                    .Concat(
                        // In case there are some fields that do not have error message we need to combine them all under one message.
                        validations.Any( v => v.Status == ValidationStatus.Error
                            && ( v.Messages == null || v.Messages.Count() == 0 )
                            && !validations.Where( v2 => v2.Status == ValidationStatus.Error && v2.Messages?.Count() > 0 ).Contains( v ) )
                        ? new string[] { MissingFieldsErrorMessage ?? "One or more fields have an error. Please check and try again." }
                        : new string[] { } )
                    .ToList();
            }
        }

        #endregion
    }
}
