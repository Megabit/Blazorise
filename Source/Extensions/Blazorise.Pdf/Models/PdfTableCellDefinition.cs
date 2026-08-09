#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes a PDF table cell.
/// </summary>
public sealed class PdfTableCellDefinition
{
    #region Properties

    /// <summary>
    /// Cell width in points.
    /// </summary>
    public double Width { get; set; } = 90;

    /// <summary>
    /// Elements rendered inside the cell.
    /// </summary>
    public List<PdfElementDefinition> Elements { get; set; } = [];

    #endregion
}