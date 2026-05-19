namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines common native SVG chart options.
/// </summary>
public class SvgChartOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the SVG scales to the available width.
    /// </summary>
    public bool Responsive { get; set; } = true;

    /// <summary>
    /// Defines the internal SVG viewport width.
    /// </summary>
    public double Width { get; set; } = 640;

    /// <summary>
    /// Defines the internal SVG viewport height.
    /// </summary>
    public double Height { get; set; } = 360;

    /// <summary>
    /// Defines the chart title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Defines the chart subtitle.
    /// </summary>
    public string Subtitle { get; set; }

    /// <summary>
    /// Defines legend options.
    /// </summary>
    public SvgChartLegendOptions Legend { get; set; } = new();

    /// <summary>
    /// Defines tooltip options.
    /// </summary>
    public SvgChartTooltipOptions Tooltip { get; set; } = new();

    /// <summary>
    /// Defines the value axis options.
    /// </summary>
    public SvgChartAxisOptions YAxis { get; set; } = new();

    #endregion
}