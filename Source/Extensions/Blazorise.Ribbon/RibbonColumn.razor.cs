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
    /// Gets the column layout after resolving the display mode.
    /// </summary>
    protected IFluentFlex EffectiveFlex => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified ? Blazorise.Flex.Row.NoWrap.AlignItems.Center.Shrink.Is0 : Blazorise.Flex.Column.JustifyContent.Center.Shrink.Is0;

    /// <summary>
    /// Gets or sets the commands in this column.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets the containing ribbon state.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }
}