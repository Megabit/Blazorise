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
public partial class RibbonTab : BaseComponent
{
    #region Events

    internal event Action HeadingChanged;

    #endregion

    #region Members

    private bool hasBeenSelected;

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
        var disabledChanged = parameters.IsParameterChanged( Disabled );
        var visibleChanged = parameters.IsParameterChanged( Visible );
        var contextualTabsChanged = parameters.IsParameterChanged( ParentContextualTabsState );
        var elementIdChanged = parameters.IsParameterChanged( ElementId );

        var headerChanged = parameters.TryGetValue<RenderFragment>( nameof( HeaderContent ), out var paramHeaderContent )
            && !Equals( paramHeaderContent, HeaderContent );

        await base.SetParametersAsync( parameters );

        ParentRibbon.ValidateTab( this );

        if ( headingChanged || nameChanged || disabledChanged || visibleChanged || contextualTabsChanged )
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

    internal string TabElementId => $"{ElementId}-tab";

    internal bool IsVisible => Visible && ( ParentContextualTabsState?.IsActive ?? true );

    internal bool IsContextual => ParentContextualTabsState is not null;

    internal Color EffectiveColor => ParentContextualTabsState?.Color ?? Blazorise.Color.Primary;

    internal bool CanSelect => IsVisible && !Disabled;

    /// <summary>
    /// Indicates whether this enabled, visible tab is the ribbon's selected tab.
    /// </summary>
    protected bool IsActive => CanSelect && ParentRibbonState?.SelectedTab == Name;

    /// <summary>
    /// Serializes whether the command panel is hidden for its aria-hidden attribute.
    /// </summary>
    protected string HiddenString => IsActive ? "false" : "true";

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
    /// Identifies the tab for selection and binding through <see cref="Ribbon.SelectedTab"/>.
    /// </summary>
    /// <remarks>
    /// Must be non-empty and unique within the containing ribbon.
    /// </remarks>
    [Parameter] public string Name { get; set; }

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

    [CascadingParameter] internal Ribbon ParentRibbon { get; set; }

    [CascadingParameter] internal RibbonContextualTabsState ParentContextualTabsState { get; set; }

    /// <summary>
    /// Shares the containing ribbon's selection, display mode, and rendering policy with this component.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}