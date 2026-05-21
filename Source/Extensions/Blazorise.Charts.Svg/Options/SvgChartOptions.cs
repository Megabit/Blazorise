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
    /// Defines the default font options.
    /// </summary>
    public SvgChartFontOptions Font { get; set; } = new();

    /// <summary>
    /// Defines the chart title options.
    /// </summary>
    public SvgChartTextOptions Title { get; set; } = new()
    {
        Font = new()
        {
            Size = 16,
            Weight = "600",
        },
        Padding = new()
        {
            Top = 8,
        },
    };

    /// <summary>
    /// Defines the chart subtitle options.
    /// </summary>
    public SvgChartTextOptions Subtitle { get; set; } = new()
    {
        Font = new()
        {
            Size = 12,
        },
        Opacity = 0.7,
        Padding = new()
        {
            Top = 7,
        },
    };

    /// <summary>
    /// Defines legend options.
    /// </summary>
    public SvgChartLegendOptions Legend { get; set; } = new();

    /// <summary>
    /// Defines tooltip options.
    /// </summary>
    public SvgChartTooltipOptions Tooltip { get; set; } = new();

    /// <summary>
    /// Defines the category axis options.
    /// </summary>
    public SvgChartAxisOptions XAxis { get; set; } = new()
    {
        BeginAtZero = false,
        GridLines = new()
        {
            Visible = false,
        },
    };

    /// <summary>
    /// Defines the value axis options.
    /// </summary>
    public SvgChartAxisOptions YAxis { get; set; } = new();

    /// <summary>
    /// Defines streaming options.
    /// </summary>
    public SvgChartStreamingOptions Streaming { get; set; }

    #endregion
}