#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Placeholder for the <see cref="Validation"/> success message.
    /// </summary>
    public partial class ValidationSuccess : BaseValidationResult
    {
        #region Members

        private bool tooltip;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( !Tooltip )
                builder.Append( ClassProvider.ValidationSuccess() );
            else
                builder.Append( ClassProvider.ValidationSuccessTooltip() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

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
