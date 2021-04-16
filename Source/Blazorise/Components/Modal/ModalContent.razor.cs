#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Main wrapper for the content area of the modal component.
    /// </summary>
    public partial class ModalContent : BaseComponent
    {
        #region Members

        private bool dialog;

        private bool centered;

        private ModalSize modalSize = ModalSize.Default;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalContent( Dialog ) );
            builder.Append( ClassProvider.ModalContentSize( Size ), Size != ModalSize.Default );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Makes the modal as classic dialog with header, body and footer. Used only by bulma https://bulma.io/documentation/components/modal/
        /// </summary>
        [Parameter]
        public bool Dialog
        {
            get => dialog;
            set
            {
                dialog = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Centers the modal vertically.
        /// </summary>
        [Parameter]
        public bool Centered
        {
            get => centered;
            set
            {
                centered = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Changes the size of the modal.
        /// </summary>
        [Parameter]
        public virtual ModalSize Size
        {
            get => modalSize;
            set
            {
                modalSize = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="ModalContent"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Modal"/> component.
        /// </summary>
        [CascadingParameter] protected Modal ParentModal { get; set; }

        #endregion
    }
}
