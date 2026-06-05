using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Configures preview behavior for the containing report.
/// </summary>
public partial class ReportViewer : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( ReportContext is null )
            return;

        ReportContext.ViewerOptions.PreviewFormats = PreviewFormats;
        ReportContext.ViewerOptions.DefaultFormat = DefaultFormat;
        ReportContext.ViewerOptions.AllowPrint = AllowPrint;
        ReportContext.ViewerOptions.AllowDownload = AllowDownload;
    }

    /// <summary>
    /// Preview formats offered by the report viewer.
    /// </summary>
    [Parameter] public ReportPreviewFormat PreviewFormats { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Preview format selected when preview mode is opened.
    /// </summary>
    [Parameter] public ReportPreviewFormat DefaultFormat { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Enables print commands in the viewer toolbar.
    /// </summary>
    [Parameter] public bool AllowPrint { get; set; } = true;

    /// <summary>
    /// Enables download commands in the viewer toolbar.
    /// </summary>
    [Parameter] public bool AllowDownload { get; set; } = true;
}