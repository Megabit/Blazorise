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

        private bool jsRegistered;

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

            builder.Append( $"padding-left: { Indentation * ParentDropdownState.NestedIndex }rem", ParentDropdownState.IsInlineDisplay );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing && Rendered )
            {
                // make sure to unregister listener
                if ( jsRegistered )
                {
                    jsRegistered = false;

                    _ = JSRunner.UnregisterClosableComponent( this );
                }

                DisposeDotNetObjectRef( dotNetObjectRef );
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

        protected virtual void HandleVisibilityStyles( bool visible )
        {
            if ( visible )
            {
                jsRegistered = true;

                ExecuteAfterRender( async () =>
                {
                    await JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementRef );
                } );
            }
            else
            {
                jsRegistered = false;

                ExecuteAfterRender( async () =>
                {
                    await JSRunner.UnregisterClosableComponent( this );
                } );
            }

            DirtyClasses();
            DirtyStyles();
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Determines how much left padding will be applied to the dropdown toggle. (in rem unit)
        /// </summary>
        [Parameter] public double Indentation { get; set; } = 1.5d;

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
                    HandleVisibilityStyles( true );
                }
                else
                {
                    HandleVisibilityStyles( false );
                }
            }
        }

        [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
