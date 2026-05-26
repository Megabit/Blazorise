namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides information used to format a native SVG chart axis tick.
/// </summary>
public sealed class SvgChartAxisTickContext
{
    #region Properties

    /// <summary>
    /// Gets the tick value.
    /// </summary>
    public object Value { get; init; }

    /// <summary>
    /// Gets the zero-based tick index.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Gets whether the tick belongs to the category axis.
    /// </summary>
    public bool CategoryAxis { get; init; }

    /// <summary>
    /// Gets the axis identifier.
    /// </summary>
    public string AxisId { get; init; }

    #endregion
}