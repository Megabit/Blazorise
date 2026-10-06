#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Groups ribbon tabs that share an editing context and accent color.
/// </summary>
public partial class RibbonContextualTabs : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( ParentRibbon is null )
        {
            throw new InvalidOperationException( "RibbonContextualTabs must be placed inside a Ribbon." );
        }

        if ( ParentContextualTabsState is not null )
        {
            throw new InvalidOperationException( "RibbonContextualTabs cannot be nested inside another contextual group." );
        }

        ParentRibbon.RegisterContextualTabs( this );
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var nameChanged = parameters.IsParameterChanged( Name );

        await base.SetParametersAsync( parameters );

        ParentRibbon.ValidateContextualTabs( this );

        if ( nameChanged )
        {
            ParentRibbon.Refresh();
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        var nextState = new RibbonContextualTabsState
        {
            Name = Name,
            Color = Color ?? Blazorise.Color.Primary,
            IsActive = ParentRibbonState?.ActiveContextualGroups.Contains( Name, StringComparer.Ordinal ) == true,
        };

        if ( State != nextState )
        {
            State = nextState;
        }

        base.OnParametersSet();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentRibbon?.UnregisterContextualTabs( this );
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Shares the group's identity, availability, and accent color with its descendant tabs.
    /// </summary>
    internal RibbonContextualTabsState State { get; private set; } = new();

    /// <summary>
    /// Identifies the context activated through <see cref="Ribbon.ActiveContextualGroups"/>.
    /// </summary>
    /// <remarks>
    /// Must be non-empty and unique within the containing ribbon.
    /// </remarks>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Specifies the shared color of the contextual tab headings and selected underline.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Blazorise.Color.Primary"/>.
    /// </remarks>
    [Parameter] public Color Color { get; set; } = Blazorise.Color.Primary;

    /// <summary>
    /// Selects the group's first enabled, visible tab when the context becomes active.
    /// </summary>
    /// <remarks>
    /// Selecting the tab also expands the ribbon. Defaults to <c>false</c>.
    /// </remarks>
    [Parameter] public bool SelectOnShow { get; set; }

    /// <summary>
    /// Defines the tabs available while this context is active.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Provides the containing ribbon for contextual group registration.
    /// </summary>
    [CascadingParameter] internal Ribbon ParentRibbon { get; set; }

    /// <summary>
    /// Shares the active editing contexts used to determine whether the group's tabs are available.
    /// </summary>
    [CascadingParameter] internal RibbonState ParentRibbonState { get; set; }

    /// <summary>
    /// Identifies an enclosing contextual group so unsupported group nesting can be rejected.
    /// </summary>
    [CascadingParameter] internal RibbonContextualTabsState ParentContextualTabsState { get; set; }

    #endregion
}