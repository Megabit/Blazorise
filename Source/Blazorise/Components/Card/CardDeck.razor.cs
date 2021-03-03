#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    public partial class CardDeck : BaseContainerComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardDeck() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
