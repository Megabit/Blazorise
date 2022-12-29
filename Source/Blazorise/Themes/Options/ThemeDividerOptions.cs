namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Divider"/> component.
/// </summary>
public record ThemeDividerOptions
{
    /// <summary>
    /// Gets or sets the divider color.
    /// </summary>
    public string Color { get; set; } = "#999999";

    /// <summary>
    /// Gets or sets the divider thickness.
    /// </summary>
    public string Thickness { get; set; } = "1px";

    /// <summary>
    /// Gets or sets the size of the divider text.
    /// </summary>
    public string TextSize { get; set; } = ".85rem";

    /// <summary>
    /// Gets or sets the default <see cref="DividerType">divider type</see>.
    /// </summary>
    public DividerType? DividerType { get; set; } = Blazorise.DividerType.Solid;
}