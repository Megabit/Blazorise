#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Container for an identical width and height cards that aren't attached to one another.
    /// </summary>
    public partial class CardDeck : BaseContainerComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardDeck() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
