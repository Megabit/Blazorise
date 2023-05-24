namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Tooltip"/> component.
/// </summary>
public record ThemeTooltipOptions : ThemeBasicOptions
{
    /// <summary>
    /// Tooltip background color. Can contain alpha value in 8-hex formatted color.
    /// </summary>
    public string BackgroundColor { get; set; } = "#808080e6";

    /// <summary>
    /// Tooltip text color. Can contain alpha value in 8-hex formatted color.
    /// </summary>
    public string Color { get; set; } = "#ffffff";

    /// <summary>
    /// Gets or sets the tooltip font-size.
    /// </summary>
    public string FontSize { get; set; } = ".875rem";

    /// <summary>
    /// Gets or sets the duration of the tooltip fade animation.
    /// </summary>
    public string FadeTime { get; set; } = "0.3s";

    /// <summary>
    /// Gets or sets the maximum width of the tooltip.
    /// </summary>
    public string MaxWidth { get; set; } = "15rem";

    /// <summary>
    /// Gets or sets the padding of the tooltip container.
    /// </summary>
    public string Padding { get; set; } = ".5rem 1rem";

    /// <summary>
    /// Gets or sets the tooltip z-index.
    /// </summary>
    public string ZIndex { get; set; } = "1020";
}