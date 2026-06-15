namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines label options for a native SVG chart axis.
/// </summary>
public class SvgChartAxisLabelsOptions
{
    #region Properties

    /// <summary>
    /// Defines whether labels are visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the interval for visible labels.
    /// </summary>
    public int Step { get; set; } = 1;

    /// <summary>
    /// Defines whether labels are automatically skipped when there is not enough space to render every label.
    /// </summary>
    public bool AutoSkip { get; set; } = true;

    /// <summary>
    /// Defines the maximum number of labels to render when automatic skipping is enabled. Set to 0 to disable the maximum.
    /// </summary>
    public int MaxTicksLimit { get; set; } = 11;

    /// <summary>
    /// Defines the extra spacing between automatically skipped labels, in SVG units.
    /// </summary>
    public double AutoSkipPadding { get; set; } = 8;

    /// <summary>
    /// Defines whether horizontal labels can rotate before additional labels are skipped.
    /// </summary>
    public bool AutoRotate { get; set; } = true;

    /// <summary>
    /// Defines the maximum automatic label rotation in degrees.
    /// </summary>
    public double MaxRotation { get; set; } = 50;

    /// <summary>
    /// Defines the label offset from the axis line.
    /// </summary>
    public double Offset { get; set; } = 24;

    /// <summary>
    /// Defines the maximum label width in SVG units. Longer labels are shortened.
    /// </summary>
    public double? MaxWidth { get; set; }

    #endregion
}