#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseModalContent : BaseComponent
    {
        #region Members

        private bool isForm;

        private bool isCentered;

        private ModalSize modalSize = ModalSize.Default;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.ModalContent( IsForm ) )
                .If( () => ClassProvider.ModalSize( Size ), () => Size != ModalSize.None );

            base.RegisterClasses();
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

                Dirty();
                ClassMapper.Dirty();
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

                Dirty();
                ClassMapper.Dirty();
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

                Dirty();
                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
