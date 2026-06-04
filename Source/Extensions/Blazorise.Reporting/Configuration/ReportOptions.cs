namespace Blazorise.Reporting;

public sealed class ReportOptions
{
    public ReportPreviewFormat PreviewFormats { get; set; } = ReportPreviewFormat.Html;

    public ReportPreviewFormat DefaultPreviewFormat { get; set; } = ReportPreviewFormat.Html;

    public ReportDefinitionMode DefinitionMode { get; set; } = ReportDefinitionMode.SeedWhenEmpty;

    public bool DesignerEnabled { get; set; }

    public bool AllowPrint { get; set; } = true;

    public bool AllowDownload { get; set; } = true;
}