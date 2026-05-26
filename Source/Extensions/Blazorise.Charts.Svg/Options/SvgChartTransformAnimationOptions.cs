namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines transform animation options for native SVG charts.
/// </summary>
public class SvgChartTransformAnimationOptions : SvgChartAnimationTargetOptions
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgChartTransformAnimationOptions"/> class.
    /// </summary>
    public SvgChartTransformAnimationOptions()
    {
        Enabled = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the scale value used by renderers that support transform scale animations.
    /// </summary>
    public double ScaleFrom { get; set; } = 0.95;

    /// <summary>
    /// Defines the scale target value used by renderers that support transform scale animations.
    /// </summary>
    public double ScaleTo { get; set; } = 1;

    #endregion
}