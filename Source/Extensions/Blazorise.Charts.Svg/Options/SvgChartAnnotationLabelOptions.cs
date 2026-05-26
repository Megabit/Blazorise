namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines label options for native SVG chart annotations.
/// </summary>
public class SvgChartAnnotationLabelOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the annotation label is visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Defines the annotation label text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Defines the annotation label position.
    /// </summary>
    public SvgChartAnnotationLabelPosition Position { get; set; } = SvgChartAnnotationLabelPosition.Center;

    /// <summary>
    /// Defines the annotation label offset from its anchor.
    /// </summary>
    public double Offset { get; set; } = 8;

    /// <summary>
    /// Defines the annotation label font options.
    /// </summary>
    public SvgChartFontOptions Font { get; set; } = new()
    {
        Size = 11,
        Weight = "600",
    };

    /// <summary>
    /// Defines the annotation label padding.
    /// </summary>
    public SvgChartSpacing Padding { get; set; } = new()
    {
        Top = 3,
        End = 6,
        Bottom = 3,
        Start = 6,
    };

    /// <summary>
    /// Defines the annotation label background color.
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the annotation label border options.
    /// </summary>
    public SvgChartBorderOptions Border { get; set; } = new()
    {
        Radius = 3,
    };

    #endregion
}