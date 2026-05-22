namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the rendering layer used by a native SVG chart plugin.
/// </summary>
public enum SvgChartRenderLayer
{
    /// <summary>
    /// Renders behind chart text, axes, and series.
    /// </summary>
    Background,

    /// <summary>
    /// Renders after axes and before chart series.
    /// </summary>
    BeforeSeries,

    /// <summary>
    /// Renders after chart series.
    /// </summary>
    SeriesOverlay,

    /// <summary>
    /// Renders above regular overlays and before tooltips.
    /// </summary>
    InteractionOverlay,

    /// <summary>
    /// Renders in the tooltip layer.
    /// </summary>
    Tooltip
}