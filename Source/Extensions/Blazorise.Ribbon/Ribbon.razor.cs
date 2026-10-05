#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Organizes application commands into named tabs and labeled groups.
/// </summary>
public partial class Ribbon : BaseComponent, IAsyncDisposable
{
    #region Members

    private readonly List<RibbonTab> tabItems = new();

    private Div containerRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the default ribbon width constraint.
    /// </summary>
    public Ribbon()
    {
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

        await HandleSelectTab( EffectiveSelectedTab );

        await JSModule.Initialize( ElementRef, ElementId );

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );
        }

        await base.DisposeAsync( disposing );
    }

    internal void RegisterTab( RibbonTab tab )
    {
        ValidateTab( tab );

        tabItems.Add( tab );
        Refresh();
    }

    internal void ValidateTab( RibbonTab tab )
    {
        if ( string.IsNullOrWhiteSpace( tab.Name ) )
        {
            throw new InvalidOperationException( "RibbonTab requires a non-empty Name." );
        }

        if ( tabItems.Any( item => !ReferenceEquals( item, tab ) && item.Name == tab.Name ) )
        {
            throw new InvalidOperationException( $"Ribbon tab names must be unique. The name '{tab.Name}' is already registered." );
        }
    }

    internal void UnregisterTab( RibbonTab tab )
    {
        tabItems.Remove( tab );
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
    /// Selects an enabled, visible tab and expands the command surface.
    /// </summary>
    /// <param name="name">
    /// Name of the tab to select.
    /// </param>
    public async Task SelectTab( string name )
    {
        if ( !tabItems.Any( tab => tab.Name == name && tab.CanSelect ) )
        {
            return;
        }

        await HandleSelectTab( name );
        await SetCollapsed( false );
    }

    /// <summary>
    /// Sets the collapsed state and notifies the bound application.
    /// </summary>
    /// <param name="collapsed">
    /// Whether to hide the command surface.
    /// </param>
    public async Task SetCollapsed( bool collapsed )
    {
        if ( Collapsed == collapsed )
        {
            return;
        }

        Collapsed = collapsed;
        SynchronizeState();

        await CollapsedChanged.InvokeAsync( collapsed );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Handles activation of the collapse toggle.
    /// </summary>
    protected Task OnCollapseHandler() => SetCollapsed( !Collapsed );

    /// <summary>
    /// Applies selection through the common notification path.
    /// </summary>
    protected virtual async Task HandleSelectTab( string name )
    {
        if ( string.Equals( SelectedTab, name, StringComparison.Ordinal ) )
        {
            return;
        }

        SelectedTab = name;
        SynchronizeState();

        await SelectedTabChanged.InvokeAsync( name );
        await InvokeAsync( StateHasChanged );
    }

    private void SynchronizeState()
    {
        var nextState = new RibbonState
        {
            SelectedTab = EffectiveSelectedTab,
            Collapsed = Collapsed,
            DisplayMode = DisplayMode,
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
    /// Gets the state shared with descendants.
    /// </summary>
    protected RibbonState State { get; private set; } = new();

    /// <summary>
    /// Gets the requested tab or the first available fallback.
    /// </summary>
    protected string EffectiveSelectedTab
        => ( tabItems.FirstOrDefault( tab => tab.Name == SelectedTab && tab.CanSelect )
            ?? tabItems.FirstOrDefault( tab => tab.CanSelect ) )?.Name;

    /// <summary>
    /// Gets the command surface element identifier.
    /// </summary>
    protected string ContentElementId => $"{ElementId}-content";

    /// <summary>
    /// Gets the expanded state for accessibility markup.
    /// </summary>
    protected string ExpandedString => Collapsed ? "false" : "true";

    /// <summary>
    /// Gets the label of the current collapse or expand action.
    /// </summary>
    protected string CollapseButtonLabel => Collapsed ? ExpandLabel : CollapseLabel;

    /// <summary>
    /// Gets the existing provider-independent tab keyboard module.
    /// </summary>
    [Inject] protected IJSTabsModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the selected tab name.
    /// </summary>
    [Parameter] public string SelectedTab { get; set; }

    /// <summary>
    /// Occurs when the selected tab changes.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedTabChanged { get; set; }

    /// <summary>
    /// Gets or sets whether the command surface is hidden.
    /// </summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>
    /// Occurs when the collapsed state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> CollapsedChanged { get; set; }

    /// <summary>
    /// Gets or sets whether to show the collapse button.
    /// </summary>
    [Parameter] public bool ShowCollapseButton { get; set; } = true;

    /// <summary>
    /// Gets or sets the command layout.
    /// </summary>
    [Parameter] public RibbonDisplayMode DisplayMode { get; set; }

    /// <summary>
    /// Gets or sets the panel rendering policy. The default preserves panel content.
    /// </summary>
    [Parameter] public TabsRenderMode RenderMode { get; set; }

    /// <summary>
    /// Gets or sets the accessible name of the tab strip.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "Ribbon tabs";

    /// <summary>
    /// Gets or sets the accessible name of the quick access area.
    /// </summary>
    [Parameter] public string QuickAccessLabel { get; set; } = "Quick access";

    /// <summary>
    /// Gets or sets the expand action label.
    /// </summary>
    [Parameter] public string ExpandLabel { get; set; } = "Expand ribbon";

    /// <summary>
    /// Gets or sets the collapse action label.
    /// </summary>
    [Parameter] public string CollapseLabel { get; set; } = "Collapse ribbon";

    /// <summary>
    /// Gets or sets quick access commands.
    /// </summary>
    [Parameter] public RenderFragment QuickAccessContent { get; set; }

    /// <summary>
    /// Gets or sets the application menu or button preceding the tabs, such as File.
    /// </summary>
    [Parameter] public RenderFragment RibbonApplicationTab { get; set; }

    /// <summary>
    /// Gets or sets actions following the tab strip, such as Share.
    /// </summary>
    [Parameter] public RenderFragment TabStripContent { get; set; }

    /// <summary>
    /// Gets or sets the nested ribbon tab declarations.
    /// </summary>
    [Parameter] public RenderFragment RibbonTabs { get; set; }

    #endregion
}