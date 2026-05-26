namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the visible chart viewport ranges.
/// </summary>
public class SvgChartViewport
{
    #region Properties

    /// <summary>
    /// Defines the minimum visible horizontal value.
    /// </summary>
    public double? XMin { get; set; }

    /// <summary>
    /// Defines the maximum visible horizontal value.
    /// </summary>
    public double? XMax { get; set; }

    /// <summary>
    /// Defines the minimum visible vertical value.
    /// </summary>
    public double? YMin { get; set; }

    /// <summary>
    /// Defines the maximum visible vertical value.
    /// </summary>
    public double? YMax { get; set; }

    #endregion
}