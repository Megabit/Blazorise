#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class ModalContent : BaseComponent
    {
        #region Members

        private bool dialog;

        private bool centered;

        private ModalSize modalSize = ModalSize.Default;

        #endregion

        #region Methods

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

        [CascadingParameter] protected Modal ParentModal { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
