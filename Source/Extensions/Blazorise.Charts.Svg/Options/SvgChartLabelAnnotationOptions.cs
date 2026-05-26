namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines label annotation options for a native SVG chart.
/// </summary>
public class SvgChartLabelAnnotationOptions : SvgChartAnnotationOptions
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

    internal override SvgChartAnnotationType AnnotationType => SvgChartAnnotationType.Label;

    #endregion
}