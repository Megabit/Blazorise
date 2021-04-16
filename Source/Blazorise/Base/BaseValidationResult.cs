#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base class for validation result messages.
    /// </summary>
    public abstract class BaseValidationResult : BaseComponent
    {
        #region Members

        private Validation previousParentValidation;

        private readonly EventHandler<ValidationStatusChangedEventArgs> validationStatusChangedHandler;

        #endregion

        #region Constructors

        /// <summary>
        /// A default constructors for <see cref="BaseValidationResult"/>.
        /// </summary>
        public BaseValidationResult()
        {
            validationStatusChangedHandler += async ( sender, eventArgs ) =>
            {
                OnValidationStatusChanged( sender, eventArgs );
                await InvokeAsync( StateHasChanged );
            };
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DetachValidationStatusChangedListener();
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override void OnParametersSet()
        {
            if ( ParentValidation != previousParentValidation )
            {
                DetachValidationStatusChangedListener();
                ParentValidation.ValidationStatusChanged += validationStatusChangedHandler;
                previousParentValidation = ParentValidation;
            }
        }

        private void DetachValidationStatusChangedListener()
        {
            if ( previousParentValidation != null )
            {
                previousParentValidation.ValidationStatusChanged -= validationStatusChangedHandler;
            }
        }

        /// <inheritdoc/>
        protected virtual void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the reference to the parent validation.
        /// </summary>
        [CascadingParameter] protected Validation ParentValidation { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BaseValidationResult"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
