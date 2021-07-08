﻿#region Using directives
using System.Threading.Tasks;
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
    public partial class BarDropdownToggle : BaseComponent, ICloseActivator
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
            builder.Append( ClassProvider.BarDropdownToggle( ParentDropdownState.Mode ) );

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

                    var task = JSRunner.UnregisterClosableComponent( this );

                    try
                    {
                        await task;
                    }
                    catch when ( task.IsCanceled )
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
            ParentBarDropdown?.Toggle();

            return Clicked.InvokeAsync( null );
        }

        /// <inheritdoc/>
        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
        {
            return Task.FromResult( closeReason == CloseReason.EscapeClosing || ( elementId != ElementId && !isChildClicked ) );
        }

        /// <inheritdoc/>
        public Task Close( CloseReason closeReason )
        {
            ParentBarDropdown?.Hide();

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
