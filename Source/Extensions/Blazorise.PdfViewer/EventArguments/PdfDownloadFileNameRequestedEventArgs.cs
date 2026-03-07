#region Using directives
using System;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Provides data when a download filename is requested for a PDF document.
/// </summary>
public class PdfDownloadFileNameRequestedEventArgs : EventArgs
{
    #region Constructors

    /// <summary>
    /// A default <see cref="PdfDownloadFileNameRequestedEventArgs"/> constructor.
    /// </summary>
    /// <param name="source">The source of the PDF currently being downloaded.</param>
    /// <param name="documentTitle">The title metadata of the PDF document.</param>
    /// <param name="suggestedFileName">The suggested filename for the download.</param>
    public PdfDownloadFileNameRequestedEventArgs( string source, string documentTitle, string suggestedFileName )
    {
        Source = source;
        DocumentTitle = documentTitle;
        SuggestedFileName = suggestedFileName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the source of the PDF document.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets the title metadata of the PDF document.
    /// </summary>
    public string DocumentTitle { get; }

    /// <summary>
    /// Gets the suggested filename for the PDF download.
    /// </summary>
    public string SuggestedFileName { get; }

    #endregion
}