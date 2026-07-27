#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes font settings for report elements that render text.
/// </summary>
public sealed class ReportFontDefinition
{
    /// <summary>
    /// Font family applied to text rendered by the element.
    /// </summary>
    public string Family { get; set; }

    /// <summary>
    /// Font size applied to text rendered by the element.
    /// </summary>
    public double? Size { get; set; }

    /// <summary>
    /// Text color applied to text rendered by the element.
    /// </summary>
    public ReportColor Color { get; set; }

    /// <summary>
    /// Enables bold text rendering.
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Enables italic text rendering.
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Enables underline text rendering.
    /// </summary>
    public bool Underline { get; set; }

    /// <summary>
    /// Text alignment applied inside the element box.
    /// </summary>
    public TextAlignment Alignment { get; set; } = TextAlignment.Default;

    /// <summary>
    /// Vertical text alignment applied inside the element box.
    /// </summary>
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Default;
}