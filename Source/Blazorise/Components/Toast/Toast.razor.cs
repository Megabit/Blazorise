﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// A Toast displays temporary content to the user.
/// </summary>
public partial class Toast : BaseComponent, ICloseActivator, IAnimatedComponent, IAsyncDisposable
{
    #region Members

    ///<summary>
    /// Holds the state of the Toast component.
    ///</summary>
    private ToastState state = new()
    {
        Visible = false,
    };

    ///<summary>
    /// Holds the reason for the Toast closing.
    ///</summary>
    private CloseReason closeReason = CloseReason.None;

    ///<summary>
    /// Indicates whether the Toast has been registered with JavaScript.
    ///</summary>
    private bool jsRegistered;

    ///<summary>
    /// Reference to the .NET object adapter for the CloseActivator.
    ///</summary>
    private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

    ///<summary>
    /// List of element IDs that act as close activators for the Toast.
    ///</summary>
    private readonly List<string> closeActivatorElementIds = new();

    ///<summary>
    /// Adapter for the Closeable interface to handle close events.
    ///</summary>
    private CloseableAdapter closeableAdapter;

    ///<summary>
    /// Event that is triggered when the Toast is opened.
    ///</summary>
    internal event Action _Opened;

    ///<summary>
    /// Event that is triggered when the Toast is closed.
    ///</summary>
    internal event Action _Closed;

    #endregion

    #region Constructors

    ///<summary>
    /// Initializes a new instance of the <see cref="Toast"/> class.
    ///</summary>
    public Toast()
    {
        closeableAdapter = new( this );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<bool>( nameof( Visible ), out var paramVisible ) && state.Visible != paramVisible )
        {
            if ( paramVisible && await IsSafeToOpen() )
            {
                await base.SetParametersAsync( parameters );
                await SetVisibleState( true );
            }
            else if ( !paramVisible && await IsSafeToClose() )
            {
                await base.SetParametersAsync( parameters );
                await SetVisibleState( false );
            }
        }
        else
        {
            await base.SetParametersAsync( parameters );
        }
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
        builder.Append( ClassProvider.Toast() );
        builder.Append( ClassProvider.ToastFade( Animated && State.Showing, Animated && State.Hiding ) );
        builder.Append( ClassProvider.ToastVisible( IsVisible ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( StyleProvider.ToastAnimationDuration( Animated, AnimationDuration ) );
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
    /// Starts the toast opening process.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Show()
    {
        if ( state.Visible )
            return;

        if ( await IsSafeToOpen() )
        {
            await SetVisibleState( true );

            if ( !Animated )
            {
                DirtyClasses();
                DirtyStyles();
            }

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Fires the toast dialog closure process.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Hide()
    {
        return Hide( CloseReason.UserClosing );
    }

    /// <summary>
    /// Internal method to hide the toast with reason of closing.
    /// </summary>
    /// <param name="closeReason">Reason why toast was closed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal protected async Task Hide( CloseReason closeReason )
    {
        if ( !state.Visible )
            return;

        this.closeReason = closeReason;

        if ( await IsSafeToClose() )
        {
            await SetVisibleState( false );

            if ( !Animated )
            {
                DirtyClasses();
                DirtyStyles();
            }

            this.closeReason = CloseReason.None;

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Determines if toast can be opened.
    /// </summary>
    /// <returns>True if toast can be opened.</returns>
    private async Task<bool> IsSafeToOpen()
    {
        var safeToOpen = true;

        if ( Opening is not null )
        {
            var eventArgs = new ToastOpeningEventArgs( false );

            await Opening.Invoke( eventArgs );

            if ( eventArgs.Cancel )
            {
                safeToOpen = false;
            }
        }

        return safeToOpen;
    }

    /// <summary>
    /// Determines if toast can be closed.
    /// </summary>
    /// <returns>True if toast can be closed.</returns>
    private async Task<bool> IsSafeToClose()
    {
        var safeToClose = true;

        if ( Closing is not null )
        {
            var eventArgs = new ToastClosingEventArgs( false, closeReason );

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
    /// <param name="visible">Toast visibility flag.</param>
    protected virtual async Task HandleVisibilityStyles( bool visible )
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

        await closeableAdapter.Run( visible );
    }

    /// <summary>
    /// Fires all the events for this toast.
    /// </summary>
    /// <param name="visible"></param>
    protected virtual async Task RaiseEvents( bool visible )
    {
        await InvokeAsync( () => VisibleChanged.InvokeAsync( visible ) );

        if ( visible )
        {
            _Opened?.Invoke();

            await Opened.InvokeAsync();
        }
        else
        {
            _Closed?.Invoke();

            await Closed.InvokeAsync();
        }
    }

    /// <summary>
    /// Registers a new element that can close the toast.
    /// </summary>
    /// <param name="elementId">Element id.</param>
    internal void NotifyCloseActivatorIdInitialized( string elementId )
    {
        if ( !closeActivatorElementIds.Contains( elementId ) )
            closeActivatorElementIds.Add( elementId );
    }

    /// <summary>
    /// Removes the element that can close the toast.
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
    private async Task SetVisibleState( bool visible )
    {
        state = state with { Visible = visible };

        await HandleVisibilityStyles( visible );
        await RaiseEvents( visible );
    }

    /// <inheritdoc/>
    public Task BeginAnimation( bool visible )
    {
        if ( visible )
        {
            state = state with { Showing = true };
        }
        else
        {
            state = state with { Hiding = true };
        }

        DirtyClasses();
        DirtyStyles();

        return InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    public Task EndAnimation( bool visible )
    {
        if ( visible )
        {
            state = state with { Showing = false };
        }
        else
        {
            state = state with { Hiding = false };
        }

        DirtyClasses();
        DirtyStyles();

        return InvokeAsync( StateHasChanged );
    }

    internal void NotifyToastHeaderInitialized()
    {
        HasToastHeader = true;
    }

    internal void NotifyToastHeaderRemoved()
    {
        HasToastHeader = false;
    }

    internal void NotifyToastBodyInitialized()
    {
        HasToastBody = true;
    }

    internal void NotifyToastBodyRemoved()
    {
        HasToastBody = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if Toast contains the <see cref="ToastHeader"/> component.
    /// </summary>
    protected bool HasToastHeader { get; private set; }

    /// <summary>
    /// True if Toast contains the <see cref="ToastBody"/> component.
    /// </summary>
    protected bool HasToastBody { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Toast is visible.
    /// </summary>
    protected internal bool IsVisible => state.Visible;

    /// <summary>
    /// Gets the reference to state object for this Toast.
    /// </summary>
    protected internal ToastState State => state;

    /// <summary>
    /// The injected JavaScript module for closable operations.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Gets or sets the visibility state of the Toast.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Event callback for when the visibility state of the Toast changes.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Callback for handling the opening of the Toast.
    /// </summary>
    [Parameter] public Func<ToastOpeningEventArgs, Task> Opening { get; set; }

    /// <summary>
    /// Callback for handling the closing of the Toast.
    /// </summary>
    [Parameter] public Func<ToastClosingEventArgs, Task> Closing { get; set; }

    /// <summary>
    /// Event callback for when the Toast has been opened.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Event callback for when the Toast has been closed.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// Specifies whether the Toast should have an animated transition.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 300;

    /// <summary>
    /// Automatically hide the toast after the delay.
    /// </summary>
    [Parameter] public bool Autohide { get; set; } = true;

    /// <summary>
    /// Delay in milliseconds before hiding the toast.
    /// </summary>
    [Parameter] public double AutohideDelay { get; set; } = 5000;

    /// <summary>
    /// The content to be rendered inside the Toast.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}