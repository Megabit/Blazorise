#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides the generated PDF content and viewer options to a custom PDF preview template.
/// </summary>
public sealed class ReportPdfPreviewContext
{
    #region Members

    private readonly EventCallback download;

    private string dataUrl;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a report PDF preview template context.
    /// </summary>
    /// <param name="content">Generated PDF bytes.</param>
    /// <param name="contentType">Generated PDF content type.</param>
    /// <param name="fileName">Suggested PDF file name.</param>
    /// <param name="allowPrint">Whether printing is enabled.</param>
    /// <param name="allowDownload">Whether downloading is enabled.</param>
    /// <param name="download">Callback that downloads the generated PDF.</param>
    public ReportPdfPreviewContext( byte[] content, string contentType, string fileName, bool allowPrint, bool allowDownload, EventCallback download )
    {
        Content = content;
        ContentType = contentType;
        FileName = fileName;
        AllowPrint = allowPrint;
        AllowDownload = allowDownload;
        this.download = download;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Downloads the generated PDF when downloading is enabled.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Download()
        => AllowDownload ? download.InvokeAsync() : Task.CompletedTask;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the generated PDF bytes.
    /// </summary>
    public byte[] Content { get; }

    /// <summary>
    /// Gets the generated PDF content type.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// Gets the suggested PDF file name.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the generated PDF as a data URL suitable for viewer source parameters.
    /// </summary>
    public string DataUrl
        => dataUrl ??= Content is { Length: > 0 }
            ? $"data:{ContentType};base64,{Convert.ToBase64String( Content )}"
            : null;

    /// <summary>
    /// Gets whether printing is enabled for the PDF preview.
    /// </summary>
    public bool AllowPrint { get; }

    /// <summary>
    /// Gets whether downloading is enabled for the PDF preview.
    /// </summary>
    public bool AllowDownload { get; }

    #endregion
}