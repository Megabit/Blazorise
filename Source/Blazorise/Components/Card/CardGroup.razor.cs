#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    public partial class CardGroup : BaseContainerComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardGroup() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
