namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the drawable plot area for native SVG chart plugins.
/// </summary>
public sealed class SvgChartPluginPlotArea
{
    #region Properties

    /// <summary>
    /// Gets the left plot coordinate.
    /// </summary>
    public double Left { get; init; }

    /// <summary>
    /// Gets the top plot coordinate.
    /// </summary>
    public double Top { get; init; }

    /// <summary>
    /// Gets the right plot coordinate.
    /// </summary>
    public double Right { get; init; }

    /// <summary>
    /// Gets the bottom plot coordinate.
    /// </summary>
    public double Bottom { get; init; }

    /// <summary>
    /// Gets the plot width.
    /// </summary>
    public double Width => Right - Left;

    /// <summary>
    /// Gets the plot height.
    /// </summary>
    public double Height => Bottom - Top;

    #endregion
}