#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Arranges commands and arbitrary Blazorise inputs horizontally within a rows group.
/// </summary>
public partial class RibbonRow : BaseComponent
{
    /// <summary>
    /// Initializes the default command row alignment and spacing.
    /// </summary>
    public RibbonRow()
    {
        Gap = Blazorise.Gap.Is1;
        Height = Blazorise.Height.Px().Min( 0 );
    }

    /// <summary>
    /// Gets the row layout after resolving the display mode.
    /// </summary>
    protected IFluentFlex EffectiveFlex => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified ? Blazorise.Flex.Row.NoWrap.AlignItems.Center.Shrink.Is0 : Blazorise.Flex.Row.NoWrap.AlignItems.Center.Grow.Is1.Basis.Is0;

    /// <summary>
    /// Gets or sets row content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets the containing ribbon state.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }
}