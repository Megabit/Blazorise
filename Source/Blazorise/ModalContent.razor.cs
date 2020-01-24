#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class ModalContent : BaseComponent
    {
        #region Members

        private bool isForm;

        private bool isCentered;

        private ModalSize modalSize = ModalSize.Default;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalContent( IsForm ) );
            builder.Append( ClassProvider.ToModalSize( Size ), Size != ModalSize.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Makes the modal as classic dialog with header, body and footer. Used only by bulma https://bulma.io/documentation/components/modal/
        /// </summary>
        [Parameter]
        public bool IsForm
        {
            get => isForm;
            set
            {
                isForm = value;

                DirtyClasses();
                DirtyClasses();
            }
        }

        /// <summary>
        /// Centers the modal vertically.
        /// </summary>
        [Parameter]
        public bool IsCentered
        {
            get => isCentered;
            set
            {
                isCentered = value;

                DirtyClasses();
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
                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
