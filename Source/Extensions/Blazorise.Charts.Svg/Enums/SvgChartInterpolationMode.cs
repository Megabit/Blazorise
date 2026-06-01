namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines how SVG chart line and area paths are interpolated between data points.
/// </summary>
public enum SvgChartInterpolationMode
{
    /// <summary>
    /// Connects data points with straight line segments.
    /// </summary>
    Linear,

    /// <summary>
    /// Connects data points with monotone cubic curves.
    /// </summary>
    Monotone,

    /// <summary>
    /// Connects data points with cubic curves controlled by tension.
    /// </summary>
    Cubic,

    /// <summary>
    /// Steps to the next value before moving to the next category.
    /// </summary>
    StepBefore,

    /// <summary>
    /// Steps to the next value after moving to the next category.
    /// </summary>
    StepAfter,

    /// <summary>
    /// Steps to the next value at the middle of two categories.
    /// </summary>
    StepMiddle
}