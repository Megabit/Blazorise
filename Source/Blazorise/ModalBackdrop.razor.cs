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
    public partial class ModalBackdrop : BaseComponent, ICloseActivator
    {
        #region Members

        private bool visible;

        private bool isRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentModal != null )
            {
                // initialize backdrop in case that modal is already set to visible
                Visible = ParentModal.Visible;

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
                if ( ParentModal != null )
                {
                    ParentModal.StateChanged -= OnModalStateChanged;
                }

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
            builder.Append( ClassProvider.ModalShow(), Visible );

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
            Visible = e.Visible;
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
        public bool Visible
        {
            get => visible;
            set
            {
                if ( value == visible )
                    return;

                visible = value;

                if ( visible )
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

        [CascadingParameter] protected Modal ParentModal { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
