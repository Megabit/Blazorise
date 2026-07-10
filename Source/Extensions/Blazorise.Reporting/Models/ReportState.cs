using System.Collections.Generic;

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
    public ReportMode Mode { get; set; }

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
    /// Elements stored by cut or copy commands.
    /// </summary>
    public List<ReportElementDefinition> ClipboardElements { get; set; } = [];

    /// <summary>
    /// Band identifier that originally contained the clipboard elements.
    /// </summary>
    public string ClipboardBandId { get; set; }

    /// <summary>
    /// Indicates whether the command history can undo the latest operation.
    /// </summary>
    public bool CanUndo { get; set; }

    /// <summary>
    /// Indicates whether the command history can redo the latest undone operation.
    /// </summary>
    public bool CanRedo { get; set; }
}