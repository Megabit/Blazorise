namespace Blazorise;

/// <summary>
/// Defines the color swatch.
/// </summary>
public class ThemeSwatch
{
    /// <summary>
    /// Gets the swatch key.
    /// </summary>
    public double Key { get; set; }

    /// <summary>
    /// Gets the swatch color in hex format.
    /// </summary>
    public string HexColor { get; set; }

    /// <summary>
    /// Gets the swatch hls-color hue component.
    /// </summary>
    public double Hue { get; set; }

    /// <summary>
    /// Gets the swatch hls-color hue scale.
    /// </summary>
    public double HueScale { get; set; }

    /// <summary>
    /// Gets the swatch hls-color saturation component.
    /// </summary>
    public double Saturation { get; set; }

    /// <summary>
    /// Gets the swatch hls-color saturation scale.
    /// </summary>
    public double SaturationScale { get; set; }

    /// <summary>
    /// Gets the swatch hls-color lightness component.
    /// </summary>
    public double Lightness { get; set; }
}