namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Button"/> component.
/// </summary>
public record ThemeButtonOptions : ThemeBasicOptions
{
    /// <summary>
    /// Gets or sets the padding for the button element.
    /// </summary>
    public string Padding { get; set; }

    /// <summary>
    /// Gets or sets the margin for the button element.
    /// </summary>
    public string Margin { get; set; }

    /// <summary>
    /// Gets or sets the box shadow size of the button element.
    /// </summary>
    public string BoxShadowSize { get; set; }

    /// <summary>
    /// Gets or sets the box shadow transparency of the button element(range 0-255)
    /// </summary>
    public byte? BoxShadowTransparency { get; set; } = 127;

    /// <summary>
    /// Defines how much the color will darken when element is hovered(range 0-100).
    /// </summary>
    public float? HoverDarkenColor { get; set; } = 15f;

    /// <summary>
    /// Defines how much the color will lighten when element is hovered(range 0-100).
    /// </summary>
    public float? HoverLightenColor { get; set; } = 20f;

    /// <summary>
    /// Defines how much the color will darken when element is active(range 0-100).
    /// </summary>
    public float? ActiveDarkenColor { get; set; } = 20f;

    /// <summary>
    /// Defines how much the color will lighten when element is active(range 0-100).
    /// </summary>
    public float? ActiveLightenColor { get; set; } = 25f;

    /// <summary>
    /// Defines the button border radius when it is <see cref="Size.Large"/>.
    /// </summary>
    public string LargeBorderRadius { get; set; } = ".3rem";

    /// <summary>
    /// Defines the button border radius when it is <see cref="Size.Small"/>.
    /// </summary>
    public string SmallBorderRadius { get; set; } = ".2rem";

    /// <summary>
    /// Defines the percentage of how much the colors will blend together.
    /// </summary>
    public float? GradientBlendPercentage { get; set; } = 15f;

    /// <summary>
    /// Defined the opacity of disabled button(range 0-1).
    /// </summary>
    public float? DisabledOpacity { get; set; } = .65f;

    /// <summary>
    /// Gets or sets the global size for button components.
    /// </summary>
    public Size? Size { get; set; }
}