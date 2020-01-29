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
    public partial class Modal : BaseComponent
    {
        #region Members

        private bool visible;

        public event EventHandler<ModalStateEventArgs> StateChanged;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Modal() );
            builder.Append( ClassProvider.ModalFade() );
            builder.Append( ClassProvider.ModalShow(), Visible );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.ModalShow(), Visible );

            base.BuildStyles( builder );
        }

        /// <summary>
        /// Open the modal dialog.
        /// </summary>
        public void Show()
        {
            Visible = true;

            StateHasChanged();
        }

        /// <summary>
        /// Close the modal dialog.
        /// </summary>
        public void Hide()
        {
            Visible = false;
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

        private void HandleOpenState( bool opened )
        {
            // TODO: find a way to remove javascript
            if ( opened )
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.OpenModal( ElementRef, ElementId );
                } );
            }
            else
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.CloseModal( ElementRef, ElementId );
                } );
            }

            StateChanged?.Invoke( this, new ModalStateEventArgs( opened ) );

            DirtyClasses();
            DirtyStyles();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the visibility of modal dialog.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                // prevent modal from calling the same code multiple times
                if ( value == visible )
                    return;

                if ( value == true )
                {
                    visible = true;

                    HandleOpenState( true );
                }
                else if ( value == false && IsSafeToClose() )
                {
                    visible = false;

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
