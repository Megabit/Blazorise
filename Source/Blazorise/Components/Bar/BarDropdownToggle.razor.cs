#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class BarDropdownToggle : BaseComponent, ICloseActivator
    {
        #region Members

        private BarDropdownState parentDropdownState;

        private bool isRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownToggle( ParentDropdownState.Mode ) );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            base.BuildStyles( builder );

            builder.Append( $"padding-left: { 1.5d * ParentDropdownState.NestedIndex }rem", ParentDropdownState.IsInlineDisplay );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( isRegistered )
                {
                    isRegistered = false;

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

        protected Task ClickHandler()
        {
            ParentBarDropdown?.Toggle();

            return Task.CompletedTask;
        }

        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
        {
            return Task.FromResult( closeReason == CloseReason.EscapeClosing || ( elementId != ElementId && !isChildClicked ) );
        }

        public Task Close( CloseReason closeReason )
        {
            ParentBarDropdown?.Hide();

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        [CascadingParameter]
        public BarDropdownState ParentDropdownState
        {
            get => parentDropdownState;
            set
            {
                if ( parentDropdownState == value )
                    return;

                parentDropdownState = value;

                if ( parentDropdownState.Visible && !( parentDropdownState.Mode == BarMode.VerticalInline && parentDropdownState.BarVisible ) )
                {
                    isRegistered = true;

                    JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementRef );
                }
                else
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                }

                DirtyClasses();
                DirtyStyles();
            }
        }

        [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
