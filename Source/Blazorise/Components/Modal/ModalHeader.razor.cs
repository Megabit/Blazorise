#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Top area of the modal component.
    /// </summary>
    public partial class ModalHeader : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            ParentModalContent?.NotifyHasModalHeader();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalHeader() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cascaded parent modal-content component.
        /// </summary>
        [CascadingParameter] protected ModalContent ParentModalContent { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="ModalHeader"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
