#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Captures a report definition together with transient designer state.
/// </summary>
public sealed class ReportState
{
    #region Properties

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

    #endregion
}