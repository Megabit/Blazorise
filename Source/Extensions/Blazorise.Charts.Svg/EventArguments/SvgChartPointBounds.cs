namespace Blazorise.Charts.Svg;

/// <summary>
/// Represents a rendered chart point bounds in SVG coordinates.
/// </summary>
public sealed class SvgChartPointBounds
{
    #region Properties

    /// <summary>
    /// Gets the left coordinate of the rendered point bounds.
    /// </summary>
    public double X { get; init; }

    /// <summary>
    /// Gets the top coordinate of the rendered point bounds.
    /// </summary>
    public double Y { get; init; }

    /// <summary>
    /// Gets the width of the rendered point bounds.
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Gets the height of the rendered point bounds.
    /// </summary>
    public double Height { get; init; }

    #endregion
}