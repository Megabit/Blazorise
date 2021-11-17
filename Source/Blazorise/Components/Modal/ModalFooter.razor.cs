#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Bottom area of the modal component.
    /// </summary>
    public partial class ModalFooter : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            ParentModalContent?.NotifyHasModalFooter();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalFooter() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cascaded parent modal-content component.
        /// </summary>
        [CascadingParameter] protected ModalContent ParentModalContent { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="ModalFooter"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
