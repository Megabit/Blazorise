#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Placeholder for the default <see cref="Validation"/> state.
    /// </summary>
    public partial class ValidationNone : BaseValidationResult
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ValidationNone() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
