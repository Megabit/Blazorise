namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines easing functions for native SVG chart animations.
/// </summary>
public enum SvgChartAnimationEasing
{
    /// <summary>
    /// Defines a linear animation speed.
    /// </summary>
    Linear,

    /// <summary>
    /// Defines a standard eased animation speed.
    /// </summary>
    Ease,

    /// <summary>
    /// Defines an animation that starts slowly.
    /// </summary>
    EaseIn,

    /// <summary>
    /// Defines an animation that ends slowly.
    /// </summary>
    EaseOut,

    /// <summary>
    /// Defines an animation that starts and ends slowly.
    /// </summary>
    EaseInOut
}