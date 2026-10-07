#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays application-level file commands and pages inside the document workspace.
/// </summary>
public partial class RibbonBackstage : BaseComponent, IDisposable
{
    #region Members

    private readonly List<RibbonBackstageItem> items = new();

    private Div containerRef;

    private Button backButtonRef;

    private bool wasVisible;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the default backstage surface utilities.
    /// </summary>
    public RibbonBackstage()
    {
        Background = Blazorise.Background.White;
        Height = Blazorise.Height.Is100;
        Width = Blazorise.Width.Is100;
        Overflow = Blazorise.Overflow.Hidden;
        ZIndex = Blazorise.ZIndex.Offcanvas;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentWorkspace?.RegisterBackstage( this );
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var elementIdChanged = parameters.IsParameterChanged( ElementId );

        await base.SetParametersAsync( parameters );

        if ( elementIdChanged )
        {
            ParentWorkspace?.Refresh();
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        SynchronizeState();

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = containerRef.ElementRef;
        }

        var visibilityChanged = EffectiveVisible != wasVisible;
        var isVisible = EffectiveVisible;
        wasVisible = isVisible;

        await base.OnAfterRenderAsync( firstRender );
        await HandleSelectItem( EffectiveSelectedItem );

        if ( !visibilityChanged || EffectiveVisible != isVisible || Disposed || AsyncDisposed )
        {
            return;
        }

        if ( isVisible )
        {
            await backButtonRef.Focus( false );

            if ( EffectiveVisible && !Disposed && !AsyncDisposed )
            {
                await Opened.InvokeAsync();
            }
        }
        else
        {
            await Closed.InvokeAsync();
        }
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentWorkspace?.UnregisterBackstage( this );
        }

        base.Dispose( disposing );
    }

    internal void RegisterItem( RibbonBackstageItem item )
    {
        ValidateItem( item );

        items.Add( item );
        Refresh();
    }

    internal void ValidateItem( RibbonBackstageItem item )
    {
        if ( string.IsNullOrWhiteSpace( item.Name ) )
        {
            throw new InvalidOperationException( "RibbonBackstageItem requires a non-empty Name." );
        }

        if ( items.Any( registeredItem => !ReferenceEquals( registeredItem, item ) && registeredItem.Name == item.Name ) )
        {
            throw new InvalidOperationException( $"Backstage item names must be unique. The name '{item.Name}' is already registered." );
        }
    }

    internal void UnregisterItem( RibbonBackstageItem item )
    {
        items.Remove( item );
        Refresh();
    }

    internal void Refresh()
    {
        if ( !Disposed && !AsyncDisposed )
        {
            SynchronizeState();

            _ = InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Shows backstage through the visibility notification path.
    /// </summary>
    public Task Show() => HandleVisible( true );

    /// <summary>
    /// Hides backstage while preserving its selection and visited pages.
    /// </summary>
    public Task Hide() => HandleVisible( false );

    /// <summary>
    /// Selects an available page without changing backstage visibility.
    /// </summary>
    /// <param name="name">
    /// Name of the page to select.
    /// </param>
    public Task SelectItem( string name )
        => items.Any( item => item.Name == name && item.CanSelect ) ? HandleSelectItem( name ) : Task.CompletedTask;

    /// <summary>
    /// Handles activation of the Back button.
    /// </summary>
    protected Task OnBackHandler() => Hide();

    /// <summary>
    /// Handles keyboard input bubbling from the backstage surface.
    /// </summary>
    protected Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
        => EffectiveVisible && eventArgs.Key == "Escape" ? Hide() : Task.CompletedTask;

    /// <summary>
    /// Applies visibility and awaits the application's binding callback.
    /// </summary>
    protected virtual async Task HandleVisible( bool visible )
    {
        if ( ParentWorkspace is not null )
        {
            await ParentWorkspace.SetBackstageVisible( visible );

            return;
        }

        if ( Visible == visible )
        {
            return;
        }

        Visible = visible;
        SynchronizeState();

        await VisibleChanged.InvokeAsync( visible );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Applies page selection and awaits the application's binding callback.
    /// </summary>
    protected virtual async Task HandleSelectItem( string name )
    {
        if ( string.Equals( SelectedItem, name, StringComparison.Ordinal ) )
        {
            return;
        }

        SelectedItem = name;
        SynchronizeState();

        await SelectedItemChanged.InvokeAsync( name );
        await InvokeAsync( StateHasChanged );
    }

    private void SynchronizeState()
    {
        var nextState = new RibbonBackstageState
        {
            Visible = EffectiveVisible,
            SelectedItem = EffectiveSelectedItem,
            RenderMode = RenderMode,
        };

        if ( State != nextState )
        {
            State = nextState;
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    internal bool EffectiveVisible => ParentWorkspace?.BackstageVisible ?? Visible;

    /// <summary>
    /// Resolves positioning for the local or viewport overlay.
    /// </summary>
    protected IFluentPosition EffectivePosition
        => Position ?? ( Fullscreen
            ? Blazorise.Position.Fixed.Top.Is0.Start.Is0
            : Blazorise.Position.Absolute.Top.Is0.Start.Is0 );

    /// <summary>
    /// Shares backstage visibility, page selection, and rendering policy with its navigation entries.
    /// </summary>
    protected RibbonBackstageState State { get; private set; } = new();

    /// <summary>
    /// Orders navigation entries, preserving registration order for equal values.
    /// </summary>
    protected IEnumerable<RibbonBackstageItem> OrderedItems => items.OrderBy( item => item.Order );

    /// <summary>
    /// Resolves the selected page, falling back to the first enabled, visible entry with page content.
    /// </summary>
    protected string EffectiveSelectedItem
        => ( items.FirstOrDefault( item => item.Name == SelectedItem && item.CanSelect )
            ?? OrderedItems.FirstOrDefault( item => item.CanSelect ) )?.Name;

    /// <summary>
    /// Identifies the Back button that closes backstage.
    /// </summary>
    protected string BackElementId => $"{ElementId}-back";

    /// <summary>
    /// Controls whether backstage is shown.
    /// </summary>
    /// <remarks>
    /// Opening backstage moves keyboard focus to the Back button. Defaults to <c>false</c>.
    /// Inside a <see cref="RibbonWorkspace"/>, use <see cref="RibbonWorkspace.BackstageVisible"/> instead.
    /// </remarks>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Occurs when standalone backstage changes visibility through its controls or public methods.
    /// Inside a workspace, visibility changes are reported through <see cref="RibbonWorkspace.BackstageVisibleChanged"/>.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Covers the viewport instead of the containing workspace or positioned ancestor. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Fullscreen { get; set; }

    /// <summary>
    /// Specifies the <see cref="RibbonBackstageItem.Name"/> of the selected page.
    /// </summary>
    /// <remarks>
    /// If unavailable, the first enabled, visible entry with page content is selected.
    /// </remarks>
    [Parameter] public string SelectedItem { get; set; }

    /// <summary>
    /// Occurs when a different backstage page is selected. Supplies the page name for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedItemChanged { get; set; }

    /// <summary>
    /// Occurs after backstage is shown and the Back button receives focus.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Occurs after backstage is hidden. Use this callback to focus an application control when needed.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// Controls when backstage page content is created and retained.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="TabsRenderMode.LazyLoad"/>, which preserves visited pages.
    /// Use <see cref="TabsRenderMode.Default"/> to render all pages or <see cref="TabsRenderMode.LazyReload"/> to retain only the selected page content.
    /// </remarks>
    [Parameter] public TabsRenderMode RenderMode { get; set; } = TabsRenderMode.LazyLoad;

    /// <summary>
    /// Specifies the accessible name announced for the backstage surface. Defaults to <c>File backstage</c>.
    /// </summary>
    [Parameter] public string Label { get; set; } = "File backstage";

    /// <summary>
    /// Specifies the accessible name announced for backstage navigation. Defaults to <c>File navigation</c>.
    /// </summary>
    [Parameter] public string NavigationLabel { get; set; } = "File navigation";

    /// <summary>
    /// Specifies the Back button text and accessible name. Defaults to <c>Back to document</c>.
    /// </summary>
    [Parameter] public string BackText { get; set; } = "Back to document";

    /// <summary>
    /// Defines backstage pages and commands using <see cref="RibbonBackstageItem"/> components.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets the optional workspace that owns backstage visibility.
    /// </summary>
    [CascadingParameter] protected RibbonWorkspace ParentWorkspace { get; set; }

    /// <summary>
    /// Receives workspace state changes so the backstage surface and its pages render together.
    /// </summary>
    [CascadingParameter] protected RibbonWorkspaceState ParentWorkspaceState { get; set; }

    #endregion
}