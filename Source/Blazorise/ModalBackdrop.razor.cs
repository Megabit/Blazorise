#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class ModalBackdrop : BaseComponent, ICloseActivator
    {
        #region Members

        private ModalStore parentModalStore;

        private bool jsRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

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
                if ( jsRegistered )
                {
                    jsRegistered = false;

                    if ( Rendered )
                    {
                        _ = JSRunner.UnregisterClosableComponent( this );
                    }
                }

                if ( Rendered )
                {
                    JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
                }
            }

            base.Dispose( disposing );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalBackdrop() );
            builder.Append( ClassProvider.ModalBackdropFade() );
            builder.Append( ClassProvider.ModalBackdropVisible( parentModalStore.Visible ) );

            base.BuildClasses( builder );
        }

        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
        {
            return Task.FromResult( ElementId == elementId );
        }

        public Task Close( CloseReason closeReason )
        {
            ParentModal?.Hide( closeReason );

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        [CascadingParameter]
        protected ModalStore ParentModalStore
        {
            get => parentModalStore;
            set
            {
                if ( parentModalStore == value )
                    return;

                parentModalStore = value;

                if ( parentModalStore.Visible )
                {
                    jsRegistered = true;

                    ExecuteAfterRender( async () => await JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementId ) );
                }
                else
                {
                    jsRegistered = false;

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
