#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseModal : BaseComponent
    {
        #region Members

        private bool isOpen;

        private BaseModalBackdrop modalBackdrop;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Modal() );
            builder.Append( ClassProvider.ModalFade() );
            builder.Append( ClassProvider.ModalShow(), IsOpen );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.ModalShow(), IsOpen );

            base.BuildStyles( builder );
        }

        /// <summary>
        /// Open the modal dialog.
        /// </summary>
        public void Show()
        {
            IsOpen = true;

            StateHasChanged();
        }

        /// <summary>
        /// Close the modal dialog.
        /// </summary>
        public void Hide()
        {
            IsOpen = false;
            Closed.InvokeAsync( null );

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
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.AddClassToBody( "modal-open" );
                } );
            }
            else
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.RemoveClassFromBody( "modal-open" );
                } );
            }

            DirtyClasses();
            DirtyStyles();
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
        public bool IsOpen
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

                    Closed.InvokeAsync( null );
                }
            }
        }

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        [Parameter] public Action<CancelEventArgs> Closing { get; set; }

        /// <summary>
        /// Occurs after the modal has closed.
        /// </summary>
        [Parameter] public EventCallback Closed { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
