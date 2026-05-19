namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines legend options for a native SVG chart.
/// </summary>
public class SvgChartLegendOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the legend is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the legend position.
    /// </summary>
    public SvgChartLegendPosition Position { get; set; } = SvgChartLegendPosition.Bottom;

    #endregion
}