namespace Blazorise.Reporting;

/// <summary>
/// Captures the full interactive state of a report designer instance.
/// </summary>
public sealed class ReportState
{
    /// <summary>
    /// Report definition currently edited or previewed.
    /// </summary>
    public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Active design or preview surface.
    /// </summary>
    public ReportStudioMode Mode { get; set; }

    /// <summary>
    /// Preview format selected by the viewer.
    /// </summary>
    public ReportPreviewFormat PreviewFormat { get; set; }

    /// <summary>
    /// Enables grid-aligned movement and resizing in the designer.
    /// </summary>
    public bool SnapToGrid { get; set; }

    /// <summary>
    /// Current report, band, or element selection.
    /// </summary>
    public ReportSelectionState Selection { get; set; } = new();

    /// <summary>
    /// Element stored by cut or copy commands.
    /// </summary>
    public ReportElementDefinition ClipboardElement { get; set; }

    /// <summary>
    /// Indicates whether the command history can undo the latest operation.
    /// </summary>
    public bool CanUndo { get; set; }

    /// <summary>
    /// Indicates whether the command history can redo the latest undone operation.
    /// </summary>
    public bool CanRedo { get; set; }
}

/// <summary>
/// Identifies the selected object inside the report designer.
/// </summary>
public sealed class ReportSelectionState
{
    /// <summary>
    /// Kind of report object currently selected.
    /// </summary>
    public ReportSelectionType Type { get; set; } = ReportSelectionType.Report;

    /// <summary>
    /// Selected band identifier when a band or band element is selected.
    /// </summary>
    public string SectionId { get; set; }

    /// <summary>
    /// Selected element identifier when an element is selected.
    /// </summary>
    public string ElementId { get; set; }
}