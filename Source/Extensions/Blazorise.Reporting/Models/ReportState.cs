namespace Blazorise.Reporting;

public sealed class ReportState
{
    public ReportDefinition Definition { get; set; }

    public ReportStudioMode Mode { get; set; }

    public ReportPreviewFormat PreviewFormat { get; set; }

    public bool SnapToGrid { get; set; }

    public ReportSelectionState Selection { get; set; } = new();

    public ReportElementDefinition ClipboardElement { get; set; }

    public bool CanUndo { get; set; }

    public bool CanRedo { get; set; }
}

public sealed class ReportSelectionState
{
    public ReportSelectionType Type { get; set; } = ReportSelectionType.Report;

    public string SectionId { get; set; }

    public string ElementId { get; set; }
}