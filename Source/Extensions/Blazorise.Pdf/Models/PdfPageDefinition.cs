#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Describes one PDF page.
/// </summary>
public sealed class PdfPageDefinition
{
    #region Properties

    /// <summary>
    /// Page size used by this page.
    /// </summary>
    public PdfPageSize Size { get; set; } = PdfPageSize.Custom;

    /// <summary>
    /// Page orientation used by this page.
    /// </summary>
    public PdfOrientation Orientation { get; set; } = PdfOrientation.Portrait;

    /// <summary>
    /// Page width in points.
    /// </summary>
    public double Width { get; set; } = PdfPageMetrics.A4Width;

    /// <summary>
    /// Page height in points.
    /// </summary>
    public double Height { get; set; } = PdfPageMetrics.A4Height;

    /// <summary>
    /// Elements rendered on the page.
    /// </summary>
    public List<PdfElementDefinition> Elements { get; set; } = [];

    #endregion
}