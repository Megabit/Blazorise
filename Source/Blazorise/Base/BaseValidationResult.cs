#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            validationStatusChangedHandler += ( sender, eventArgs ) =>
            {
                OnValidationStatusChanged( sender, eventArgs );
                StateHasChanged();
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
