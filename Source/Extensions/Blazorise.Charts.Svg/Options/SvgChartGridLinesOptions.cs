namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines grid line options for a native SVG chart axis.
/// </summary>
public class SvgChartGridLinesOptions
{
    #region Properties

    /// <summary>
    /// Defines whether grid lines are visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the grid line color.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Defines the grid line width.
    /// </summary>
    public double Width { get; set; } = 1;

    /// <summary>
    /// Defines the grid line opacity.
    /// </summary>
    public double Opacity { get; set; } = 0.14;

    /// <summary>
    /// Defines the grid line dash pattern.
    /// </summary>
    public string DashPattern { get; set; }

    #endregion
}