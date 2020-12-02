#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A classic modal overlay, in which you can include any content you want.
    /// </summary>
    public partial class Modal : BaseComponent
    {
        #region Members

        ModalStore store = new ModalStore
        {
            Visible = false,
        };

        /// <summary>
        /// Holds the last received reason for modal closure.
        /// </summary>
        private CloseReason closeReason = CloseReason.None;

        /// <summary>
        /// A focusable components placed inside of a modal.
        /// </summary>
        /// <remarks>
        /// Only one component can be focused, but the reason why we hold the list
        /// of components is in case we change Autofocus="true" from one component to the other.
        /// And because order of rendering is important, we must make sure that the last component
        /// does NOT set focusableComponent to null.
        /// </remarks>
        private List<IFocusableComponent> focusableComponents;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Modal() );
            builder.Append( ClassProvider.ModalFade() );
            builder.Append( ClassProvider.ModalVisible( Visible ) );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.ModalShow(), Visible );

            base.BuildStyles( builder );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing && Rendered )
            {
                // TODO: implement IAsyncDisposable once it is supported by Blazor!
                //
                // Sometimes user can navigates to another page based on the action runned on modal. The problem is 
                // that for providers like Bootstrap, some classnames can be left behind. So to cover those situation
                // we need to close modal and dispose of any claassnames in case there is any left. 
                _ = JSRunner.CloseModal( ElementRef, ElementId );

                if ( focusableComponents != null )
                {
                    focusableComponents.Clear();
                    focusableComponents = null;
                }
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Opens the modal dialog.
        /// </summary>
        public void Show()
        {
            if ( Visible )
                return;

            Visible = true;

            StateHasChanged();
        }

        /// <summary>
        /// Fires the modal dialog closure process.
        /// </summary>
        public void Hide()
        {
            Hide( CloseReason.UserClosing );
        }

        internal void Hide( CloseReason closeReason )
        {
            if ( !Visible )
                return;

            this.closeReason = closeReason;

            if ( IsSafeToClose() )
            {
                store.Visible = false;

                HandleVisibilityStyles( false );
                RaiseEvents( false );

                // finally reset close reason so it doesn't interfere with internal closing by Visible property
                this.closeReason = CloseReason.None;

                StateHasChanged();
            }
        }

        private bool IsSafeToClose()
        {
            var safeToClose = true;

            var handler = Closing;

            if ( handler != null )
            {
                var args = new ModalClosingEventArgs( false, closeReason );

                foreach ( Action<ModalClosingEventArgs> subHandler in handler?.GetInvocationList() )
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

        private void HandleVisibilityStyles( bool visible )
        {
            if ( visible )
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.OpenModal( ElementRef, ElementId, ScrollToTop );
                } );

                // only one component can be focused
                if ( FocusableComponents.Count > 0 )
                {
                    ExecuteAfterRender( async () =>
                    {
                        await FocusableComponents.First().FocusAsync();
                    } );
                }
            }
            else
            {
                ExecuteAfterRender( async () =>
                {
                    await JSRunner.CloseModal( ElementRef, ElementId );
                } );
            }

            DirtyClasses();
            DirtyStyles();
        }

        private void RaiseEvents( bool visible )
        {
            if ( !visible )
            {
                Closed.InvokeAsync( null );
            }
        }

        public void AddFocusableComponent( IFocusableComponent focusableComponent )
        {
            if ( focusableComponent == null )
                return;

            if ( !FocusableComponents.Contains( focusableComponent ) )
            {
                FocusableComponents.Add( focusableComponent );
            }
        }

        public void RemoveFocusableComponent( IFocusableComponent focusableComponent )
        {
            if ( focusableComponent == null )
                return;

            if ( FocusableComponents.Contains( focusableComponent ) )
            {
                FocusableComponents.Remove( focusableComponent );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of focusable components.
        /// </summary>
        protected IList<IFocusableComponent> FocusableComponents
            => focusableComponents ??= new List<IFocusableComponent>();

        /// <summary>
        /// Defines the visibility of modal dialog.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => store.Visible;
            set
            {
                // prevent modal from calling the same code multiple times
                if ( value == store.Visible )
                    return;

                if ( value == true )
                {
                    store.Visible = true;

                    HandleVisibilityStyles( true );
                    RaiseEvents( true );
                }
                else if ( value == false && IsSafeToClose() )
                {
                    store.Visible = false;

                    HandleVisibilityStyles( false );
                    RaiseEvents( false );
                }
            }
        }

        /// <summary>
        /// If true modal will scroll to top when opened.
        /// </summary>
        [Parameter] public bool ScrollToTop { get; set; } = true;

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        [Parameter] public Action<ModalClosingEventArgs> Closing { get; set; }

        /// <summary>
        /// Occurs after the modal has closed.
        /// </summary>
        [Parameter] public EventCallback Closed { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Modal"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
