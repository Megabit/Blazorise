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
    /// Gets or sets the theme settings for the <see cref="Bar"/> dark color scheme.
    /// </summary>
    public ThemeBarColorOptions DarkColors { get; set; }

    /// <summary>
    /// Gets or sets the theme settings for the <see cref="Bar"/> light color scheme.
    /// </summary>
    public ThemeBarColorOptions LightColors { get; set; }
}