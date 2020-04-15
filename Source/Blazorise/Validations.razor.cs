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

        public event ClearAllValidatinaEventHandler ClearingAll;

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
                ValidatedAll.InvokeAsync( null );
            }

            return result;
        }

        /// <summary>
        /// Clears all validation statuses.
        /// </summary>
        public void ClearAll()
        {
            ClearingAll?.Invoke();
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

        internal void NotifyValidationStatusChanged()
        {
            if ( Mode == ValidationMode.Manual )
                return;

            if ( validations.All( x => x.Status == ValidationStatus.Success ) )
            {
                ValidatedAll.InvokeAsync( null );
            }
        }

        #endregion

        #region Properties

        protected EditContext EditContext { get; set; }

        /// <summary>
        /// Defines the validation mode for validations inside of this container.
        /// </summary>
        [Parameter] public ValidationMode Mode { get; set; }

        /// <summary>
        /// Specifies the top-level model object for the form. An edit context will be constructed for this model.
        /// </summary>
        [Parameter] public object Model { get; set; }

        [Parameter] public EventCallback ValidatedAll { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
