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
    /// Gets the size after applying the ribbon display mode.
    /// </summary>
    protected RibbonItemSize EffectiveSize => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified && Size == RibbonItemSize.Large ? RibbonItemSize.Medium : Size;

    /// <summary>
    /// Gets the command height after applying explicit sizing and the display mode.
    /// </summary>
    protected IFluentSizing EffectiveHeight => Height ?? ( EffectiveSize == RibbonItemSize.Large ? Blazorise.Height.Rem( 6 ) : Blazorise.Height.Auto );

    /// <summary>
    /// Gets or sets the command text and accessible name.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets an icon name supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Gets or sets the command layout, independently of font size.
    /// </summary>
    [Parameter] public RibbonItemSize Size { get; set; } = RibbonItemSize.Medium;

    /// <summary>
    /// Gets or sets the provider-native command color.
    /// </summary>
    [Parameter] public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// Gets or sets whether the command is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets child content for the command. Dropdown and split commands render this content as menu items; other commands use it to replace the default icon and text.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets the containing ribbon state.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}