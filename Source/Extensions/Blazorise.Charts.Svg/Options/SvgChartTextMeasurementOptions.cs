#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Controls text width measurement used by SVG chart layout.
/// </summary>
public class SvgChartTextMeasurementOptions
{
    /// <summary>
    /// Measures text in SVG units using the resolved font. When null, an estimate is used.
    /// The callback must be synchronous and return a finite, nonnegative width.
    /// </summary>
    public Func<string, SvgChartFontOptions, double> Measure { get; set; }

    /// <summary>
    /// Defines the estimated width of one text element relative to the font size when no measurement is available.
    /// </summary>
    public double AverageCharacterWidth { get; set; } = 0.58;
}