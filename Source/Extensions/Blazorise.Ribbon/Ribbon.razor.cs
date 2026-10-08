#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Animate;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Ribbon.Extensions;
using Blazorise.Ribbon.Utilities;
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

    private readonly List<RibbonContextualTabs> contextualTabsItems = new();

    private IReadOnlyList<string> previousActiveContextualGroups = Array.Empty<string>();

    private string lastRegularTab;

    private Div containerRef;

    private Blazorise.Animate.Animate contentAnimationRef;

    private string previousSelectedTab;

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
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChangedHandler;

        base.OnInitialized();
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

        var shouldAnimateSelection = !firstRender
            && EffectiveAnimationDuration > 0
            && !Collapsed
            && previousSelectedTab is not null
            && State.SelectedTab is not null
            && tabItems.Any( tab => tab.Name == previousSelectedTab )
            && !string.Equals( previousSelectedTab, State.SelectedTab, StringComparison.Ordinal );

        previousSelectedTab = State.SelectedTab;

        if ( shouldAnimateSelection )
        {
            contentAnimationRef?.Run();
        }

        var contextualTabToSelect = OrderedTabs.FirstOrDefault( tab => tab.CanSelect
            && contextualTabsItems.Any( group => group.SelectOnShow
                && tab.ParentContextualTabsState?.Name == group.Name
                && State.ActiveContextualGroups.Contains( group.Name, StringComparer.Ordinal )
                && !previousActiveContextualGroups.Contains( group.Name, StringComparer.Ordinal ) ) );

        previousActiveContextualGroups = State.ActiveContextualGroups
            .Where( name => contextualTabsItems.Any( group => string.Equals( group.Name, name, StringComparison.Ordinal ) ) )
            .ToArray();

        await HandleSelectTab( contextualTabToSelect?.Name ?? EffectiveSelectedTab );

        if ( contextualTabToSelect is not null )
        {
            await SetCollapsed( false );
        }

        await JSModule.Initialize( ElementRef, ElementId );

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChangedHandler;

            if ( Rendered )
            {
                await JSModule.SafeDestroy( ElementRef, ElementId );
            }
        }

        await base.DisposeAsync( disposing );
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
    /// Activates the named contextual groups and hides the others.
    /// </summary>
    /// <param name="names">
    /// Names of the contexts to activate. Pass no names to hide all contextual groups.
    /// </param>
    /// <remarks>
    /// Updates <see cref="ActiveContextualGroups"/> and invokes <see cref="ActiveContextualGroupsChanged"/> so bound application state stays synchronized.
    /// </remarks>
    public async Task SetActiveContextualGroups( params string[] names )
    {
        var activeContextualGroups = ResolveActiveContextualGroups( names );

        if ( State.ActiveContextualGroups.SequenceEqual( activeContextualGroups, StringComparer.Ordinal ) )
        {
            return;
        }

        ActiveContextualGroups = activeContextualGroups;
        SynchronizeState();

        await ActiveContextualGroupsChanged.InvokeAsync( ActiveContextualGroups );
        await InvokeAsync( StateHasChanged );
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
    /// Refreshes built-in labels after the application's language changes.
    /// </summary>
    private void OnLocalizationChangedHandler( object sender, EventArgs eventArgs )
    {
        _ = InvokeAsync( StateHasChanged );
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

    internal void RegisterContextualTabs( RibbonContextualTabs group )
    {
        ValidateContextualTabs( group );

        contextualTabsItems.Add( group );
        Refresh();
    }

    internal void ValidateContextualTabs( RibbonContextualTabs group )
    {
        if ( string.IsNullOrWhiteSpace( group.Name ) )
        {
            throw new InvalidOperationException( "RibbonContextualTabs requires a non-empty Name." );
        }

        if ( contextualTabsItems.Any( item => !ReferenceEquals( item, group ) && string.Equals( item.Name, group.Name, StringComparison.Ordinal ) ) )
        {
            throw new InvalidOperationException( $"Ribbon contextual group names must be unique. The name '{group.Name}' is already registered." );
        }
    }

    internal void UnregisterContextualTabs( RibbonContextualTabs group )
    {
        contextualTabsItems.Remove( group );
        Refresh();
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

    private void SynchronizeState()
    {
        var regularTab = tabItems.FirstOrDefault( tab => tab.Name == SelectedTab && tab.CanSelect && !tab.IsContextual );

        if ( regularTab is not null )
        {
            lastRegularTab = regularTab.Name;
        }

        var activeContextualGroups = ResolveActiveContextualGroups( ActiveContextualGroups );

        var nextState = new RibbonState
        {
            SelectedTab = EffectiveSelectedTab,
            ActiveContextualGroups = State.ActiveContextualGroups.SequenceEqual( activeContextualGroups, StringComparer.Ordinal )
                ? State.ActiveContextualGroups
                : activeContextualGroups,
            Collapsed = Collapsed,
            DisplayMode = DisplayMode,
            RenderMode = RenderMode,
        };

        if ( State != nextState )
        {
            State = nextState;
        }
    }

    private static IReadOnlyList<string> ResolveActiveContextualGroups( IEnumerable<string> names )
        => ( names ?? Array.Empty<string>() )
            .Where( name => !string.IsNullOrWhiteSpace( name ) )
            .Distinct( StringComparer.Ordinal )
            .ToArray();

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Shares the current selection, collapse state, and command layout with ribbon descendants.
    /// </summary>
    protected RibbonState State { get; private set; } = new();

    /// <summary>
    /// Orders headings, preserving registration order for equal values.
    /// </summary>
    protected IEnumerable<RibbonTab> OrderedTabs => tabItems.OrderBy( tab => tab.Order );

    /// <summary>
    /// Resolves the selected tab, restoring the last available regular tab before falling back to the first enabled, visible tab.
    /// </summary>
    protected string EffectiveSelectedTab
        => ( tabItems.FirstOrDefault( tab => tab.Name == SelectedTab && tab.CanSelect )
            ?? tabItems.FirstOrDefault( tab => tab.Name == lastRegularTab && tab.CanSelect && !tab.IsContextual )
            ?? OrderedTabs.FirstOrDefault( tab => tab.CanSelect ) )?.Name;

    /// <summary>
    /// Selects the slide direction from the new tab's position relative to the previously displayed tab.
    /// </summary>
    protected IAnimation EffectiveTabAnimation
    {
        get
        {
            var orderedTabs = OrderedTabs.ToList();

            return orderedTabs.FindIndex( tab => tab.Name == State.SelectedTab ) > orderedTabs.FindIndex( tab => tab.Name == previousSelectedTab )
                ? Animations.SlideLeft
                : Animations.SlideRight;
        }
    }

    /// <summary>
    /// Resolves the transition duration, returning zero when animation is disabled or <see cref="AnimationDuration"/> is negative.
    /// </summary>
    protected int EffectiveAnimationDuration => Animated ? Math.Max( 0, AnimationDuration ) : 0;

    /// <summary>
    /// Identifies the command panel container referenced by the collapse button.
    /// </summary>
    protected string ContentElementId => $"{ElementId}-content";

    /// <summary>
    /// Serializes the command surface's expanded state for the collapse button's aria-expanded attribute.
    /// </summary>
    protected string ExpandedString => Collapsed ? "false" : "true";

    /// <summary>
    /// Selects the accessible label for the action that the collapse button will perform.
    /// </summary>
    protected string EffectiveCollapseButtonLabel => Collapsed ? EffectiveExpandLabel : EffectiveCollapseLabel;

    /// <summary>
    /// Resolves the tab strip's accessible name from an explicit label or its translation.
    /// </summary>
    protected string EffectiveLabel => Label ?? Localizer.Localize( Localizers?.LabelLocalizer, LocalizationConstants.RibbonTabs );

    /// <summary>
    /// Resolves the quick access toolbar's accessible name.
    /// </summary>
    protected string EffectiveQuickAccessLabel => QuickAccessLabel ?? Localizer.Localize( Localizers?.QuickAccessLocalizer, LocalizationConstants.QuickAccess );

    /// <summary>
    /// Resolves the accessible name of the action that expands the ribbon.
    /// </summary>
    protected string EffectiveExpandLabel => ExpandLabel ?? Localizer.Localize( Localizers?.ExpandLocalizer, LocalizationConstants.Expand );

    /// <summary>
    /// Resolves the accessible name of the action that collapses the ribbon.
    /// </summary>
    protected string EffectiveCollapseLabel => CollapseLabel ?? Localizer.Localize( Localizers?.CollapseLocalizer, LocalizationConstants.Collapse );

    /// <summary>
    /// Supplies embedded translations for built-in component labels.
    /// </summary>
    [Inject] protected ITextLocalizer<Ribbon> Localizer { get; set; }

    /// <summary>
    /// Notifies the component when the application's language changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Provides keyboard navigation and focus management for the ribbon tab strip.
    /// </summary>
    [Inject] protected IJSTabsModule JSModule { get; set; }

    /// <summary>
    /// Specifies the <see cref="RibbonTab.Name"/> of the selected tab.
    /// </summary>
    /// <remarks>
    /// If unavailable, the last available regular tab is restored, or the first enabled, visible tab is selected.
    /// </remarks>
    [Parameter] public string SelectedTab { get; set; }

    /// <summary>
    /// Occurs when the selected tab changes.
    /// </summary>
    [Parameter] public EventCallback<string> SelectedTabChanged { get; set; }

    /// <summary>
    /// Identifies the contextual groups whose tabs are currently available.
    /// </summary>
    /// <remarks>
    /// Bind this collection to the application's current editing context. Multiple groups may be active together.
    /// Defaults to an empty collection, leaving all contextual groups hidden.
    /// </remarks>
    [Parameter] public IReadOnlyList<string> ActiveContextualGroups { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Occurs when an imperative context change updates the active group names.
    /// </summary>
    [Parameter] public EventCallback<IReadOnlyList<string>> ActiveContextualGroupsChanged { get; set; }

    /// <summary>
    /// Hides the command panels while keeping the tab strip available.
    /// </summary>
    /// <remarks>
    /// Selecting a tab expands the ribbon. Defaults to <c>false</c>.
    /// </remarks>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>
    /// Occurs when the collapsed state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> CollapsedChanged { get; set; }

    /// <summary>
    /// Displays a button for collapsing or expanding the command panels. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowCollapseButton { get; set; } = true;

    /// <summary>
    /// Controls whether commands use the full group layout or a single compact row.
    /// Defaults to <see cref="RibbonDisplayMode.Classic"/>.
    /// </summary>
    [Parameter] public RibbonDisplayMode DisplayMode { get; set; }

    /// <summary>
    /// Controls when tab content is created and whether it is retained after switching tabs.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="TabsRenderMode.Default"/>, which renders all panels.
    /// Use <see cref="TabsRenderMode.LazyLoad"/> to retain visited panels or <see cref="TabsRenderMode.LazyReload"/> to render only the active panel.
    /// </remarks>
    [Parameter] public TabsRenderMode RenderMode { get; set; }

    /// <summary>
    /// Enables slide transitions when switching tabs.
    /// </summary>
    /// <remarks>
    /// The direction follows the new tab's position relative to the current tab.
    /// The initial render is not animated. Defaults to <c>false</c>.
    /// </remarks>
    [Parameter] public bool Animated { get; set; }

    /// <summary>
    /// Controls the tab transition duration in milliseconds.
    /// </summary>
    /// <remarks>
    /// Applies when <see cref="Animated"/> is enabled. Zero or a negative value disables transitions.
    /// Defaults to <c>200</c>.
    /// </remarks>
    [Parameter] public int AnimationDuration { get; set; } = 200;

    /// <summary>
    /// Specifies the accessible name announced for the tab strip.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Specifies the accessible name announced for the quick access commands.
    /// </summary>
    [Parameter] public string QuickAccessLabel { get; set; }

    /// <summary>
    /// Specifies the accessible label of the collapse button when it will expand the ribbon.
    /// </summary>
    [Parameter] public string ExpandLabel { get; set; }

    /// <summary>
    /// Specifies the accessible label of the collapse button when it will collapse the ribbon.
    /// </summary>
    [Parameter] public string CollapseLabel { get; set; }

    /// <summary>
    /// Overrides translations for built-in ribbon labels without changing application command text.
    /// </summary>
    [Parameter] public RibbonLocalizers Localizers { get; set; }

    /// <summary>
    /// Defines frequently used commands displayed above the tab strip, such as Save, Undo, and Redo.
    /// </summary>
    [Parameter] public RenderFragment QuickAccessContent { get; set; }

    /// <summary>
    /// Defines the application entry preceding the tab headings, such as File.
    /// </summary>
    /// <remarks>
    /// Use <see cref="RibbonApplicationMenu"/> for a dropdown menu or <see cref="RibbonApplicationButton"/> to open backstage or invoke a command.
    /// </remarks>
    [Parameter] public RenderFragment ApplicationTab { get; set; }

    /// <summary>
    /// Defines commands displayed after the tab headings, such as Share or Comments.
    /// </summary>
    [Parameter] public RenderFragment TabStripToolbar { get; set; }

    /// <summary>
    /// Defines ordinary <see cref="RibbonTab"/> components and named <see cref="RibbonContextualTabs"/> collections.
    /// </summary>
    [Parameter] public RenderFragment RibbonTabs { get; set; }

    #endregion
}