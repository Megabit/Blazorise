#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A component to displays temporary content to the user.
/// </summary>
public partial class Toast : BaseComponent, IAnimatedComponent, IDisposable
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
    /// Adapter for the Closeable interface to handle close events.
    ///</summary>
    private CloseableAdapter closeableAdapter;

    ///<summary>
    /// Internal event that is triggered when the Toast is opened.
    ///</summary>
    internal event Action _Opened;

    ///<summary>
    /// Internal event that is triggered when the Toast is closed.
    ///</summary>
    internal event Action _Closed;

    /// <summary>
    /// Timer used to countdown the close event.
    /// </summary>
    private AsyncCountdownTimer countdownTimer;

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
            parameters.TryGetValue<Func<ToastOpeningEventArgs, Task>>( nameof( Opening ), out var paramOpening );
            parameters.TryGetValue<Func<ToastClosingEventArgs, Task>>( nameof( Closing ), out var paramClosing );

            if ( paramVisible && await IsSafeToOpen( paramOpening ) )
            {
                await base.SetParametersAsync( parameters );
                await SetVisibleState( true );
            }
            else if ( !paramVisible && await IsSafeToClose( paramClosing ) )
            {
                await base.SetParametersAsync( parameters );
                await SetVisibleState( false );
            }

            if ( Rendered )
                await InvokeAsync( StateHasChanged );
        }
        else
        {
            await base.SetParametersAsync( parameters );
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( Autohide && AutohideDelay > 0 && countdownTimer is null )
        {
            countdownTimer = new AsyncCountdownTimer( AutohideDelay )
                .Elapsed( OnCountdownTimerElapsed );
        }

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( countdownTimer is not null )
            {
                countdownTimer.Dispose();
                countdownTimer = null;
            }
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Toast() );
        builder.Append( ClassProvider.ToastAnimated( Animated ) );
        builder.Append( ClassProvider.ToastFade( IsVisible, Animated && State.Showing, Animated && State.Hiding ) );
        builder.Append( ClassProvider.ToastVisible( IsVisible ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( StyleProvider.ToastAnimationDuration( Animated, AnimationDuration ) );
    }

    private Task OnCountdownTimerElapsed()
    {
        return Hide( CloseReason.None );
    }

    /// <summary>
    /// Starts the toast opening process.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Show()
    {
        if ( state.Visible )
            return;

        if ( await IsSafeToOpen( Opening ) )
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

        if ( await IsSafeToClose( Closing ) )
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
    /// <param name="opening">Callback for handling the opening of the Toast.</param>
    /// <returns>True if toast can be opened.</returns>
    private async Task<bool> IsSafeToOpen( Func<ToastOpeningEventArgs, Task> opening )
    {
        var safeToOpen = true;

        if ( opening is not null )
        {
            var eventArgs = new ToastOpeningEventArgs( false );

            await opening.Invoke( eventArgs );

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
    /// <param name="closing">Callback for handling the closing of the Toast.</param>
    /// <returns>True if toast can be closed.</returns>
    private async Task<bool> IsSafeToClose( Func<ToastClosingEventArgs, Task> closing )
    {
        var safeToClose = true;

        if ( closing is not null )
        {
            var eventArgs = new ToastClosingEventArgs( false, closeReason );

            await closing.Invoke( eventArgs );

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
            ExecuteAfterRender( () =>
            {
                countdownTimer?.Start();

                return Task.CompletedTask;
            } );

            _Opened?.Invoke();

            await Opened.InvokeAsync();
        }
        else
        {
            _Closed?.Invoke();

            await Closed.InvokeAsync();
        }
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
    protected virtual async Task SetVisibleState( bool visible )
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
    protected internal ToastState State { get => state; set => state = value; }

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