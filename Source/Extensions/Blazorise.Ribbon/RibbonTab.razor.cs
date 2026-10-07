#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Declares a ribbon heading and its command groups together.
/// </summary>
public partial class RibbonTab : BaseComponent, IDisposable
{
    #region Events

    internal event Action HeadingChanged;

    #endregion

    #region Members

    private bool hasBeenSelected;

    private Div containerRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( ParentRibbon is null )
        {
            throw new InvalidOperationException( "RibbonTab must be placed inside a Ribbon." );
        }

        ParentRibbon.RegisterTab( this );
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var headingChanged = parameters.IsParameterChanged( Text );
        var nameChanged = parameters.IsParameterChanged( Name );
        var orderChanged = parameters.IsParameterChanged( Order );
        var disabledChanged = parameters.IsParameterChanged( Disabled );
        var visibleChanged = parameters.IsParameterChanged( Visible );
        var contextualTabsChanged = parameters.IsParameterChanged( ParentContextualTabsState );
        var elementIdChanged = parameters.IsParameterChanged( ElementId );

        var headerChanged = parameters.TryGetValue<RenderFragment>( nameof( HeaderContent ), out var paramHeaderContent )
            && !Equals( paramHeaderContent, HeaderContent );

        await base.SetParametersAsync( parameters );

        ParentRibbon.ValidateTab( this );

        if ( headingChanged || nameChanged || orderChanged || disabledChanged || visibleChanged || contextualTabsChanged )
        {
            ParentRibbon.Refresh();
        }

        if ( headingChanged || nameChanged || disabledChanged || contextualTabsChanged || elementIdChanged || headerChanged )
        {
            HeadingChanged?.Invoke();
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        hasBeenSelected |= IsActive;

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = containerRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentRibbon?.UnregisterTab( this );
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Indicates whether the tab is available after applying its visibility and contextual group state.
    /// </summary>
    internal bool IsVisible => Visible && ( ParentContextualTabsState?.IsActive ?? true );

    /// <summary>
    /// Indicates whether a contextual group owns the tab's appearance and availability.
    /// </summary>
    internal bool IsContextual => ParentContextualTabsState is not null;

    /// <summary>
    /// Resolves the contextual group's accent color, using the primary color for regular tabs.
    /// </summary>
    internal Color EffectiveColor => ParentContextualTabsState?.Color ?? Blazorise.Color.Primary;

    /// <summary>
    /// Indicates whether the tab is available and enabled for selection.
    /// </summary>
    internal bool CanSelect => IsVisible && !Disabled;

    /// <summary>
    /// Indicates whether this enabled, visible tab is the ribbon's selected tab.
    /// </summary>
    protected bool IsActive => CanSelect && ParentRibbonState?.SelectedTab == Name;

    /// <summary>
    /// Determines whether to create or retain the command panel according to the ribbon's rendering policy and this tab's selection history.
    /// </summary>
    protected bool ShouldRenderContent => ParentRibbonState?.RenderMode switch
    {
        TabsRenderMode.LazyLoad => hasBeenSelected,
        TabsRenderMode.LazyReload => IsActive,
        _ => true,
    };

    /// <summary>
    /// Identifies the heading that labels this tab's command panel.
    /// </summary>
    internal string TabElementId => $"{ElementId}-tab";

    /// <summary>
    /// Serializes whether the command panel is hidden for its aria-hidden attribute.
    /// </summary>
    protected string HiddenString => IsActive ? "false" : "true";

    /// <summary>
    /// Identifies the tab for selection and binding through <see cref="Ribbon.SelectedTab"/>.
    /// </summary>
    /// <remarks>
    /// Must be non-empty and unique within the containing ribbon.
    /// </remarks>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Controls heading order, fallback selection, and animation direction. Lower values appear first. Defaults to zero.
    /// </summary>
    /// <remarks>
    /// Applies across regular and contextual tabs. Equal values retain registration order.
    /// Bind this parameter to the current index when inserting or reordering dynamic tabs.
    /// </remarks>
    [Parameter] public int Order { get; set; }

    /// <summary>
    /// Specifies the tab heading displayed when <see cref="HeaderContent"/> is not supplied.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Prevents selecting the tab while keeping its heading visible. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Controls whether the tab heading is shown and the tab can be selected.
    /// </summary>
    /// <remarks>
    /// Contextual groups also control whether their member tabs are available. Defaults to <c>true</c>.
    /// </remarks>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines custom content for the tab heading, replacing <see cref="Text"/>.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }

    /// <summary>
    /// Defines the command panel displayed when this tab is selected, typically containing <see cref="RibbonGroup"/> components.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Provides the containing ribbon for tab registration and selection.
    /// </summary>
    [CascadingParameter] internal Ribbon ParentRibbon { get; set; }

    /// <summary>
    /// Shares the containing ribbon's selection, display mode, and rendering policy with this component.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    /// <summary>
    /// Shares the owning contextual group's availability and accent color, when the tab belongs to a group.
    /// </summary>
    [CascadingParameter] internal RibbonContextualTabsState ParentContextualTabsState { get; set; }

    #endregion
}