namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines legend label and color box options for a native SVG chart.
/// </summary>
public class SvgChartLegendLabelsOptions
{
    /// <summary>
    /// Defines the label font. Unspecified values inherit the chart font.
    /// </summary>
    public SvgChartFontOptions Font { get; set; }

    /// <summary>
    /// Defines the color box width, in SVG units. When null, inherits the parent setting or defaults to 12.
    /// </summary>
    public double? BoxWidth { get; set; }

    /// <summary>
    /// Defines the color box height, in SVG units. When null, inherits the parent setting or uses the label font size.
    /// </summary>
    public double? BoxHeight { get; set; }

    /// <summary>
    /// Defines the spacing between legend items and rows, in SVG units. When null, inherits the parent setting or defaults to 10.
    /// </summary>
    public double? Padding { get; set; }

    /// <summary>
    /// Defines the gap between a color box and its label, in SVG units. When null, inherits the parent setting or uses half the label font size.
    /// This is an SVG chart extension to the Chart.js legend label options.
    /// </summary>
    public double? LabelGap { get; set; }
}