#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The main block of a <see cref="Card"/>.
    /// </summary>
    public partial class CardBody : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CardBody() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="CardBody"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
