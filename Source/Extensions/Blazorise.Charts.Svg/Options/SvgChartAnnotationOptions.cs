namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines base annotation options for a native SVG chart.
/// </summary>
public class SvgChartAnnotationOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the annotation is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the annotation name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Defines the value axis identifier used by this annotation.
    /// </summary>
    public string ValueAxisId { get; set; }

    /// <summary>
    /// Defines the annotation rendering order among annotations of the same type. Lower values are rendered first, behind higher values.
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Defines annotation label options.
    /// </summary>
    public SvgChartAnnotationLabelOptions Label { get; set; }

    internal virtual SvgChartAnnotationType AnnotationType { get; }

    #endregion
}