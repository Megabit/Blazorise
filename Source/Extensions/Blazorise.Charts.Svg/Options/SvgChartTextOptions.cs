namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines text options for a native SVG chart.
/// </summary>
public class SvgChartTextOptions
{
    #region Properties

    /// <summary>
    /// Defines whether the text is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the text content.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Defines the text position.
    /// </summary>
    public SvgChartTextPosition Position { get; set; } = SvgChartTextPosition.Top;

    /// <summary>
    /// Defines the text alignment.
    /// </summary>
    public SvgChartTextAlignment Alignment { get; set; } = SvgChartTextAlignment.Center;

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