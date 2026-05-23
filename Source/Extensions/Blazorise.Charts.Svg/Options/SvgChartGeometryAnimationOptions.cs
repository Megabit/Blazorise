namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines geometry animation options for native SVG charts.
/// </summary>
public class SvgChartGeometryAnimationOptions : SvgChartAnimationTargetOptions
{
    #region Properties

    /// <summary>
    /// Defines whether position attributes such as x, y, cx, and cy are animated.
    /// </summary>
    public bool AnimatePosition { get; set; } = true;

    /// <summary>
    /// Defines whether size attributes such as width, height, and radius are animated.
    /// </summary>
    public bool AnimateSize { get; set; } = true;

    #endregion
}