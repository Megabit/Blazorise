namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines box annotation options for a native SVG chart.
/// </summary>
public class SvgChartBoxAnnotationOptions : SvgChartAnnotationOptions
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
    /// Defines the annotation background color.
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the annotation border options.
    /// </summary>
    public SvgChartBorderOptions Border { get; set; } = new();

    /// <summary>
    /// Defines the annotation opacity.
    /// </summary>
    public double Opacity { get; set; } = 0.16;

    internal override SvgChartAnnotationType AnnotationType => SvgChartAnnotationType.Box;

    #endregion
}