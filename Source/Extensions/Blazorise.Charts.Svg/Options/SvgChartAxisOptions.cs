namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines axis options for a native SVG chart.
/// </summary>
public class SvgChartAxisOptions
{
    #region Properties

    /// <summary>
    /// Defines the axis identifier used by series to target this axis.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Defines the axis position.
    /// </summary>
    public SvgChartAxisPosition Position { get; set; } = SvgChartAxisPosition.Auto;

    /// <summary>
    /// Defines whether the value axis includes zero.
    /// </summary>
    public bool BeginAtZero { get; set; } = true;

    /// <summary>
    /// Defines a custom minimum value.
    /// </summary>
    public double? Min { get; set; }

    /// <summary>
    /// Defines a custom maximum value.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Defines the number of axis ticks.
    /// </summary>
    public int TickCount { get; set; } = 5;

    /// <summary>
    /// Defines grid line options.
    /// </summary>
    public SvgChartGridLinesOptions GridLines { get; set; } = new();

    /// <summary>
    /// Defines label options.
    /// </summary>
    public SvgChartAxisLabelsOptions Labels { get; set; } = new();

    /// <summary>
    /// Defines the axis title.
    /// </summary>
    public string Title { get; set; }

    #endregion
}