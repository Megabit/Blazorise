#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An optional header within a card.
    /// </summary>
    public partial class CardHeader : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardHeader() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="CardHeader"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
