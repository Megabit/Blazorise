#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Components;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazorise.Providers;
using System.Runtime.CompilerServices;
#endregion

namespace Blazorise;

/// <summary>
/// A sidebar component that slides in and out of the screen.
/// </summary>
public partial class Offcanvas : BaseComponent, ICloseActivator, IAnimatedComponent, IHideableComponent, IAsyncDisposable
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

    private bool showing;

    private bool hiding;

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
    public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChildClicked )
    {
        // Check if the elementId belongs to an element within the Offcanvas content
        bool isElementInOffcanvas = closeActivatorElementIds.Contains( elementId );

        // Determine if it's safe to close based on the closeReason and isChildClicked
        bool isSafeToClose = closeReason == CloseReason.UserClosing && ( isChildClicked || !isElementInOffcanvas );
        return Task.FromResult( true );
    }

    /// <inheritdoc/>
    public Task Close( CloseReason closeReason )
    {
        return Hide( closeReason );
    }

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
        builder.Append( ClassProvider.OffcanvasFade( showing, hiding ) );
        builder.Append( ClassProvider.OffcanvasVisible( IsVisible ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( StyleProvider.OffcanvasAnimationDuration( AnimationDuration ) );
    }

    /// <inheritdoc/>
    protected virtual async Task HandleVisibilityStyles( bool visible )
    {
        if ( visible )
        {
            jsRegistered = true;

            ExecuteAfterRender( async () =>
            {
                await JSOffcanvasModule.OpenOffcanvas( ElementRef, Placement );

                await JSClosableModule.Register( dotNetObjectRef, ElementRef );
            } );
        }
        else
        {
            jsRegistered = false;

            ExecuteAfterRender( async () =>
            {
                await JSOffcanvasModule.CloseOffcanvas( ElementRef );


                await JSClosableModule.Unregister( this );
            } );
        }

        await closeableAdapter.Run( visible );
    }

    /// <inheritdoc/>
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

    internal void NotifyHasOffcanvasHeader()
    {
        HasOffcanvasHeader = true;
    }

    internal void NotifyHasOffcanvasBody()
    {
        HasOffcanvasBody = true;
    }

    internal void NotifyCloseActivatorIdInitialized( string elementId )
    {
        if ( !closeActivatorElementIds.Contains( elementId ) )
            closeActivatorElementIds.Add( elementId );
    }

    internal void NotifyCloseActivatorIdRemoved( string elementId )
    {
        if ( closeActivatorElementIds.Contains( elementId ) )
            closeActivatorElementIds.Remove( elementId );
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

        if ( Opening != null )
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

        if ( Closing != null )
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
            showing = true;

            BackdropVisible = ShowBackdrop;
        }
        else
        {
            hiding = true;
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
            showing = false;
        }
        else
        {
            hiding = false;
        }

        DirtyClasses();
        DirtyStyles();

        BackdropVisible = ShowBackdrop && visible;

        return InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if Offcanvas contains the <see cref="OffcanvasHeader"/> component.
    /// </summary>
    protected bool HasOffcanvasHeader { get; set; }

    /// <summary>
    /// True if Offcanvas contains the <see cref="OffcanvasBody"/> component.
    /// </summary>
    protected bool HasOffcanvasBody { get; set; }

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is visible.
    /// </summary>
    protected internal bool IsVisible => state.Visible == true;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is positioned to the right.
    /// </summary>
    protected internal bool IsRight => Placement == Placement.End;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is positioned to the left.
    /// </summary>
    protected internal bool IsLeft => Placement == Placement.Start;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is positioned at the top.
    /// </summary>
    protected internal bool IsTop => Placement == Placement.Top;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is positioned at the bottom.
    /// </summary>
    protected internal bool IsBottom => Placement == Placement.Bottom;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is positioned vertically (top or bottom).
    /// </summary>
    protected internal bool IsVertical => IsTop || IsBottom;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is positioned horizontally (left or right).
    /// </summary>
    protected internal bool IsHorizontal => IsLeft || IsRight;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is vertically collapsed (hidden).
    /// </summary>
    protected internal bool IsVerticalCollapsed => IsVertical && !IsVisible;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is horizontally collapsed (hidden).
    /// </summary>
    protected internal bool IsHorizontalCollapsed => IsHorizontal && !IsVisible;

    /// <summary>
    /// Gets a value indicating whether the Offcanvas is collapsed (hidden).
    /// </summary>
    protected internal bool IsCollapsed => !IsVertical && !IsHorizontal && !IsVisible;

    /// <summary>
    /// Gets the reference to state object for this Offcanvas.
    /// </summary>
    protected internal OffcanvasState State => state;

    /// <summary>
    /// Returns true if the offcanvas backdrop should be visible.
    /// </summary>
    protected internal bool BackdropVisible = false;

    /// <summary>
    /// Gets the CSS class for the Offcanvas based on its position.
    /// </summary>
    protected internal string OffcanvasClass
    {
        get
        {
            var placement = IsVisible ? Placement : Placement.Start;

            return placement switch
            {
                Placement.Start => "offcanvas-start",
                Placement.End => "offcanvas-end",
                Placement.Top => "offcanvas-top",
                Placement.Bottom => "offcanvas-bottom",
                _ => "",
            };
        }
    }

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

    /// <summary>
    /// The injected JavaScript module for Offcanvas operations.
    /// </summary>
    [Inject] public IJSOffcanvasModule JSOffcanvasModule { get; set; }

    /// <summary>
    /// The injected JavaScript module for closable operations.
    /// </summary>
    [Inject] public IJSClosableModule JSClosableModule { get; set; }

    #endregion
}