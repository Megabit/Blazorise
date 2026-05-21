namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines point annotation options for a native SVG chart.
/// </summary>
public class SvgChartPointAnnotationOptions : SvgChartAnnotationOptions
{
    #region Properties

    /// <summary>
    /// Defines the annotation X value. For category charts, this is the category index.
    /// </summary>
    public double? X { get; set; }

    /// <summary>
    /// Defines the annotation Y value.
    /// </summary>
    public double? Y { get; set; }

    /// <summary>
    /// Defines the annotation point radius.
    /// </summary>
    public double Radius { get; set; } = 5;

    /// <summary>
    /// Defines the annotation background color.
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the annotation border options.
    /// </summary>
    public SvgChartBorderOptions Border { get; set; } = new()
    {
        Width = 1,
    };

    /// <summary>
    /// Defines the annotation opacity.
    /// </summary>
    public double Opacity { get; set; } = 1;

    internal override SvgChartAnnotationType AnnotationType => SvgChartAnnotationType.Point;

    #endregion
}