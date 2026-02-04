#region Using directives
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
/// A sidebar component that slides in and out of the screen.
/// </summary>
public partial class Offcanvas : BaseComponent<OffcanvasClasses, OffcanvasStyles>, ICloseActivator, IAnimatedComponent, IHideableComponent, IAsyncDisposable
{
    #region Members

    ///<summary>
    /// Holds the state of the Offcanvas component.
    ///</summary>
    private OffcanvasState state = new()
    {
        Visible = false,
    };

    ///<summary>
    /// Holds the reason for the Offcanvas closing.
    ///</summary>
    private CloseReason closeReason = CloseReason.None;

    ///<summary>
    /// Holds the Offcanvas header component.
    ///</summary>
    private OffcanvasHeader header;

    ///<summary>
    /// Holds the Offcanvas body component.
    ///</summary>
    private OffcanvasBody body;

    ///<summary>
    /// Indicates whether the Offcanvas has been registered with JavaScript.
    ///</summary>
    private bool jsRegistered;

    ///<summary>
    /// Reference to the .NET object adapter for the CloseActivator.
    ///</summary>
    private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

    ///<summary>
    /// List of element IDs that act as close activators for the Offcanvas.
    ///</summary>
    private readonly List<string> closeActivatorElementIds = new();

    ///<summary>
    /// Adapter for the Closeable interface to handle close events.
    ///</summary>
    private CloseableAdapter closeableAdapter;

    ///<summary>
    /// Event that is triggered when the Offcanvas is opened.
    ///</summary>
    internal event Action _Opened;

    ///<summary>
    /// Event that is triggered when the Offcanvas is closed.
    ///</summary>
    internal event Action _Closed;

    #endregion

    #region Constructors

    ///<summary>
    /// Initializes a new instance of the <see cref="Offcanvas"/> class.
    ///</summary>
    public Offcanvas()
    {
        closeableAdapter = new( this );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<bool>( nameof( Visible ), out var visibleResult ) && state.Visible != visibleResult )
        {
            if ( visibleResult && await IsSafeToOpen() )
            {
                await base.SetParametersAsync( parameters );
                await SetVisibleState( true );
            }
            else if ( !visibleResult && await IsSafeToClose() )
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
        builder.Append( ClassProvider.Offcanvas() );
        builder.Append( ClassProvider.OffcanvasPlacement( Placement, IsVisible ) );
        builder.Append( ClassProvider.OffcanvasFade( Animated && State.Showing, Animated && State.Hiding ) );
        builder.Append( ClassProvider.OffcanvasVisible( IsVisible ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( StyleProvider.OffcanvasAnimationDuration( Animated, AnimationDuration ) );
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
    /// Starts the offcanvas opening process.
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

                if ( ShowBackdrop )
                {
                    BackdropVisible = true;
                }
            }

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Fires the offcanvas dialog closure process.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Hide()
    {
        return Hide( CloseReason.UserClosing );
    }

    /// <summary>
    /// Internal method to hide the offcanvas with reason of closing.
    /// </summary>
    /// <param name="closeReason">Reason why offcanvas was closed.</param>
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

                if ( ShowBackdrop )
                {
                    BackdropVisible = false;
                }
            }

            this.closeReason = CloseReason.None;

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Determines if offcanvas can be opened.
    /// </summary>
    /// <returns>True if offcanvas can be opened.</returns>
    private async Task<bool> IsSafeToOpen()
    {
        var safeToOpen = true;

        if ( Opening is not null )
        {
            var eventArgs = new OffcanvasOpeningEventArgs( false );

            await Opening.Invoke( eventArgs );

            if ( eventArgs.Cancel )
            {
                safeToOpen = false;
            }
        }

        return safeToOpen;
    }

    /// <summary>
    /// Determines if offcanvas can be closed.
    /// </summary>
    /// <returns>True if offcanvas can be closed.</returns>
    private async Task<bool> IsSafeToClose()
    {
        var safeToClose = true;

        if ( Closing is not null )
        {
            var eventArgs = new OffcanvasClosingEventArgs( false, closeReason );

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
    /// <param name="visible">Offcanvas visibility flag.</param>
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
    /// Fires all the events for this offcanvas.
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
    /// Registers a new element that can close the offcanvas.
    /// </summary>
    /// <param name="elementId">Element id.</param>
    internal void NotifyCloseActivatorIdInitialized( string elementId )
    {
        if ( !closeActivatorElementIds.Contains( elementId ) )
            closeActivatorElementIds.Add( elementId );
    }

    /// <summary>
    /// Removes the element that can close the offcanvas.
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

            BackdropVisible = ShowBackdrop;
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

        BackdropVisible = ShowBackdrop && visible;

        return InvokeAsync( StateHasChanged );
    }

    internal void NotifyOffcanvasHeaderInitialized( OffcanvasHeader offcanvasHeader )
    {
        HasOffcanvasHeader = true;

        if ( offcanvasHeader is not null && !ReferenceEquals( header, offcanvasHeader ) )
        {
            header = offcanvasHeader;
            NotifyAriaChanged();
        }
    }

    internal void NotifyOffcanvasHeaderRemoved( OffcanvasHeader offcanvasHeader )
    {
        HasOffcanvasHeader = false;

        if ( ReferenceEquals( header, offcanvasHeader ) )
        {
            header = null;
            NotifyAriaChanged();
        }
    }

    internal void NotifyOffcanvasFooterInitialized()
    {
        HasOffcanvasFooter = true;
    }

    internal void NotifyOffcanvasFooterRemoved()
    {
        HasOffcanvasFooter = false;
    }

    internal void NotifyOffcanvasBodyInitialized( OffcanvasBody offcanvasBody )
    {
        HasOffcanvasBody = true;

        if ( offcanvasBody is not null && !ReferenceEquals( body, offcanvasBody ) )
        {
            body = offcanvasBody;
            NotifyAriaChanged();
        }
    }

    internal void NotifyOffcanvasBodyRemoved( OffcanvasBody offcanvasBody )
    {
        HasOffcanvasBody = false;

        if ( ReferenceEquals( body, offcanvasBody ) )
        {
            body = null;
            NotifyAriaChanged();
        }
    }

    private void NotifyAriaChanged()
    {
        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if Offcanvas contains the <see cref="OffcanvasHeader"/> component.
    /// </summary>
    protected bool HasOffcanvasHeader { get; private set; }

    /// <summary>
    /// True if Offcanvas contains the <see cref="OffcanvasFooter"/> component.
    /// </summary>
    protected bool HasOffcanvasFooter { get; private set; }

    /// <summary>
    /// True if Offcanvas contains the <see cref="OffcanvasBody"/> component.
    /// </summary>
    protected bool HasOffcanvasBody { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is visible.
    /// </summary>
    protected internal bool IsVisible => state.Visible;

    /// <summary>
    /// Gets the aria-modal attribute value.
    /// </summary>
    protected string AriaModal => IsVisible ? "true" : null;

    /// <summary>
    /// Gets the aria-labelledby attribute value.
    /// </summary>
    protected string AriaLabelledBy => header?.ElementId;

    /// <summary>
    /// Gets the aria-describedby attribute value.
    /// </summary>
    protected string AriaDescribedBy => body?.ElementId;

    /// <summary>
    /// Gets the reference to state object for this Offcanvas.
    /// </summary>
    protected internal OffcanvasState State => state;

    /// <summary>
    /// Returns true if the offcanvas backdrop should be visible.
    /// </summary>
    protected internal bool BackdropVisible = false;

    /// <summary>
    /// The injected JavaScript module for closable operations.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Gets or sets the visibility state of the Offcanvas.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Event callback for when the visibility state of the Offcanvas changes.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Callback for handling the opening of the Offcanvas.
    /// </summary>
    [Parameter] public Func<OffcanvasOpeningEventArgs, Task> Opening { get; set; }

    /// <summary>
    /// Callback for handling the closing of the Offcanvas.
    /// </summary>
    [Parameter] public Func<OffcanvasClosingEventArgs, Task> Closing { get; set; }

    /// <summary>
    /// Event callback for when the Offcanvas has been opened.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Event callback for when the Offcanvas has been closed.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// Specifies the position of the Offcanvas.
    /// </summary>
    [Parameter] public Placement Placement { get; set; } = Placement.Start;

    /// <summary>
    /// Specifies whether the Offcanvas should have an animated transition.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 300;

    /// <summary>
    /// Specifies whether to render the backdrop for this <see cref="Offcanvas"/>.
    /// </summary>
    [Parameter] public bool ShowBackdrop { get; set; } = true;

    /// <summary>
    /// The content to be rendered inside the Offcanvas.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}