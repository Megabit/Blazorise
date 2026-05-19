namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines value axis options for a native SVG chart.
/// </summary>
public class SvgChartAxisOptions
{
    #region Properties

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
    /// Defines the axis title.
    /// </summary>
    public string Title { get; set; }

    #endregion
}