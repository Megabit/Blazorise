namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines stroke animation options for native SVG charts.
/// </summary>
public class SvgChartStrokeAnimationOptions : SvgChartAnimationTargetOptions
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgChartStrokeAnimationOptions"/> class.
    /// </summary>
    public SvgChartStrokeAnimationOptions()
    {
        Enabled = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether stroke width changes may be animated by renderers that support stroke animations.
    /// </summary>
    public bool AnimateWidth { get; set; } = true;

    /// <summary>
    /// Defines whether stroke dash pattern changes may be animated by renderers that support stroke animations.
    /// </summary>
    public bool AnimateDashPattern { get; set; } = true;

    #endregion
}