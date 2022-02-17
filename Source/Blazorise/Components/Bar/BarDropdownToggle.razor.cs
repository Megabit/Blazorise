#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Toggles the visibility or collapse of <see cref="Bar"/> component.
    /// </summary>
    public partial class BarDropdownToggle : BaseComponent, ICloseActivator, IAsyncDisposable
    {
        #region Members

        private BarDropdownState parentDropdownState;

        private bool jsRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            return base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownToggle( ParentDropdownState.Mode, ParentBarDropdown?.IsBarDropdownSubmenu == true ) );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void BuildStyles( StyleBuilder builder )
        {
            base.BuildStyles( builder );

            builder.Append( $"padding-left: { Indentation * ParentDropdownState.NestedIndex }rem", ParentDropdownState.IsInlineDisplay );
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
                // make sure to unregister listener
                if ( jsRegistered )
                {
                    jsRegistered = false;

                    var task = JSClosableModule.Unregister( this );

                    try
                    {
                        await task;
                    }
                    catch when ( task.IsCanceled )
                    {
                    }
                    catch ( Microsoft.JSInterop.JSDisconnectedException )
                    {
                    }
                }

                DisposeDotNetObjectRef( dotNetObjectRef );
                dotNetObjectRef = null;
            }

            await base.DisposeAsync( disposing );
        }

        /// <summary>
        /// Handles the button click event.
        /// </summary>
        /// <returns>Returns the awaitable task.</returns>
        protected Task ClickHandler()
        {

            if ( ParentBarDropdown != null )
                return ParentBarDropdown.Toggle( ElementId );

            return Clicked.InvokeAsync();
        }

        /// <inheritdoc/>
        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
        {
            return Task.FromResult( closeReason == CloseReason.EscapeClosing || ( ParentBarDropdown?.ShouldClose ?? true && ( elementId != ElementId && ParentBarDropdown?.SelectedBarDropdownElementId != ElementId && !isChildClicked ) ) );
        }

        /// <inheritdoc/>
        public Task Close( CloseReason closeReason )
        {
            if ( ParentBarDropdown != null )
                return ParentBarDropdown.Hide();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the visibility styles and JS interop states.
        /// </summary>
        /// <param name="visible">True if component is visible.</param>
        protected virtual void HandleVisibilityStyles( bool visible )
        {
            if ( visible )
            {
                jsRegistered = true;

                ExecuteAfterRender( async () =>
                {
                    await JSClosableModule.Register( dotNetObjectRef, ElementRef );
                } );
            }
            else
            {
                jsRegistered = false;

                ExecuteAfterRender( async () =>
                {
                    await JSClosableModule.Unregister( this );
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
        /// Gets or sets the <see cref="IJSClosableModule"/> instance.
        /// </summary>
        [Inject] public IJSClosableModule JSClosableModule { get; set; }

        /// <summary>
        /// Determines how much left padding will be applied to the dropdown toggle. (in rem unit)
        /// </summary>
        [Parameter] public double Indentation { get; set; } = 1.5d;

        /// <summary>
        /// Occurs when the toggle button is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Gets or sets the parent dropdown state object.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reference to the parent dropdown.
        /// </summary>
        [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BarDropdownToggle"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
