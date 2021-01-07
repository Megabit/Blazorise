#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    public partial class DropdownDivider : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownDivider() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
