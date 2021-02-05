#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseValidationResult : BaseComponent
    {
        #region Members

        private Validation previousParentValidation;

        private readonly EventHandler<ValidationStatusChangedEventArgs> validationStatusChangedHandler;

        #endregion

        #region Methods

        public BaseValidationResult()
        {
            validationStatusChangedHandler += async ( sender, eventArgs ) =>
            {
                OnValidationStatusChanged( sender, eventArgs );
                await InvokeAsync( StateHasChanged );
            };
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DetachValidationStatusChangedListener();
            }

            base.Dispose( disposing );
        }

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

        protected virtual void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
        {
        }

        #endregion

        #region Properties

        [CascadingParameter] protected Validation ParentValidation { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
