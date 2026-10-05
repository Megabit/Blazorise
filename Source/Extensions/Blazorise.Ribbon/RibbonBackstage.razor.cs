#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays application-level file commands and pages inside the document workspace.
/// </summary>
public partial class RibbonBackstage : BaseComponent
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
        Height = Blazorise.Height.Rem( 32 );
        Width = Blazorise.Width.Px().Min( 0 );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        SynchronizeState();

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        ElementRef = containerRef.ElementRef;

        var visibilityChanged = Visible != wasVisible;
        var isVisible = Visible;
        wasVisible = isVisible;

        await base.OnAfterRenderAsync( firstRender );
        await HandleSelectItem( EffectiveSelectedItem );

        if ( !visibilityChanged || Visible != isVisible || Disposed || AsyncDisposed )
        {
            return;
        }

        if ( isVisible )
        {
            await backButtonRef.Focus( false );

            if ( Visible && !Disposed && !AsyncDisposed )
            {
                await Opened.InvokeAsync();
            }
        }
        else
        {
            await Closed.InvokeAsync();
        }
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
        => Visible && eventArgs.Key == "Escape" ? Hide() : Task.CompletedTask;

    /// <summary>
    /// Applies visibility and awaits the application's binding callback.
    /// </summary>
    protected virtual async Task HandleVisible( bool visible )
    {
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
            Visible = Visible,
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

    /// <summary>
    /// Gets the state shared with backstage items.
    /// </summary>
    protected RibbonBackstageState State { get; private set; } = new();

    /// <summary>
    /// Gets the requested page or the first available page.
    /// </summary>
    protected string EffectiveSelectedItem
        => ( items.FirstOrDefault( item => item.Name == SelectedItem && item.CanSelect )
            ?? items.FirstOrDefault( item => item.CanSelect ) )?.Name;

    /// <summary>
    /// Gets the Back button identifier.
    /// </summary>
    protected string BackElementId => $"{ElementId}-back";

    /// <summary>
    /// Gets or sets whether backstage is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Occurs when backstage visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Gets or sets the selected page name.
    /// </summary>
    [Parameter] public string SelectedItem { get; set; }

    /// <summary>
    /// Occurs when the selected page changes.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedItemChanged { get; set; }

    /// <summary>
    /// Occurs after backstage is shown and the Back button receives focus.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Occurs after backstage is hidden. Use this callback to restore focus to its opener.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// Gets or sets the page rendering policy. Visited pages are preserved by default.
    /// </summary>
    [Parameter] public TabsRenderMode RenderMode { get; set; } = TabsRenderMode.LazyLoad;

    /// <summary>
    /// Gets or sets the accessible name of the backstage surface.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "File backstage";

    /// <summary>
    /// Gets or sets the accessible name of the navigation area.
    /// </summary>
    [Parameter] public string NavigationLabel { get; set; } = "File navigation";

    /// <summary>
    /// Gets or sets the Back button text.
    /// </summary>
    [Parameter] public string BackLabel { get; set; } = "Back to document";

    /// <summary>
    /// Gets or sets the nested backstage page and command declarations.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}