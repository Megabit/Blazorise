#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseModal : BaseComponent
    {
        #region Members

        private bool isOpen;

        private BaseModalBackdrop modalBackdrop;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Modal() )
                .Add( () => ClassProvider.ModalFade() )
                .If( () => ClassProvider.ModalShow(), () => IsOpen );

            base.RegisterClasses();
        }

        protected override void RegisterStyles()
        {
            StyleMapper
                .If( () => StyleProvider.ModalShow(), () => IsOpen );

            base.RegisterStyles();
        }

        public void Show()
        {
            IsOpen = true;

            StateHasChanged();
        }

        public void Hide()
        {
            IsOpen = false;

            StateHasChanged();
        }

        private bool IsSafeToClose()
        {
            var safeToClose = true;

            var handler = Closing;

            if ( handler != null )
            {
                var args = new CancelEventArgs( false );

                foreach ( Action<CancelEventArgs> subHandler in handler?.GetInvocationList() )
                {
                    subHandler( args );

                    if ( args.Cancel )
                    {
                        safeToClose = false;
                    }
                }
            }

            return safeToClose;
        }

        private void HandleOpenState( bool isOpen )
        {
            if ( modalBackdrop != null )
                modalBackdrop.IsOpen = isOpen;

            // TODO: find a way to remove javascript
            if ( isOpen )
                JSRunner.AddClassToBody( "modal-open" );
            else
                JSRunner.RemoveClassFromBody( "modal-open" );

            ClassMapper.Dirty();
            StyleMapper.Dirty();
        }

        internal void Hook( BaseModalBackdrop modalBackdrop )
        {
            this.modalBackdrop = modalBackdrop;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the visibility of modal dialog.
        /// </summary>
        [Parameter]
        internal bool IsOpen
        {
            get => isOpen;
            set
            {
                if ( value == true )
                {
                    isOpen = true;

                    HandleOpenState( true );
                }
                else if ( value == false && IsSafeToClose() )
                {
                    isOpen = false;

                    HandleOpenState( false );
                }
            }
        }

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        [Parameter] protected Action<CancelEventArgs> Closing { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
