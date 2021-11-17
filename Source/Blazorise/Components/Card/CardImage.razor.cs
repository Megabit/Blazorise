#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A fullwidth container for a responsive image.
    /// </summary>
    public partial class CardImage : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardImage() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Image url.
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// Alternative image text.
        /// </summary>
        [Parameter] public string Alt { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="CardImage"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
