#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An accordion is a vertically stacked list of headers that reveal or hide associated sections of content.
    /// </summary>
    public partial class Accordion : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Accordion() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Accordion"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
