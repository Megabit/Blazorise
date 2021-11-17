#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper for a card links.
    /// </summary>
    public partial class CardLink : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardLink() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Link url.
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// Alternative link text.
        /// </summary>
        [Parameter] public string Alt { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="CardLink"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
