namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the axes along which native SVG chart data points can be dragged.
/// </summary>
public enum SvgChartDataDragMode
{
    /// <summary>
    /// Allows dragging along the horizontal value dimension.
    /// </summary>
    X,

    /// <summary>
    /// Allows dragging along the vertical or radial value dimension.
    /// </summary>
    Y,

    /// <summary>
    /// Allows dragging along both value dimensions where supported.
    /// </summary>
    XY
}