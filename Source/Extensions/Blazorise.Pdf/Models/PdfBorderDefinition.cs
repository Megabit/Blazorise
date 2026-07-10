#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes PDF border styling.
/// </summary>
public sealed class PdfBorderDefinition
{
    #region Properties

    /// <summary>
    /// Border color as a hex color value.
    /// </summary>
    public string Color { get; set; } = "#000000";

    /// <summary>
    /// Border width in points.
    /// </summary>
    public double Width { get; set; } = 1;

    /// <summary>
    /// Border line style.
    /// </summary>
    public PdfBorderStyle Style { get; set; }

    #endregion
}