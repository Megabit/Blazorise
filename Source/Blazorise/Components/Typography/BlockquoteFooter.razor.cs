#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Element for identifying the source of the quote.
    /// </summary>
    public partial class BlockquoteFooter : BaseTypographyComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BlockquoteFooter() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
