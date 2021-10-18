﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// A classic modal overlay, in which you can include any content you want.
    /// </summary>
    public partial class Modal : BaseComponent, ICloseActivator
    {
        #region Members

        /// <summary>
        /// Holds the state of this modal dialog.
        /// </summary>
        private ModalState state = new()
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

        /// <summary>
        /// Tells us that modal is tracked by the JS interop.
        /// </summary>
        private bool jsRegistered;

        /// <summary>
        /// A JS interop object reference used to access this modal.
        /// </summary>
        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        /// <summary>
        /// A list of all elements id that could potentially trigger the modal close event.
        /// </summary>
        private readonly List<string> closeActivatorElementIds = new();

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            if ( parameters.TryGetValue<bool>( nameof( Visible ), out var visibleResult ) && state.Visible != visibleResult )
            {
                if ( visibleResult == true && await IsSafeToOpen() )
                {
                    SetVisibleState( true );
                }
                else if ( visibleResult == false && await IsSafeToClose() )
                {
                    SetVisibleState( false );
                }
                else
                {
                    // skip execution
                    return;
                }
            }

            await base.SetParametersAsync( parameters );
        }

        /// <inheritdoc/>
        protected override Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            return base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Modal() );
            builder.Append( ClassProvider.ModalFade() );
            builder.Append( ClassProvider.ModalVisible( IsVisible ) );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.ModalShow(), IsVisible );

            base.BuildStyles( builder );
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

                    var unregisterClosableTask = JSClosableModule.Unregister( this );

                    try
                    {
                        await unregisterClosableTask;
                    }
                    catch when ( unregisterClosableTask.IsCanceled )
                    {
                    }
#if NET6_0_OR_GREATER
                    catch ( Microsoft.JSInterop.JSDisconnectedException )
                    {
                    }
#endif
                }

                DisposeDotNetObjectRef( dotNetObjectRef );
                dotNetObjectRef = null;

                // TODO: implement IAsyncDisposable once it is supported by Blazor!
                //
                // Sometimes user can navigates to another page based on the action runned on modal. The problem is
                // that for providers like Bootstrap, some classnames can be left behind. So to cover those situation
                // we need to close modal and dispose of any claassnames in case there is any left.
                var closeModalTask = JSModalModule.CloseModal( ElementRef );

                try
                {
                    await closeModalTask;
                }
                catch when ( closeModalTask.IsCanceled )
                {
                }
#if NET6_0_OR_GREATER
                catch ( Microsoft.JSInterop.JSDisconnectedException )
                {
                }
#endif

                if ( focusableComponents != null )
                {
                    focusableComponents.Clear();
                    focusableComponents = null;
                }
            }

            await base.DisposeAsync( disposing );
        }

        /// <summary>
        /// Opens the modal dialog.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Show()
        {
            if ( state.Visible )
                return;

            if ( await IsSafeToOpen() )
            {
                SetVisibleState( true );

                await InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Fires the modal dialog closure process.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Hide()
        {
            return Hide( CloseReason.UserClosing );
        }

        /// <summary>
        /// Internal method to hide the modal with reason of closing.
        /// </summary>
        /// <param name="closeReason">Reason why modal was closed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal protected async Task Hide( CloseReason closeReason )
        {
            if ( !state.Visible )
                return;

            this.closeReason = closeReason;

            if ( await IsSafeToClose() )
            {
                SetVisibleState( false );

                // finally reset close reason so it doesn't interfere with internal closing by Visible property
                this.closeReason = CloseReason.None;

                await InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Determines if modal can be opened.
        /// </summary>
        /// <returns>True if modal can be opened.</returns>
        private async Task<bool> IsSafeToOpen()
        {
            var safeToOpen = true;

            if ( Opening != null )
            {
                var eventArgs = new ModalOpeningEventArgs( false );

                await Opening.Invoke( eventArgs );

                if ( eventArgs.Cancel )
                {
                    safeToOpen = false;
                }
            }

            return safeToOpen;
        }

        /// <summary>
        /// Determines if modal can be closed.
        /// </summary>
        /// <returns>True if modal can be closed.</returns>
        private async Task<bool> IsSafeToClose()
        {
            var safeToClose = true;

            if ( Closing != null )
            {
                var eventArgs = new ModalClosingEventArgs( false, closeReason );

                await Closing.Invoke( eventArgs );

                if ( eventArgs.Cancel )
                {
                    safeToClose = false;
                }
            }

            return safeToClose;
        }

        /// <summary>
        /// Handles the styles based on the visibility flag.
        /// </summary>
        /// <param name="visible">Modal visibility flag.</param>
        protected virtual void HandleVisibilityStyles( bool visible )
        {
            if ( visible )
            {
                jsRegistered = true;

                ExecuteAfterRender( async () =>
                {
                    await JSModalModule.OpenModal( ElementRef, ScrollToTop );

                    await JSClosableModule.Register( dotNetObjectRef, ElementRef );
                } );

                // only one component can be focused
                if ( FocusableComponents.Count > 0 )
                {
                    ExecuteAfterRender( () =>
                    {
                        //TODO: This warrants further investigation
                        //Even with the Count>0 check above, sometimes FocusableComponents.First() fails intermittently with an InvalidOperationException: Sequence contains no elements
                        //This null check helps prevent the application from crashing unexpectedly, until a more indepth solution can be found.
                        var firstFocusableComponent = FocusableComponents.FirstOrDefault();

                        if ( firstFocusableComponent != null )
                        {
                            return firstFocusableComponent.Focus();
                        }

                        return Task.CompletedTask;
                    } );
                }
            }
            else
            {
                jsRegistered = false;

                ExecuteAfterRender( async () =>
                {
                    await JSModalModule.CloseModal( ElementRef );

                    await JSClosableModule.Unregister( this );
                } );
            }

            DirtyClasses();
            DirtyStyles();
        }

        /// <summary>
        /// Fires all the events for this modal
        /// </summary>
        /// <param name="visible"></param>
        protected virtual void RaiseEvents( bool visible )
        {
            if ( visible )
            {
                Opened.InvokeAsync();
            }
            else
            {
                Closed.InvokeAsync();
            }

            InvokeAsync( () => VisibleChanged.InvokeAsync( visible ) );
        }

        internal void NotifyFocusableComponentInitialized( IFocusableComponent focusableComponent )
        {
            if ( focusableComponent == null )
                return;

            if ( !FocusableComponents.Contains( focusableComponent ) )
            {
                FocusableComponents.Add( focusableComponent );
            }
        }

        internal void NotifyFocusableComponentRemoved( IFocusableComponent focusableComponent )
        {
            if ( focusableComponent == null )
                return;

            if ( FocusableComponents.Contains( focusableComponent ) )
            {
                FocusableComponents.Remove( focusableComponent );
            }
        }

        /// <summary>
        /// Registers a new element that can close the modal.
        /// </summary>
        /// <param name="elementId">Element id.</param>
        internal void NotifyCloseActivatorIdInitialized( string elementId )
        {
            if ( !closeActivatorElementIds.Contains( elementId ) )
                closeActivatorElementIds.Add( elementId );
        }

        /// <summary>
        /// Removes the element that can close the modal.
        /// </summary>
        /// <param name="elementId">Element id.</param>
        internal void NotifyCloseActivatorIdRemoved( string elementId )
        {
            if ( closeActivatorElementIds.Contains( elementId ) )
                closeActivatorElementIds.Remove( elementId );
        }

        /// <inheritdoc/>
        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
        {
            return Task.FromResult( ElementId == elementId || closeActivatorElementIds.Contains( elementId ) );
        }

        /// <inheritdoc/>
        public Task Close( CloseReason closeReason )
        {
            return Hide( closeReason );
        }

        /// <summary>
        /// Handles the internal visibility states.
        /// </summary>
        /// <param name="visible">Visible state.</param>
        private void SetVisibleState( bool visible )
        {
            state = state with { Visible = visible };

            HandleVisibilityStyles( visible );
            RaiseEvents( visible );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the modal should be visible.
        /// </summary>
        protected internal bool IsVisible => State.Visible == true;

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Gets the reference to state object for this modal.
        /// </summary>
        protected internal ModalState State => state;

        /// <summary>
        /// Gets the list of focusable components.
        /// </summary>
        protected IList<IFocusableComponent> FocusableComponents
            => focusableComponents ??= new();

        /// <summary>
        /// Gets the list of all element ids that could trigger modal close event.
        /// </summary>
        public IEnumerable<string> CloseActivatorElementIds
            => closeActivatorElementIds;

        /// <summary>
        /// Gets or sets the <see cref="IJSModalModule"/> instance.
        /// </summary>
        [Inject] public IJSModalModule JSModalModule { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IJSClosableModule"/> instance.
        /// </summary>
        [Inject] public IJSClosableModule JSClosableModule { get; set; }

        /// <summary>
        /// Defines the visibility of modal dialog.
        /// </summary>
        /// <remarks>The <see cref="Visible"/> parameter should only be used in .razor code.</remarks>
        [Parameter] public bool Visible { get; set; }

        /// <summary>
        /// Occurs when the modal visibility state changes.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// If true modal will scroll to top when opened.
        /// </summary>
        [Parameter] public bool ScrollToTop { get; set; } = true;

        /// <summary>
        /// Occurs before the modal is opened.
        /// </summary>
        [Parameter] public Func<ModalOpeningEventArgs, Task> Opening { get; set; }

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        [Parameter] public Func<ModalClosingEventArgs, Task> Closing { get; set; }

        /// <summary>
        /// Occurs after the modal has opened.
        /// </summary>
        [Parameter] public EventCallback Opened { get; set; }

        /// <summary>
        /// Occurs after the modal has closed.
        /// </summary>
        [Parameter] public EventCallback Closed { get; set; }

        /// <summary>
        /// Specifies the backdrop needs to be rendered for this <see cref="Modal"/>.
        /// </summary>
        [Parameter] public bool ShowBackdrop { get; set; } = true;

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Modal"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
