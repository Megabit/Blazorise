namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines opacity animation options for native SVG charts.
/// </summary>
public class SvgChartOpacityAnimationOptions : SvgChartAnimationTargetOptions
{
    #region Properties

    /// <summary>
    /// Defines the opacity value used as the animation start value.
    /// </summary>
    public double From { get; set; }

    #endregion
}