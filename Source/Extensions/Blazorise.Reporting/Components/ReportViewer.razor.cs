#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Configures preview behavior for the containing report.
/// </summary>
public partial class ReportViewer : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( ReportContext is null )
            return;

        ReportContext.ViewerOptions.PreviewFormats = PreviewFormat;
        ReportContext.ViewerOptions.DefaultFormat = DefaultPreviewFormat;
        ReportContext.ViewerOptions.AllowPrint = AllowPrint;
        ReportContext.ViewerOptions.AllowDownload = AllowDownload;
        ReportContext.ViewerOptions.PdfPreviewTemplate = PdfPreviewTemplate;
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Preview formats offered by the report viewer.
    /// </summary>
    [Parameter] public ReportPreviewFormat PreviewFormat { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Preview format selected when preview mode is opened.
    /// </summary>
    [Parameter] public ReportPreviewFormat DefaultPreviewFormat { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Enables print commands in the viewer toolbar.
    /// </summary>
    [Parameter] public bool AllowPrint { get; set; } = true;

    /// <summary>
    /// Enables download commands in the viewer toolbar.
    /// </summary>
    [Parameter] public bool AllowDownload { get; set; } = true;

    /// <summary>
    /// Template used to render generated PDF previews.
    /// </summary>
    [Parameter] public RenderFragment<ReportPdfPreviewContext> PdfPreviewTemplate { get; set; }

    #endregion
}