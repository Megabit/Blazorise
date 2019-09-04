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
    public abstract class BaseModalBackdrop : BaseComponent, ICloseActivator, IDisposable
    {
        #region Members

        private bool isOpen;

        private bool isRegistered;

        private DotNetObjectRef<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            // link to the parent component
            ParentModal?.Hook( this );

            // initialize backdrop in case that modal is already set to visible
            if ( ParentModal != null )
                IsOpen = ParentModal.IsOpen;

            base.OnInitialized();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        public void Dispose()
        {
            // make sure to unregister listener
            if ( isRegistered )
            {
                isRegistered = false;

                JSRunner.UnregisterClosableComponent( this );
                JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
            }
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

                Dirty();
            }
        }

        [CascadingParameter] protected BaseModal ParentModal { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
