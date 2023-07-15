namespace Blazorise;

/// <summary>
/// Defines the options to build the color swatches.
/// </summary>
public class ThemeSwatchOptions
{
    /// <summary>
    /// Gets or sets the color in HEX format.
    /// </summary>
    public string HexColor { get; set; }

    /// <summary>
    /// Gets or sets the palete Hue component.
    /// </summary>
    public double? Hue { get; set; }

    /// <summary>
    /// Gets or sets the palete Saturation component.
    /// </summary>
    public double? Saturation { get; set; }

    /// <summary>
    /// Gets or sets the Lightness/Luminance Distribution 0-100.
    /// </summary>
    public double? LightnessMin { get; set; }

    /// <summary>
    /// Lightness/Luminance Distribution 0-100.
    /// </summary>
    public double? LightnessMax { get; set; }

    /// <summary>
    /// Set to true if you want to calculate colors based on lightness instead of luminance.
    /// </summary>
    public bool? UseLightness { get; set; }
}