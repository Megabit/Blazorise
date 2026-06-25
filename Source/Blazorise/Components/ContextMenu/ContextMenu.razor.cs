#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
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

    private string targetSelector;

    private string targetId;

    private bool preventDefault = true;

    private bool stopPropagation = true;

    private string subscribedTargetSelector;

    private List<ContextMenuBody> bodies;

    private bool subscriptionsDirty = true;

    private IAsyncDisposable contextMenuSubscription;

    private IAsyncDisposable outsidePointerSubscription;

    private IAsyncDisposable keyDownSubscription;

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
        if ( firstRender || subscriptionsDirty )
            await EnsureSubscriptions();

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
            await DisposeSubscriptions();

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

        if ( VisibleChanged.HasDelegate )
            await VisibleChanged.InvokeAsync( false );

        if ( Closed.HasDelegate )
            await Closed.InvokeAsync( new ContextMenuEventArgs( clientX, clientY, null ) );

        await InvokeAsync( StateHasChanged );
    }

    internal void NotifyToggleInitialized( ContextMenuToggle toggle )
    {
        if ( toggle is null || toggle.ElementId.IsNullOrEmpty() )
            return;

        toggleSelector = CssSelectorUtilities.BuildElementIdSelector( toggle.ElementId );
        subscriptionsDirty = true;
    }

    internal void NotifyToggleRemoved( ContextMenuToggle toggle )
    {
        if ( toggleSelector == CssSelectorUtilities.BuildElementIdSelector( toggle?.ElementId ) )
        {
            toggleSelector = null;
            subscriptionsDirty = true;
        }
    }

    internal void NotifyBodyInitialized( ContextMenuBody body )
    {
        if ( body is null )
            return;

        bodies ??= new();

        if ( !bodies.Contains( body ) )
            bodies.Add( body );
    }

    internal void NotifyBodyRemoved( ContextMenuBody body )
    {
        if ( body is not null && bodies is not null )
            bodies.Remove( body );
    }

    private async Task Show( double clientX, double clientY, DocumentEventArgs documentEventArgs )
    {
        this.clientX = clientX;
        this.clientY = clientY;

        bool wasVisible = Visible;

        visible = true;
        DirtyClasses();

        if ( !wasVisible && VisibleChanged.HasDelegate )
            await VisibleChanged.InvokeAsync( true );

        if ( Opened.HasDelegate )
            await Opened.InvokeAsync( new ContextMenuEventArgs( clientX, clientY, documentEventArgs ) );

        await InvokeAsync( StateHasChanged );
    }

    private async Task HandleContextMenu( DocumentEventArgs eventArgs )
    {
        if ( Disabled )
            return;

        var openingEventArgs = new ContextMenuOpeningEventArgs( eventArgs.ClientX, eventArgs.ClientY, eventArgs );

        if ( Opening.HasDelegate )
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

    private async Task EnsureSubscriptions()
    {
        subscriptionsDirty = false;

        string targetSelector = ResolvedTargetSelector;

        if ( subscribedTargetSelector != targetSelector )
        {
            if ( contextMenuSubscription is not null )
            {
                await contextMenuSubscription.DisposeAsync();
                contextMenuSubscription = null;
            }

            subscribedTargetSelector = targetSelector;

            if ( !string.IsNullOrWhiteSpace( targetSelector ) )
            {
                contextMenuSubscription = await DocumentObserver.Subscribe( new()
                {
                    OwnerId = ElementId,
                    EventTypes = DocumentEventTypes.ContextMenu,
                    Selector = targetSelector,
                    PreventDefault = PreventDefault,
                    StopPropagation = StopPropagation,
                    Handler = HandleContextMenu,
                } );
            }
        }

        outsidePointerSubscription ??= await DocumentObserver.Subscribe( new()
        {
            OwnerId = ElementId,
            EventTypes = DocumentEventTypes.PointerDown,
            ExcludeSelector = RootSelector,
            Priority = -100,
            Handler = HandleOutsidePointer,
        } );

        keyDownSubscription ??= await DocumentObserver.Subscribe( new()
        {
            OwnerId = ElementId,
            EventTypes = DocumentEventTypes.KeyDown,
            Handler = HandleKeyDown,
        } );
    }

    private async ValueTask DisposeSubscriptions()
    {
        if ( contextMenuSubscription is not null )
        {
            await contextMenuSubscription.DisposeAsync();
            contextMenuSubscription = null;
        }

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

    private void DirtyBodyStyles()
    {
        if ( bodies is null )
            return;

        foreach ( ContextMenuBody body in bodies )
            body.DirtyStyles();
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        DirtyBodyStyles();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    internal double ClientX => clientX;

    internal double ClientY => clientY;

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

    /// <summary>
    /// Gets the shared document observer.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

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
    [Parameter]
    public string TargetSelector
    {
        get => targetSelector;
        set
        {
            if ( targetSelector.IsEqual( value ) )
                return;

            targetSelector = value;
            subscriptionsDirty = true;
        }
    }

    /// <summary>
    /// Specifies an element id that opens this context menu when right-clicked.
    /// </summary>
    [Parameter]
    public string TargetId
    {
        get => targetId;
        set
        {
            if ( targetId.IsEqual( value ) )
                return;

            targetId = value;
            subscriptionsDirty = true;
        }
    }

    /// <summary>
    /// Prevents the browser's default context menu for observed targets.
    /// </summary>
    [Parameter]
    public bool PreventDefault
    {
        get => preventDefault;
        set
        {
            if ( preventDefault == value )
                return;

            preventDefault = value;
            subscribedTargetSelector = null;
            subscriptionsDirty = true;
        }
    }

    /// <summary>
    /// Stops propagation for observed context menu events.
    /// </summary>
    [Parameter]
    public bool StopPropagation
    {
        get => stopPropagation;
        set
        {
            if ( stopPropagation == value )
                return;

            stopPropagation = value;
            subscribedTargetSelector = null;
            subscriptionsDirty = true;
        }
    }

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