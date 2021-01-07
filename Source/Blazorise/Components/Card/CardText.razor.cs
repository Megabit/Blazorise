#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    public partial class CardText : BaseTypographyComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardText() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
