namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines text options for a native SVG chart.
/// </summary>
public class SvgChartTextOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the text is visible. When null, the value is inherited from the parent options.
    /// </summary>
    public bool? Visible { get; set; }

    /// <summary>
    /// Defines the text content.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Defines the text position. When null, the value is inherited from the parent options.
    /// </summary>
    public SvgChartTextPosition? Position { get; set; }

    /// <summary>
    /// Defines the text alignment. When null, the value is inherited from the parent options.
    /// </summary>
    public SvgChartTextAlignment? Alignment { get; set; }

    /// <summary>
    /// Defines the text padding.
    /// </summary>
    public SvgChartSpacing Padding { get; set; }

    /// <summary>
    /// Defines the text font options.
    /// </summary>
    public SvgChartFontOptions Font { get; set; }

    /// <summary>
    /// Defines the text opacity.
    /// </summary>
    public double? Opacity { get; set; }

    #endregion

    #region Operators

    /// <summary>
    /// Converts a text value to chart text options.
    /// </summary>
    /// <param name="text">The text content.</param>
    public static implicit operator SvgChartTextOptions( string text )
    {
        return new()
        {
            Text = text,
        };
    }

    #endregion
}