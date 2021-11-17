#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper for all text inside of <see cref="Card"/> component.
    /// </summary>
    public partial class CardText : BaseTypographyComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardText() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
