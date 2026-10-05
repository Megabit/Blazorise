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
        var elementIdChanged = parameters.IsParameterChanged( ElementId );

        var headerChanged = parameters.TryGetValue<RenderFragment>( nameof( HeaderContent ), out var paramHeaderContent )
            && !Equals( paramHeaderContent, HeaderContent );

        await base.SetParametersAsync( parameters );

        ParentRibbon.ValidateTab( this );

        if ( headingChanged || nameChanged || disabledChanged || visibleChanged )
        {
            ParentRibbon.Refresh();
        }

        if ( headingChanged || nameChanged || disabledChanged || elementIdChanged || headerChanged )
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

    internal bool CanSelect => Visible && !Disabled;

    /// <summary>
    /// Gets whether this tab owns the selected command panel.
    /// </summary>
    protected bool IsActive => CanSelect && ParentRibbonState?.SelectedTab == Name;

    /// <summary>
    /// Gets the panel's hidden state for accessibility markup.
    /// </summary>
    protected string HiddenString => IsActive ? "false" : "true";

    /// <summary>
    /// Gets whether to instantiate panel content under the current render policy.
    /// </summary>
    protected bool ShouldRenderContent => ParentRibbonState?.RenderMode switch
    {
        TabsRenderMode.LazyLoad => hasBeenSelected,
        TabsRenderMode.LazyReload => IsActive,
        _ => true,
    };

    /// <summary>
    /// Gets or sets the unique tab name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Gets or sets the tab heading.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets whether selection is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the tab is available, including contextual tabs.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets custom heading content.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }

    /// <summary>
    /// Gets or sets the command groups belonging to the tab.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] internal Ribbon ParentRibbon { get; set; }

    /// <summary>
    /// Gets the containing ribbon state.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}