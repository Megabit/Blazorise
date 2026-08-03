#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Provides a contextual menu that can be opened from a right-click target or programmatically.
/// </summary>
public partial class ContextMenu : BaseComponent, IAsyncDisposable
{
    #region Members

    private ContextMenuState state = new();

    private string toggleSelector;

    private string contextElementSelector;

    private string subscribedTargetSelector;

    private ContextMenuBody body;

    private IAsyncDisposable contextMenuSubscription;

    private IAsyncDisposable outsidePointerSubscription;

    private IAsyncDisposable keyDownSubscription;

    private bool floatingPositionDirty;

    private bool floatingPositionInitialized;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool visibilityChanged = parameters.TryGetValue<bool>( nameof( Visible ), out bool visibleResult )
            && state.Visible != visibleResult;

        await base.SetParametersAsync( parameters );

        if ( !visibilityChanged )
            return;

        if ( visibleResult )
            await Show();
        else
            await Hide();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenu() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await SynchronizeContextMenuSubscription();
        await SynchronizeVisibilitySubscriptions();

        if ( State.Visible )
            await EnsureFloatingPosition();
        else if ( floatingPositionInitialized )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );
            floatingPositionInitialized = false;
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            await DisposeSubscriptions();

            if ( Rendered )
                await JSModule.SafeDestroy( ElementRef, ElementId );
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Opens the context menu relative to its configured target.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Show()
        => Show( null, null, null );

    /// <summary>
    /// Opens the context menu at the supplied viewport coordinates.
    /// </summary>
    /// <param name="clientX">The viewport client X coordinate.</param>
    /// <param name="clientY">The viewport client Y coordinate.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Show( double clientX, double clientY )
        => Show( clientX, clientY, null );

    /// <summary>
    /// Hides the context menu.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Hide()
    {
        if ( !State.Visible )
            return;

        state = state with { Visible = false };
        DirtyClasses();

        await DisposeVisibilitySubscriptions();

        await VisibleChanged.InvokeAsync( false );

        await Closed.InvokeAsync( new ContextMenuEventArgs( State.ClientX, State.ClientY, null ) );

        await InvokeAsync( StateHasChanged );
    }

    internal void NotifyToggleInitialized( ContextMenuToggle toggle )
    {
        if ( toggle is null || toggle.ElementId.IsNullOrEmpty() )
            return;

        toggleSelector = CssSelectorUtilities.BuildElementIdSelector( toggle.ElementId );

        if ( State.Visible && State.ClientX is null && State.ClientY is null )
        {
            contextElementSelector = ResolvedTargetSelector;
            floatingPositionDirty = true;
        }
    }

    internal void NotifyToggleRemoved( ContextMenuToggle toggle )
    {
        if ( toggleSelector == CssSelectorUtilities.BuildElementIdSelector( toggle?.ElementId ) )
        {
            toggleSelector = null;

            if ( State.Visible && State.ClientX is null && State.ClientY is null )
            {
                contextElementSelector = ResolvedTargetSelector;
                floatingPositionDirty = true;
            }
        }
    }

    internal void NotifyBodyInitialized( ContextMenuBody body )
    {
        if ( body is null )
            return;

        this.body = body;
    }

    internal void NotifyBodyRemoved( ContextMenuBody body )
    {
        if ( ReferenceEquals( this.body, body ) )
            this.body = null;
    }

    private async Task Show( double? clientX, double? clientY, DocumentEventArgs documentEventArgs )
    {
        ContextMenuOpeningEventArgs openingEventArgs = new( clientX, clientY, documentEventArgs );

        await Opening.InvokeAsync( openingEventArgs );

        if ( openingEventArgs.Cancel )
        {
            if ( !State.Visible )
                await VisibleChanged.InvokeAsync( false );

            return;
        }

        contextElementSelector = documentEventArgs?.ContextElementSelector ?? documentEventArgs?.MatchedSelector ?? ResolvedTargetSelector;
        floatingPositionDirty = true;

        bool wasVisible = State.Visible;

        state = state with
        {
            Visible = true,
            ClientX = clientX,
            ClientY = clientY,
        };
        DirtyClasses();

        if ( Rendered )
            await SynchronizeVisibilitySubscriptions();

        if ( !wasVisible )
            await VisibleChanged.InvokeAsync( true );

        await Opened.InvokeAsync( new ContextMenuEventArgs( clientX, clientY, documentEventArgs ) );

        await InvokeAsync( StateHasChanged );
    }

    private async Task HandleContextMenu( DocumentEventArgs eventArgs )
    {
        if ( Disabled )
            return;

        await Show( eventArgs.ClientX, eventArgs.ClientY, eventArgs );
    }

    private async Task HandleOutsidePointer( DocumentEventArgs eventArgs )
    {
        if ( State.Visible && CloseOnOutsideClick )
            await Hide();
    }

    private async Task HandleKeyDown( DocumentEventArgs eventArgs )
    {
        if ( State.Visible && CloseOnEscape && string.Equals( eventArgs.Key, "Escape", StringComparison.Ordinal ) )
            await Hide();
    }

    private async Task SynchronizeContextMenuSubscription()
    {
        string targetSelector = ResolvedTargetSelector;

        if ( subscribedTargetSelector == targetSelector )
            return;

        if ( contextMenuSubscription is not null )
            await contextMenuSubscription.DisposeAsync();

        subscribedTargetSelector = targetSelector;
        contextMenuSubscription = string.IsNullOrWhiteSpace( targetSelector )
            ? null
            : await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.ContextMenu,
                Selector = targetSelector,
                PreventDefault = true,
                StopPropagation = true,
                Handler = HandleContextMenu,
            } );
    }

    private async Task SynchronizeVisibilitySubscriptions()
    {
        if ( State.Visible && CloseOnOutsideClick )
        {
            outsidePointerSubscription ??= await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.PointerDown,
                ExcludeSelector = RootSelector,
                Priority = -100,
                Handler = HandleOutsidePointer,
            } );
        }
        else if ( outsidePointerSubscription is not null )
        {
            await outsidePointerSubscription.DisposeAsync();
            outsidePointerSubscription = null;
        }

        if ( State.Visible && CloseOnEscape )
        {
            keyDownSubscription ??= await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.KeyDown,
                Handler = HandleKeyDown,
            } );
        }
        else if ( keyDownSubscription is not null )
        {
            await keyDownSubscription.DisposeAsync();
            keyDownSubscription = null;
        }
    }

    private async Task EnsureFloatingPosition()
    {
        if ( !floatingPositionDirty && floatingPositionInitialized )
            return;

        string bodyElementId = BodyElementId;

        if ( string.IsNullOrWhiteSpace( bodyElementId ) )
            return;

        await JSModule.Initialize( ElementRef, ElementId, bodyElementId, State.ClientX, State.ClientY, contextElementSelector, new()
        {
            Direction = Direction.Down.ToString( "g" ),
            Strategy = "fixed",
            OnlyWhenPositioned = true,
        } );

        floatingPositionDirty = false;
        floatingPositionInitialized = true;
    }

    private async ValueTask DisposeSubscriptions()
    {
        if ( contextMenuSubscription is not null )
        {
            await contextMenuSubscription.DisposeAsync();
            contextMenuSubscription = null;
        }

        await DisposeVisibilitySubscriptions();
    }

    private async Task DisposeVisibilitySubscriptions()
    {
        if ( outsidePointerSubscription is not null )
        {
            await outsidePointerSubscription.DisposeAsync();
            outsidePointerSubscription = null;
        }

        if ( keyDownSubscription is not null )
        {
            await keyDownSubscription.DisposeAsync();
            keyDownSubscription = null;
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the current context menu state.
    /// </summary>
    protected internal ContextMenuState State => state;

    internal bool EffectiveCloseOnClick => CloseOnClick;

    internal DropdownTrigger EffectiveSubmenuTrigger => SubmenuTrigger;

    internal int EffectiveSubmenuHoverCloseDelay => SubmenuHoverCloseDelay;

    private string RootSelector => $"[data-context-menu-id='{ElementId}']";

    private string ResolvedTargetSelector
        => !string.IsNullOrWhiteSpace( TargetSelector )
            ? TargetSelector
            : !string.IsNullOrWhiteSpace( TargetId )
                ? CssSelectorUtilities.BuildElementIdSelector( TargetId )
                : toggleSelector;

    private string BodyElementId
        => body?.ElementId;

    /// <summary>
    /// Gets the shared document observer.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Gets the context menu JavaScript module.
    /// </summary>
    [Inject] protected IJSContextMenuModule JSModule { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenu"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the menu is visible.
    /// </summary>
    /// <remarks>
    /// Setting this to <see langword="true"/> opens the menu relative to its configured target.
    /// Use <see cref="Show(double,double)"/> to open the menu at a specific viewport position.
    /// </remarks>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Occurs after the menu visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Specifies a CSS selector that opens this context menu when a matching element is right-clicked.
    /// </summary>
    [Parameter] public string TargetSelector { get; set; }

    /// <summary>
    /// Specifies an element id that opens this context menu when right-clicked.
    /// </summary>
    [Parameter] public string TargetId { get; set; }

    /// <summary>
    /// Closes the menu when clicking outside of it.
    /// </summary>
    [Parameter] public bool CloseOnOutsideClick { get; set; } = true;

    /// <summary>
    /// Closes the menu when pressing the Escape key.
    /// </summary>
    [Parameter] public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Closes the menu when a regular item is clicked.
    /// </summary>
    [Parameter] public bool CloseOnClick { get; set; } = true;

    /// <summary>
    /// Defines which pointer interactions can open or close nested submenus.
    /// </summary>
    [Parameter] public DropdownTrigger SubmenuTrigger { get; set; } = DropdownTrigger.All;

    /// <summary>
    /// Delay in milliseconds before hiding a hover-opened submenu after the mouse leaves it.
    /// </summary>
    [Parameter] public int SubmenuHoverCloseDelay { get; set; } = 300;

    /// <summary>
    /// Prevents the menu from opening through its observed target.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Occurs before the menu opens. Set <see cref="ContextMenuOpeningEventArgs.Cancel"/> to prevent opening.
    /// </summary>
    [Parameter] public EventCallback<ContextMenuOpeningEventArgs> Opening { get; set; }

    /// <summary>
    /// Occurs after the menu opens.
    /// </summary>
    [Parameter] public EventCallback<ContextMenuEventArgs> Opened { get; set; }

    /// <summary>
    /// Occurs after the menu closes.
    /// </summary>
    [Parameter] public EventCallback<ContextMenuEventArgs> Closed { get; set; }

    #endregion
}