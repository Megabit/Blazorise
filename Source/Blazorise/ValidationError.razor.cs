#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseValidationError : BaseValidationSummary
    {
        #region Members

        private bool isTooltip;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( !IsTooltip )
                builder.Append( ClassProvider.ValidationError() );
            else
                builder.Append( ClassProvider.ValidationErrorTooltip() );

            base.BuildClasses( builder );
        }

        protected override void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
        {
            if ( eventArgs.Status == ValidationStatus.Error )
            {
                ErrorText = eventArgs.Message;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Custom error type that will override default content.
        /// </summary>
        protected string ErrorText { get; set; }

        /// <summary>
        /// Shows the tooltip instead of label.
        /// </summary>
        [Parameter]
        public bool IsTooltip
        {
            get => isTooltip;
            set
            {
                isTooltip = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
