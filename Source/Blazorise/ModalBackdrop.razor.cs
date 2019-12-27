#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public abstract class BaseModalBackdrop : BaseComponent, ICloseActivator
    {
        #region Members

        private bool isOpen;

        private bool isRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentModal != null )
            {
                // initialize backdrop in case that modal is already set to visible
                IsOpen = ParentModal.IsOpen;

                ParentModal.StateChanged += OnModalStateChanged;
            }

            base.OnInitialized();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( isRegistered )
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                    JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
                }
            }

            base.Dispose( disposing );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalBackdrop() );
            builder.Append( ClassProvider.ModalFade() );
            builder.Append( ClassProvider.ModalShow(), IsOpen );

            base.BuildClasses( builder );
        }

        public bool SafeToClose( string elementId, bool isEscapeKey )
        {
            // TODO: ask for parent modal is it OK to close it
            return ElementId == elementId;
        }

        public void Close()
        {
            ParentModal?.Hide();
        }

        private void OnModalStateChanged( object sender, ModalStateEventArgs e )
        {
            IsOpen = e.Opened;
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
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                if ( value == isOpen )
                    return;

                isOpen = value;

                if ( isOpen )
                {
                    isRegistered = true;

                    ExecuteAfterRender( async () => await JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementId ) );
                }
                else
                {
                    isRegistered = false;

                    ExecuteAfterRender( async () => await JSRunner.UnregisterClosableComponent( this ) );
                }

                DirtyClasses();
            }
        }

        [CascadingParameter] public BaseModal ParentModal { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
