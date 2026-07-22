namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines outline options for a native SVG line series.
/// </summary>
public class SvgChartLineOutlineOptions
{
    #region Properties

    /// <summary>
    /// Defines the outline color. When omitted, the series color is used.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Defines the total outline stroke width.
    /// </summary>
    public double StrokeWidth { get; set; } = 4;

    /// <summary>
    /// Defines the outline opacity between 0 and 1.
    /// </summary>
    public double Opacity { get; set; } = 1;

    #endregion
}