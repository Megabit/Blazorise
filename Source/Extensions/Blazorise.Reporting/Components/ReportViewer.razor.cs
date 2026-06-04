using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public partial class ReportViewer : ComponentBase
{
    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    protected override void OnParametersSet()
    {
        if ( ReportContext is null )
            return;

        ReportContext.ViewerOptions.PreviewFormats = PreviewFormats;
        ReportContext.ViewerOptions.DefaultFormat = DefaultFormat;
        ReportContext.ViewerOptions.AllowPrint = AllowPrint;
        ReportContext.ViewerOptions.AllowDownload = AllowDownload;
    }

    [Parameter] public ReportPreviewFormat PreviewFormats { get; set; } = ReportPreviewFormat.Html;

    [Parameter] public ReportPreviewFormat DefaultFormat { get; set; } = ReportPreviewFormat.Html;

    [Parameter] public bool AllowPrint { get; set; } = true;

    [Parameter] public bool AllowDownload { get; set; } = true;
}