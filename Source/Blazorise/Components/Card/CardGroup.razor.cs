#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Represent cards as a single, attached component with same width and height columns. Card groups use display: flex; to reach their sizing.
    /// </summary>
    public partial class CardGroup : BaseContainerComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardGroup() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
