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

    /// <summary>
    /// Defines legend label styling, color boxes, and item spacing.
    /// </summary>
    public SvgChartLegendLabelsOptions Labels { get; set; } = new();

    /// <summary>
    /// Defines space around the legend, in SVG units.
    /// </summary>
    public SvgChartSpacing Padding { get; set; } = new()
    {
        Top = 6,
        Bottom = 6,
        Start = 8,
        End = 8,
    };

    #endregion
}