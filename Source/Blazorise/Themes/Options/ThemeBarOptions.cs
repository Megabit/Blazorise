namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Bar"/> component.
/// </summary>
public record ThemeBarOptions
{
    /// <summary>
    /// Gets or sets the width of the <see cref="Bar"/> in vertical mode.
    /// </summary>
    public string VerticalWidth { get; set; } = "230px";

    /// <summary>
    /// Gets or sets the width of the <see cref="Bar"/> in vertical collapsed mode.
    /// </summary>
    public string VerticalSmallWidth { get; set; } = "64px";

    /// <summary>
    /// Gets or sets the size of the <see cref="BarBrand"/> component.
    /// </summary>
    public string VerticalBrandHeight { get; set; } = "64px";

    /// <summary>
    /// Gets or sets the width of the dropdown-menu in vertical mode.
    /// </summary>
    public string VerticalPopoutMenuWidth { get; set; } = "180px";

    /// <summary>
    /// Gets or sets the height of the <see cref="Bar"/> in horizontal mode (only for Bar that is placed inside of <see cref="LayoutHeader"/>).
    /// </summary>
    public string HorizontalHeight { get; set; } = "auto";

    /// <summary>
    /// Gets or sets the font options for bar navigation items.
    /// </summary>
    public ThemeFontOptions FontOptions { get; set; }

    /// <summary>
    /// Gets or sets which bar orientations receive <see cref="LightColors"/> and <see cref="DarkColors"/>. Defaults to <see cref="ThemeBarColorScope.Both"/>.
    /// </summary>
    /// <remarks>
    /// Includes item, brand, and dropdown colors. Does not affect sizing, fonts, or dropdown behavior.
    /// </remarks>
    public ThemeBarColorScope ColorScope { get; set; } = ThemeBarColorScope.Both;

    /// <summary>
    /// Gets or sets the dark color scheme for bars with <see cref="ThemeContrast.Dark"/>, applied to the orientations selected by <see cref="ColorScope"/>.
    /// </summary>
    public ThemeBarColorOptions DarkColors { get; set; }

    /// <summary>
    /// Gets or sets the light color scheme for bars with <see cref="ThemeContrast.Light"/>, applied to the orientations selected by <see cref="ColorScope"/>.
    /// </summary>
    public ThemeBarColorOptions LightColors { get; set; }

    /// <summary>
    /// Gets or sets the theme settings for the <see cref="BarDropdown"/> component.
    /// </summary>
    public ThemeBarDropdownOptions DropdownOptions { get; set; }
}