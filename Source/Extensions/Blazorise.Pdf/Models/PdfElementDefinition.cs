#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes a PDF element.
/// </summary>
public sealed class PdfElementDefinition
{
    #region Properties

    /// <summary>
    /// Element kind.
    /// </summary>
    public PdfElementType Type { get; set; }

    /// <summary>
    /// Horizontal element position in points.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Vertical element position in points.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Element width in points.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Element height in points.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Orientation used by line elements.
    /// </summary>
    public Orientation Orientation { get; set; } = Orientation.Horizontal;

    /// <summary>
    /// Text content rendered by text-like elements.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Indicates that text should wrap inside the element bounds.
    /// </summary>
    public bool Wrap { get; set; } = true;

    /// <summary>
    /// Image source used by image elements.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Defines how the image fits inside the element bounds.
    /// </summary>
    public PdfImageFit ImageFit { get; set; } = PdfImageFit.Fill;

    /// <summary>
    /// Font settings used by text-like elements.
    /// </summary>
    public PdfFontDefinition Font { get; set; } = new();

    /// <summary>
    /// Border settings used by shape-like elements.
    /// </summary>
    public PdfBorderDefinition Border { get; set; } = new();

    /// <summary>
    /// Appearance settings used by filled elements.
    /// </summary>
    public PdfAppearanceDefinition Appearance { get; set; } = new();

    /// <summary>
    /// Table rows used by table elements.
    /// </summary>
    public List<PdfTableRowDefinition> Rows { get; set; } = [];

    #endregion
}