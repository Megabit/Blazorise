#region Using directives
using System;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines title options for a native SVG chart axis.
/// </summary>
public class SvgChartAxisTitleOptions
{
    #region Properties

    internal double EffectiveTopPadding => Math.Max( 0, Padding?.Top ?? 4 );

    internal double EffectiveBottomPadding => Math.Max( 0, Padding?.Bottom ?? 4 );

    /// <summary>
    /// Defines the title text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Defines padding in the title's local orientation, in SVG units. Only <see cref="SvgChartSpacing.Top"/> and <see cref="SvgChartSpacing.Bottom"/> are used.
    /// </summary>
    /// <remarks>
    /// When null, inherits the parent title padding or defaults to 4 on each side.
    /// Top separates a horizontal title from its labels; Bottom separates a vertical title from its labels.
    /// </remarks>
    public SvgChartSpacing Padding { get; set; }

    /// <summary>
    /// Defines the title font options. Unspecified values inherit from the parent title or chart font.
    /// </summary>
    public SvgChartFontOptions Font { get; set; }

    #endregion

    #region Operators

    /// <summary>
    /// Converts a text value to axis title options.
    /// </summary>
    /// <param name="text">The title text.</param>
    public static implicit operator SvgChartAxisTitleOptions( string text ) => text is null ? null : new() { Text = text };

    #endregion
}