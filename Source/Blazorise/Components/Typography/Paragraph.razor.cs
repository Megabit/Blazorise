#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A paragraph always starts on a new line, and is usually a block of text.
    /// </summary>
    public partial class Paragraph : BaseTypographyComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Paragraph() );

            base.BuildClasses( builder );
        }

        #endregion
    }
}
