namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines line annotation options for a native SVG chart.
/// </summary>
public class SvgChartLineAnnotationOptions : SvgChartAnnotationOptions
{
    #region Properties

    /// <summary>
    /// Defines the annotation start X value. For category charts, this is the category index where <c>-0.5</c> represents the left plot edge.
    /// </summary>
    public double? XMin { get; set; }

    /// <summary>
    /// Defines the annotation end X value. For category charts, this is the category index where <c>-0.5</c> represents the left plot edge.
    /// </summary>
    public double? XMax { get; set; }

    /// <summary>
    /// Defines the annotation start Y value.
    /// </summary>
    public double? YMin { get; set; }

    /// <summary>
    /// Defines the annotation end Y value.
    /// </summary>
    public double? YMax { get; set; }

    /// <summary>
    /// Defines the annotation line border options.
    /// </summary>
    public SvgChartBorderOptions Border { get; set; } = new()
    {
        Width = 2,
    };

    /// <summary>
    /// Defines the annotation line dash pattern.
    /// </summary>
    public string DashPattern { get; set; }

    /// <summary>
    /// Defines the annotation opacity.
    /// </summary>
    public double Opacity { get; set; } = 1;

    internal override SvgChartAnnotationType AnnotationType => SvgChartAnnotationType.Line;

    #endregion
}