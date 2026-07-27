#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Options used when generating a PDF document.
/// </summary>
public sealed class PdfGenerationOptions
{
    #region Properties

    /// <summary>
    /// Suggested output file name.
    /// </summary>
    public string FileName { get; set; } = "document.pdf";

    /// <summary>
    /// Callback invoked when PDF generation progress changes.
    /// </summary>
    public Func<PdfGenerationProgress, Task> Progress { get; set; }

    #endregion
}