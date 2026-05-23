namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines path animation options for native SVG charts.
/// </summary>
public class SvgChartPathAnimationOptions : SvgChartAnimationTargetOptions
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgChartPathAnimationOptions"/> class.
    /// </summary>
    public SvgChartPathAnimationOptions()
    {
        Enabled = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether path shape changes may be animated by renderers that support path animations.
    /// </summary>
    public bool AnimateShape { get; set; } = true;

    /// <summary>
    /// Defines whether path length changes may be animated by renderers that support path animations.
    /// </summary>
    public bool AnimateLength { get; set; } = true;

    #endregion
}