namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the position of a native SVG chart data label.
/// </summary>
public enum SvgChartDataLabelPosition
{
    /// <summary>
    /// Positions the label automatically based on the chart type.
    /// </summary>
    Auto,

    /// <summary>
    /// Positions the label at the point center.
    /// </summary>
    Center,

    /// <summary>
    /// Positions the label above the point.
    /// </summary>
    Top,

    /// <summary>
    /// Positions the label after the point.
    /// </summary>
    End,

    /// <summary>
    /// Positions the label below the point.
    /// </summary>
    Bottom,

    /// <summary>
    /// Positions the label before the point.
    /// </summary>
    Start
}