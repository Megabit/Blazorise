#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class ModalBackdrop : BaseComponent, ICloseActivator
    {
        #region Members

        private ModalState parentModalState;

        private bool jsRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

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
                    DisposeDotNetObjectRef( dotNetObjectRef );
                }
            }

            base.Dispose( disposing );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalBackdrop() );
            builder.Append( ClassProvider.ModalBackdropFade() );
            builder.Append( ClassProvider.ModalBackdropVisible( parentModalState.Visible ) );

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

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        [CascadingParameter]
        protected ModalState ParentModalState
        {
            get => parentModalState;
            set
            {
                if ( parentModalState == value )
                    return;

                parentModalState = value;

                if ( parentModalState.Visible )
                {
                    jsRegistered = true;

                    ExecuteAfterRender( async () => await JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementRef ) );
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
