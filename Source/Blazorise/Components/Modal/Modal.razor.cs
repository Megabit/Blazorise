﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            builder.Append( ClassProvider.ModalVisible( Visible ) );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( StyleProvider.ModalShow(), Visible );

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

                // TODO: implement IAsyncDisposable once it is supported by Blazor!
                //
                // Sometimes user can navigates to another page based on the action runned on modal. The problem is
                // that for providers like Bootstrap, some classnames can be left behind. So to cover those situation
                // we need to close modal and dispose of any claassnames in case there is any left.
                var closeModalTask = JSRunner.CloseModal( ElementRef );

                try
                {
                    await closeModalTask;
                }
                catch
                {
                    if ( !closeModalTask.IsCanceled )
                    {
                        throw;
                    }
                }

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
        public void Show()
        {
            if ( Visible )
                return;

            Visible = true;

            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Fires the modal dialog closure process.
        /// </summary>
        public void Hide()
        {
            Hide( CloseReason.UserClosing );
        }

        /// <summary>
        /// Internal method to hide the modal with reason of closing.
        /// </summary>
        /// <param name="closeReason">Reason why modal was closed.</param>
        internal protected void Hide( CloseReason closeReason )
        {
            if ( !Visible )
                return;

            this.closeReason = closeReason;

            if ( IsSafeToClose() )
            {
                state = state with { Visible = false };

                HandleVisibilityStyles( false );
                RaiseEvents( false );

                // finally reset close reason so it doesn't interfere with internal closing by Visible property
                this.closeReason = CloseReason.None;

                InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Determines if modal can be closed.
        /// </summary>
        /// <returns>True if modal can be closed.</returns>
        private bool IsSafeToClose()
        {
            var safeToClose = true;

            var handler = Closing;

            if ( handler != null )
            {
                var args = new ModalClosingEventArgs( false, closeReason );

                foreach ( Action<ModalClosingEventArgs> subHandler in handler?.GetInvocationList() )
                {
                    subHandler( args );

                    if ( args.Cancel )
                    {
                        safeToClose = false;
                    }
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
                    await JSRunner.OpenModal( ElementRef, ScrollToTop );

                    await JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementRef );
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
                            return firstFocusableComponent.FocusAsync();
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
                    await JSRunner.CloseModal( ElementRef );

                    await JSRunner.UnregisterClosableComponent( this );
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
            if ( !visible )
            {
                Closed.InvokeAsync( null );
            }

            VisibleChanged.InvokeAsync( visible );
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
            Hide( closeReason );

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Gets the reference to state object for this modal.
        /// </summary>
        protected ModalState State => state;

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
        /// Defines the visibility of modal dialog.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => state.Visible;
            set
            {
                // prevent modal from calling the same code multiple times
                if ( value == state.Visible )
                    return;

                if ( value == true )
                {
                    state = state with { Visible = true };

                    HandleVisibilityStyles( true );
                    RaiseEvents( true );
                }
                else if ( value == false && IsSafeToClose() )
                {
                    state = state with { Visible = false };

                    HandleVisibilityStyles( false );
                    RaiseEvents( false );
                }
            }
        }

        /// <summary>
        /// Occurs when the modal visibility state changes.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// If true modal will scroll to top when opened.
        /// </summary>
        [Parameter] public bool ScrollToTop { get; set; } = true;

        /// <summary>
        /// Occurs before the modal is closed.
        /// </summary>
        [Parameter] public Action<ModalClosingEventArgs> Closing { get; set; }

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
