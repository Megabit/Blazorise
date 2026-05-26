namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines zoom and pan behavior for a native SVG chart.
/// </summary>
public class SvgChartZoomOptions
{
    #region Properties

    /// <summary>
    /// Defines whether zoom and pan behavior is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Defines which chart axis can be zoomed or panned.
    /// </summary>
    public SvgChartZoomMode Mode { get; set; } = SvgChartZoomMode.X;

    /// <summary>
    /// Defines whether mouse wheel zoom is enabled.
    /// </summary>
    public bool Wheel { get; set; } = true;

    /// <summary>
    /// Defines whether drag panning is enabled.
    /// </summary>
    public bool Pan { get; set; } = true;

    /// <summary>
    /// Defines the minimum zoom factor relative to the full data range.
    /// </summary>
    public double MinZoom { get; set; } = 1;

    /// <summary>
    /// Defines the maximum zoom factor relative to the full data range.
    /// </summary>
    public double MaxZoom { get; set; } = 20;

    /// <summary>
    /// Defines the current visible viewport.
    /// </summary>
    public SvgChartViewport Viewport { get; set; }

    #endregion
}