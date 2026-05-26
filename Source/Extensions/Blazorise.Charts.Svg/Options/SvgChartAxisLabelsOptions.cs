namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines label options for a native SVG chart axis.
/// </summary>
public class SvgChartAxisLabelsOptions
{
    #region Properties

    /// <summary>
    /// Defines whether labels are visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the interval for visible labels.
    /// </summary>
    public int Step { get; set; } = 1;

    /// <summary>
    /// Defines the label offset from the axis line.
    /// </summary>
    public double Offset { get; set; } = 24;

    #endregion
}