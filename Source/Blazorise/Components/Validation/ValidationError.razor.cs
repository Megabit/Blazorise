#region Using directives
using System.Linq;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Placeholder for the <see cref="Validation"/> error message.
    /// </summary>
    public partial class ValidationError : BaseValidationResult
    {
        #region Members

        private bool tooltip;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            ErrorText = ParentValidation?.Messages?.Count() > 0
                ? string.Join( ";", ParentValidation?.Messages )
                : null;

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( !Tooltip )
                builder.Append( ClassProvider.ValidationError() );
            else
                builder.Append( ClassProvider.ValidationErrorTooltip() );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected async override void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
        {
            if ( eventArgs.Status == ValidationStatus.Error )
            {
                ErrorText = eventArgs.Messages?.Count() > 0
                    ? string.Join( ";", eventArgs.Messages )
                    : null;
            }
            await InvokeAsync( StateHasChanged );
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
        public bool Tooltip
        {
            get => tooltip;
            set
            {
                tooltip = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
