#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Represents a generated PDF result.
/// </summary>
public sealed class PdfGenerationResult
{
    #region Properties

    /// <summary>
    /// Generated PDF bytes returned by in-memory generation.
    /// </summary>
    public byte[] Content { get; set; }

    /// <summary>
    /// Result content type.
    /// </summary>
    public string ContentType { get; set; } = "application/pdf";

    /// <summary>
    /// Suggested file name for the generated PDF.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Normalization messages produced while preparing the document.
    /// </summary>
    public IReadOnlyList<string> Diagnostics { get; set; } = [];

    #endregion
}