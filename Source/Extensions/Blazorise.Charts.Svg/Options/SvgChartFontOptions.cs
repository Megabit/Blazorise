namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines font options for native SVG chart text.
/// </summary>
public class SvgChartFontOptions
{
    #region Properties

    /// <summary>
    /// Defines the font family.
    /// </summary>
    public string Family { get; set; }

    /// <summary>
    /// Defines the font size.
    /// </summary>
    public double? Size { get; set; }

    /// <summary>
    /// Defines the font weight.
    /// </summary>
    public string Weight { get; set; }

    /// <summary>
    /// Defines the font color.
    /// </summary>
    public Color Color { get; set; }

    #endregion
}