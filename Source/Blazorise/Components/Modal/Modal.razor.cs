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
/// A classic modal overlay, in which you can include any content you want.
/// </summary>
public partial class Modal : BaseComponent, ICloseActivator, IAnimatedComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Tracks whether the component fulfills the requirements to be lazy loaded and then kept rendered to the DOM.
    /// </summary>
    protected bool lazyLoaded;

    /// <summary>
    /// Holds the state of this modal dialog.
    /// </summary>
    private ModalState state = new()
    {
        Visible = false,
    };

    /// <summary>
    /// Centers the modal vertically.
    /// </summary>
    private bool centered;

    /// <summary>
    /// Scrolls the modal content independent of the page itself.
    /// </summary>
    private bool scrollable;

    /// <summary>
    /// Changes the size of the modal.
    /// </summary>
    private ModalSize modalSize = ModalSize.Default;

    /// <summary>
    /// Holds the last received reason for modal closure.
    /// </summary>
    private CloseReason closeReason = CloseReason.None;

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

    /// <summary>
    /// Manages the modal visibility states.
    /// </summary>
    private CloseableAdapter closeableAdapter;

    /// <summary>
    /// Internal event that is raised after the modal has opened.
    /// </summary>
    internal event Action _Opened;

    /// <summary>
    /// Internal event that is raised after the modal has closed.
    /// </summary>
    internal event Action _Closed;

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public Modal()
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
        builder.Append( ClassProvider.ModalFade( Animated && State.Showing, Animated && State.Hiding ) );
        builder.Append( ClassProvider.ModalVisible( IsVisible ) );
        builder.Append( ClassProvider.ModalSize( Size ) );
        builder.Append( ClassProvider.ModalCentered( Centered ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.ModalShow( IsVisible ) );
        builder.Append( StyleProvider.ModalFade( Animated && State.Showing, Animated && State.Hiding ) );
        builder.Append( StyleProvider.ModalZIndex( OpenIndex ) );
        builder.Append( StyleProvider.ModalAnimationDuration( Animated, AnimationDuration ) );

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
                catch ( Microsoft.JSInterop.JSDisconnectedException )
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
            var closeModalTask = JSModalModule.CloseModal( ElementRef );

            try
            {
                await closeModalTask;
            }
            catch when ( closeModalTask.IsCanceled )
            {
            }
            catch ( Microsoft.JSInterop.JSDisconnectedException )
            {
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Starts the modal opening process.
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
                    BackdropVisible = true;
            }

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
            await SetVisibleState( false );

            if ( !Animated )
            {
                DirtyClasses();
                DirtyStyles();

                if ( ShowBackdrop )
                    BackdropVisible = false;
            }

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

        if ( Opening is not null )
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

        if ( Closing is not null )
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
    protected virtual async Task HandleVisibilityStyles( bool visible )
    {
        if ( visible )
        {
            if ( ModalContext is not null )
            {
                // save the state of the modal index
                state = state with { OpenIndex = ModalContext.RaiseModalOpenIndex() };
            }

            jsRegistered = true;

            ExecuteAfterRender( async () =>
            {
                await JSModalModule.OpenModal( ElementRef, ScrollToTop );

                await JSClosableModule.Register( dotNetObjectRef, ElementRef );
            } );
        }
        else
        {
            if ( ModalContext is not null )
            {
                // save the state of the modal index
                state = state with { OpenIndex = ModalContext.DecreaseModalOpenIndex() };
            }

            jsRegistered = false;

            ExecuteAfterRender( async () =>
            {
                await JSModalModule.CloseModal( ElementRef );

                await JSClosableModule.Unregister( this );
            } );
        }

        await closeableAdapter.Run( visible );
    }

    /// <summary>
    /// Fires all the events for this modal
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
    private async Task SetVisibleState( bool visible )
    {
        if ( visible )
            lazyLoaded = RenderMode == ModalRenderMode.LazyLoad;

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

    #endregion

    #region Properties

    /// <summary>
    /// Returns true if the modal should be visible.
    /// </summary>
    protected internal bool IsVisible => State.Visible == true;

    /// <summary>
    /// Returns the opened index of modal.
    /// </summary>
    protected internal int OpenIndex => State.OpenIndex;

    /// <summary>
    /// Returns true if the modal backdrop should be visible.
    /// </summary>
    protected internal bool BackdropVisible = false;

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the reference to state object for this modal.
    /// </summary>
    protected internal ModalState State => state;

    /// <summary>
    /// Evaluates the rendering mode to see of the modal content is ready to be rendered.
    /// </summary>
    protected bool ShouldRenderContent => RenderMode == ModalRenderMode.Default || ( RenderMode == ModalRenderMode.LazyReload && IsVisible ) || ( RenderMode == ModalRenderMode.LazyLoad && lazyLoaded );

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
    /// Gets or sets the modal shared context.
    /// </summary>
    [Inject] private IModalSharedContext ModalContext { get; set; }

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

    /// <inheritdoc/>
    [Parameter] public bool Animated { get; set; } = true;

    /// <inheritdoc/>
    [Parameter] public int AnimationDuration { get; set; } = 150;

    /// <summary>
    /// Defines how the modal content will be rendered.
    /// </summary>
    [Parameter] public ModalRenderMode RenderMode { get; set; }

    /// <summary>
    /// Defines if the modal should keep the input focus at all times.
    /// </summary>
    [Parameter] public bool? FocusTrap { get; set; }

    /// <summary>
    /// Centers the modal vertically.
    /// </summary>
    [Parameter]
    public bool Centered
    {
        get => centered;
        set
        {
            if ( centered == value )
                return;

            centered = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Scrolls the modal content independent of the page itself.
    /// </summary>
    [Parameter]
    public bool Scrollable
    {
        get => scrollable;
        set
        {
            if ( scrollable == value )
                return;

            scrollable = value;
        }
    }

    /// <summary>
    /// Changes the size of the modal.
    /// </summary>
    [Parameter]
    public ModalSize Size
    {
        get => modalSize;
        set
        {
            if ( modalSize == value )
                return;

            modalSize = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Modal"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}