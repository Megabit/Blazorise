#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines data label options for a native SVG chart.
/// </summary>
public class SvgChartDataLabelsOptions
{
    #region Properties

    /// <summary>
    /// Defines whether data labels are visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Defines whether data labels react to pointer and keyboard interactions.
    /// </summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// Defines the data label position.
    /// </summary>
    public SvgChartDataLabelPosition Position { get; set; } = SvgChartDataLabelPosition.Auto;

    /// <summary>
    /// Defines the label offset from the point anchor.
    /// </summary>
    public double Offset { get; set; } = 6;

    /// <summary>
    /// Defines the label opacity.
    /// </summary>
    public double Opacity { get; set; } = 1;

    /// <summary>
    /// Defines data label font options.
    /// </summary>
    public SvgChartFontOptions Font { get; set; } = new()
    {
        Size = 11,
        Weight = "600",
    };

    /// <summary>
    /// Defines the label padding.
    /// </summary>
    public SvgChartSpacing Padding { get; set; } = new()
    {
        Top = 3,
        End = 5,
        Bottom = 3,
        Start = 5,
    };

    /// <summary>
    /// Defines the label background color.
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the label border options.
    /// </summary>
    public SvgChartBorderOptions Border { get; set; } = new()
    {
        Radius = 3,
    };

    /// <summary>
    /// Defines a callback used to format label text.
    /// </summary>
    public Func<SvgChartDataLabelContext, string> Formatter { get; set; }

    #endregion
}