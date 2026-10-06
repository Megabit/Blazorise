#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Supplies shared layout and presentation parameters for ribbon commands.
/// </summary>
public abstract class BaseRibbonItem : BaseComponent
{
    #region Constructors

    /// <summary>
    /// Initializes the default command utilities.
    /// </summary>
    protected BaseRibbonItem()
    {
        Border = Blazorise.Border.Is0;
        Flex = Blazorise.Flex.Shrink.Is0;
        Width = Blazorise.Width.Rem().Min( 1.75 );
        TextSize = Blazorise.TextSize.Px( 13 );
        TextOverflow = Blazorise.TextOverflow.NoWrap;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Resolves the command layout, reducing <see cref="RibbonItemSize.Large"/> to <see cref="RibbonItemSize.Medium"/> in simplified mode.
    /// </summary>
    protected RibbonItemSize EffectiveSize => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified && Size == RibbonItemSize.Large ? RibbonItemSize.Medium : Size;

    /// <summary>
    /// Uses the explicit height when supplied; otherwise, large commands fill the group height and other commands size to their content.
    /// </summary>
    protected IFluentSizing EffectiveHeight => Height ?? ( EffectiveSize == RibbonItemSize.Large ? Blazorise.Height.Rem( 6 ) : Blazorise.Height.Auto );

    /// <summary>
    /// Specifies the command label and accessible name.
    /// </summary>
    /// <remarks>
    /// Small commands with an icon omit the visible label.
    /// </remarks>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Specifies the command icon. Accepts an icon name supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Controls the placement of the command icon and label, independently of font size.
    /// </summary>
    /// <remarks>
    /// Large commands become medium commands in simplified mode. Defaults to <see cref="RibbonItemSize.Medium"/>.
    /// </remarks>
    [Parameter] public RibbonItemSize Size { get; set; } = RibbonItemSize.Medium;

    /// <summary>
    /// Selects the color used for the command and its interaction states. Defaults to <see cref="Color.Default"/>.
    /// </summary>
    [Parameter] public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// Prevents activating the command or opening its menu. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Defines the command content.
    /// </summary>
    /// <remarks>
    /// Buttons use this content instead of the default icon and label.
    /// Dropdowns and split buttons use it for menu content, such as <see cref="RibbonDropdownItem"/> and <see cref="RibbonDropdownDivider"/>.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Shares the containing ribbon's selection, display mode, and rendering policy with this component.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}