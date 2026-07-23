#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes PDF font styling.
/// </summary>
public sealed class PdfFontDefinition
{
    #region Properties

    /// <summary>
    /// Font family name. The built-in renderer maps the family to the closest PDF standard font (Helvetica, Times, or Courier).
    /// </summary>
    public string Family { get; set; } = "Helvetica";

    /// <summary>
    /// Font size in points.
    /// </summary>
    public double Size { get; set; } = 12;

    /// <summary>
    /// Text color as a hex color value.
    /// </summary>
    public string Color { get; set; } = "#000000";

    /// <summary>
    /// Text alignment inside the element bounds.
    /// </summary>
    /// <remarks>
    /// <see cref="TextAlignment.Default"/> and <see cref="TextAlignment.Start"/> align to the start.
    /// <see cref="TextAlignment.Justified"/> distributes words across wrapped non-final paragraph lines.
    /// </remarks>
    public TextAlignment Alignment { get; set; }

    /// <summary>
    /// Text vertical alignment inside the element bounds.
    /// </summary>
    /// <remarks>
    /// <see cref="VerticalAlignment.Default"/>, <see cref="VerticalAlignment.Baseline"/>,
    /// <see cref="VerticalAlignment.Top"/>, and <see cref="VerticalAlignment.TextTop"/> align to the top.
    /// <see cref="VerticalAlignment.Middle"/> centers the text, while <see cref="VerticalAlignment.Bottom"/>
    /// and <see cref="VerticalAlignment.TextBottom"/> align to the bottom.
    /// </remarks>
    public VerticalAlignment VerticalAlignment { get; set; }

    /// <summary>
    /// Indicates that text should be rendered bold when supported by the renderer.
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Indicates that text should be rendered italic when supported by the renderer.
    /// </summary>
    public bool Italic { get; set; }

    #endregion
}