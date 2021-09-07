#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// For quoting blocks of content from another source within your document.
    /// </summary>
    public partial class Blockquote : BaseTypographyComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Blockquote() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
