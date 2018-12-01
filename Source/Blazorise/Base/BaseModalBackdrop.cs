#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseModalBackdrop : BaseComponent
    {
        #region Members

        private bool isOpen;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.ModalBackdrop() )
                .Add( () => ClassProvider.ModalFade() )
                .If( () => ClassProvider.ModalShow(), () => IsOpen );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            ParentModal?.Hook( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the visibility of modal backdrop.
        /// </summary>
        /// <remarks>
        /// Use this only when backdrop is placed outside of modal.
        /// </remarks>
        [Parameter]
        internal bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected Modal ParentModal { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
