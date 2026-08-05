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
    #region Members

    internal const int DefaultMaxPages = 10000;

    internal const int DefaultMaxDefinitionNodes = 100000;

    internal const long DefaultMaxTextLength = 10000000;

    internal const long DefaultMaxResourceSize = 20 * 1024 * 1024;

    internal const long DefaultMaxTotalResourceSize = 200 * 1024 * 1024;

    internal const long DefaultMaxImagePixels = 40000000;

    #endregion

    #region Properties

    /// <summary>
    /// Suggested output file name.
    /// </summary>
    public string FileName { get; set; } = "document.pdf";

    /// <summary>
    /// Callback invoked when PDF generation progress changes.
    /// </summary>
    public Func<PdfGenerationProgress, Task> Progress { get; set; }

    /// <summary>
    /// Maximum number of pages allowed in one document.
    /// </summary>
    public int MaxPages { get; set; } = DefaultMaxPages;

    /// <summary>
    /// Maximum number of definition nodes allowed in one document.
    /// </summary>
    public int MaxDefinitionNodes { get; set; } = DefaultMaxDefinitionNodes;

    /// <summary>
    /// Maximum combined number of text characters allowed in one document.
    /// </summary>
    public long MaxTextLength { get; set; } = DefaultMaxTextLength;

    /// <summary>
    /// Maximum number of bytes allowed for one image or font.
    /// </summary>
    public long MaxResourceSize { get; set; } = DefaultMaxResourceSize;

    /// <summary>
    /// Maximum combined number of image and font bytes allowed in one document.
    /// </summary>
    public long MaxTotalResourceSize { get; set; } = DefaultMaxTotalResourceSize;

    /// <summary>
    /// Maximum number of pixels allowed in one image.
    /// </summary>
    public long MaxImagePixels { get; set; } = DefaultMaxImagePixels;

    #endregion
}