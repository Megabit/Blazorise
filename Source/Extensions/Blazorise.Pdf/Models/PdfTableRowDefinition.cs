#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes a PDF table row.
/// </summary>
public sealed class PdfTableRowDefinition
{
    #region Properties

    /// <summary>
    /// Row height in points.
    /// </summary>
    public double Height { get; set; } = 24;

    /// <summary>
    /// Cells included in the row.
    /// </summary>
    public List<PdfTableCellDefinition> Cells { get; set; } = [];

    #endregion
}