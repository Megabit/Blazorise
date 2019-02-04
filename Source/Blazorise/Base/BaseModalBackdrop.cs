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
    public abstract class BaseModalBackdrop : BaseComponent, ICloseActivator
    {
        #region Members

        private bool isOpen;

        private bool isRegistered;

        #endregion

        #region Methods

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( isRegistered )
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                }
            }

            base.Dispose( disposing );
        }

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

        public bool SafeToClose( string elementId )
        {
            // TODO: ask for parent modal is it OK to close it
            return ElementId == elementId;
        }

        public void Close()
        {
            ParentModal?.Hide();
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

                if ( isOpen )
                {
                    isRegistered = true;

                    JSRunner.RegisterClosableComponent( this );
                }
                else
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                }

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected Modal ParentModal { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
