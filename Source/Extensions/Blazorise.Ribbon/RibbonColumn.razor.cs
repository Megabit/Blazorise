#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Stacks commands beside full-height items and lays them out horizontally in simplified mode.
/// </summary>
public partial class RibbonColumn : BaseComponent
{
    /// <summary>
    /// Initializes the default column spacing.
    /// </summary>
    public RibbonColumn()
    {
        Gap = Blazorise.Gap.Is1;
    }

    /// <summary>
    /// Stacks commands vertically in classic mode and arranges them horizontally in simplified mode.
    /// </summary>
    protected IFluentFlex EffectiveFlex => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified ? Blazorise.Flex.Row.NoWrap.AlignItems.Center.Shrink.Is0 : Blazorise.Flex.Column.JustifyContent.Center.Shrink.Is0;

    /// <summary>
    /// Defines commands stacked beside large commands in classic mode. These commands form a horizontal row in simplified mode.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Shares the containing ribbon's selection, display mode, and rendering policy with this component.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }
}