﻿#region Using directives
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

        private bool centered;

        private bool scrollable;

        private ModalSize modalSize = ModalSize.Default;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalContent( AsDialog ) );
            builder.Append( ClassProvider.ModalContentSize( Size ), Size != ModalSize.Default );
            builder.Append( ClassProvider.ModalContentCentered( Centered ) );
            builder.Append( ClassProvider.ModalContentScrollable( Scrollable ) );

            base.BuildClasses( builder );
        }

        internal void NotifyHasModalHeader()
        {
            HasModalHeader = true;
        }

        internal void NotifyHasModalBody()
        {
            HasModalBody = true;
        }

        internal void NotifyHasModalFooter()
        {
            HasModalFooter = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if modal contains the <see cref="ModalHeader"/> component.
        /// </summary>
        protected bool HasModalHeader { get; set; }

        /// <summary>
        /// True if modal contains the <see cref="ModalBody"/> component.
        /// </summary>
        protected bool HasModalBody { get; set; }

        /// <summary>
        /// True if modal contains the <see cref="ModalFooter"/> component.
        /// </summary>
        protected bool HasModalFooter { get; set; }

        /// <summary>
        /// Makes the modal as classic dialog with header, body and footer.
        /// 
        /// Currently used only by bulma https://bulma.io/documentation/components/modal/
        /// </summary>
        protected virtual bool AsDialog => false;

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
        /// Scrolls the modal content independent of the page itself.
        /// </summary>
        [Parameter]
        public bool Scrollable
        {
            get => scrollable;
            set
            {
                scrollable = value;

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
