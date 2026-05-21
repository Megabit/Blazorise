namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines trendline options for a native SVG chart.
/// </summary>
public class SvgChartTrendlineOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the trendline is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the source series name used to calculate the trendline.
    /// </summary>
    public string SeriesName { get; set; }

    /// <summary>
    /// Defines the trendline name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Defines the trendline calculation type.
    /// </summary>
    public SvgChartTrendlineType Type { get; set; } = SvgChartTrendlineType.Linear;

    /// <summary>
    /// Defines the trendline color. Use a Blazorise theme color, or pass a CSS color value such as <c>#4c6ef5</c>, <c>rgb(76, 110, 245)</c>, <c>hsl(228 88% 60%)</c>, or <c>var(--chart-color)</c>.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Defines the trendline stroke width.
    /// </summary>
    public double StrokeWidth { get; set; } = 2;

    /// <summary>
    /// Defines the trendline stroke dash pattern.
    /// </summary>
    public string DashPattern { get; set; } = "6 4";

    /// <summary>
    /// Defines the trendline opacity.
    /// </summary>
    public double Opacity { get; set; } = 0.85;

    /// <summary>
    /// Defines the trendline rendering order among other trendlines. Lower values are rendered first, behind higher values.
    /// </summary>
    public int? Order { get; set; }

    #endregion
}