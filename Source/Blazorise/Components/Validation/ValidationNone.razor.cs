#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    public partial class ValidationNone : BaseValidationResult
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ValidationNone() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
