#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The left side of the media content.
    /// </summary>
    public partial class MediaLeft : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.MediaLeft() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="MediaLeft"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
