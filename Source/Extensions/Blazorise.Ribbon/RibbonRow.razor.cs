#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Arranges commands and arbitrary Blazorise inputs horizontally within a rows group.
/// </summary>
public partial class RibbonRow : BaseComponent
{
    #region Members

    private Div containerRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the default command row alignment and spacing.
    /// </summary>
    public RibbonRow()
    {
        Gap = Blazorise.Gap.Is1;
        Height = Blazorise.Height.Px().Min( 0 );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = containerRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Arranges commands horizontally, sharing the group's height with other rows in classic mode and using content height in simplified mode.
    /// </summary>
    protected IFluentFlex EffectiveFlex => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified ? Blazorise.Flex.Row.NoWrap.AlignItems.Center.Shrink.Is0 : Blazorise.Flex.Row.NoWrap.AlignItems.Center.Grow.Is1.Basis.Is0;

    /// <summary>
    /// Defines commands and inputs arranged horizontally within this row.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Shares the containing ribbon's selection, display mode, and rendering policy with this component.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}