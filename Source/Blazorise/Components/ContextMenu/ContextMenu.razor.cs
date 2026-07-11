#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
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

    private bool visible;

    private double clientX;

    private double clientY;

    private string toggleSelector;

    private string contextElementSelector;

    private string subscribedTargetSelector;

    private bool subscribedPreventDefault;

    private bool subscribedStopPropagation;

    private ContextMenuBody body;

    private IAsyncDisposable contextMenuSubscription;

    private IAsyncDisposable outsidePointerSubscription;

    private IAsyncDisposable keyDownSubscription;

    private bool floatingPositionDirty;

    private bool floatingPositionInitialized;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenu() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await EnsureContextMenuSubscription();
        await SynchronizeVisibilitySubscriptions();

        if ( Visible )
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
    /// Opens the context menu at the supplied document coordinates.
    /// </summary>
    /// <param name="clientX">The document client X coordinate.</param>
    /// <param name="clientY">The document client Y coordinate.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Show( double clientX, double clientY )
        => Show( clientX, clientY, null );

    /// <summary>
    /// Hides the context menu.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Hide()
    {
        if ( !Visible )
            return;

        visible = false;
        DirtyClasses();

        await DisposeVisibilitySubscriptions();

        await VisibleChanged.InvokeAsync( false );

        await Closed.InvokeAsync( new ContextMenuEventArgs( clientX, clientY, null ) );

        await InvokeAsync( StateHasChanged );
    }

    internal void NotifyToggleInitialized( ContextMenuToggle toggle )
    {
        if ( toggle is null || toggle.ElementId.IsNullOrEmpty() )
            return;

        toggleSelector = CssSelectorUtilities.BuildElementIdSelector( toggle.ElementId );
    }

    internal void NotifyToggleRemoved( ContextMenuToggle toggle )
    {
        if ( toggleSelector == CssSelectorUtilities.BuildElementIdSelector( toggle?.ElementId ) )
        {
            toggleSelector = null;
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

    private async Task Show( double clientX, double clientY, DocumentEventArgs documentEventArgs )
    {
        this.clientX = clientX;
        this.clientY = clientY;
        contextElementSelector = documentEventArgs?.ContextElementSelector ?? documentEventArgs?.MatchedSelector ?? ResolvedTargetSelector;
        floatingPositionDirty = true;

        bool wasVisible = Visible;

        visible = true;
        DirtyClasses();

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

        var openingEventArgs = new ContextMenuOpeningEventArgs( eventArgs.ClientX, eventArgs.ClientY, eventArgs );

        await Opening.InvokeAsync( openingEventArgs );

        if ( openingEventArgs.Cancel )
            return;

        await Show( eventArgs.ClientX, eventArgs.ClientY, eventArgs );
    }

    private async Task HandleOutsidePointer( DocumentEventArgs eventArgs )
    {
        if ( Visible && CloseOnOutsideClick )
            await Hide();
    }

    private async Task HandleKeyDown( DocumentEventArgs eventArgs )
    {
        if ( Visible && CloseOnEscape && string.Equals( eventArgs.Key, "Escape", StringComparison.Ordinal ) )
            await Hide();
    }

    private async Task EnsureContextMenuSubscription()
    {
        string targetSelector = ResolvedTargetSelector;

        if ( subscribedTargetSelector == targetSelector
             && subscribedPreventDefault == PreventDefault
             && subscribedStopPropagation == StopPropagation )
            return;

        if ( contextMenuSubscription is not null )
            await contextMenuSubscription.DisposeAsync();

        subscribedTargetSelector = targetSelector;
        subscribedPreventDefault = PreventDefault;
        subscribedStopPropagation = StopPropagation;
        contextMenuSubscription = string.IsNullOrWhiteSpace( targetSelector )
            ? null
            : await DocumentObserver.Subscribe( new()
            {
                OwnerId = ElementId,
                EventTypes = DocumentEventTypes.ContextMenu,
                Selector = targetSelector,
                PreventDefault = PreventDefault,
                StopPropagation = StopPropagation,
                Handler = HandleContextMenu,
            } );
    }

    private async Task SynchronizeVisibilitySubscriptions()
    {
        if ( Visible && CloseOnOutsideClick )
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

        if ( Visible && CloseOnEscape )
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

        await JSModule.Initialize( ElementRef, ElementId, bodyElementId, clientX, clientY, contextElementSelector, new()
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
    /// Gets or sets the menu visibility.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible;
        set
        {
            if ( visible == value )
                return;

            visible = value;
            floatingPositionDirty = value;
            DirtyClasses();
        }
    }

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
    /// Prevents the browser's default context menu for observed targets.
    /// </summary>
    [Parameter] public bool PreventDefault { get; set; } = true;

    /// <summary>
    /// Stops propagation for observed context menu events.
    /// </summary>
    [Parameter] public bool StopPropagation { get; set; } = true;

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