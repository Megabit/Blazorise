namespace Blazorise.Reporting;

/// <summary>
/// Defines commands that can be issued from report toolbars.
/// </summary>
public enum ReportCommand
{
    /// <summary>
    /// Switches to design mode.
    /// </summary>
    Design,

    /// <summary>
    /// Switches to the default preview mode.
    /// </summary>
    Preview,

    /// <summary>
    /// Switches to HTML preview mode.
    /// </summary>
    PreviewHtml,

    /// <summary>
    /// Switches to PDF preview mode.
    /// </summary>
    PreviewPdf,

    /// <summary>
    /// Opens the data source connection dialog.
    /// </summary>
    ConnectDataSource,

    /// <summary>
    /// Generates and downloads a PDF file from the current report.
    /// </summary>
    DownloadPdf,

    /// <summary>
    /// Removes the selected element and places it on the report clipboard.
    /// </summary>
    Cut,

    /// <summary>
    /// Copies the selected element to the report clipboard.
    /// </summary>
    Copy,

    /// <summary>
    /// Copies and inserts the selected element.
    /// </summary>
    Duplicate,

    /// <summary>
    /// Inserts the report clipboard element.
    /// </summary>
    Paste,

    /// <summary>
    /// Removes the current report selection.
    /// </summary>
    Delete,

    /// <summary>
    /// Reverts the previous designer command.
    /// </summary>
    Undo,

    /// <summary>
    /// Reapplies the previously undone designer command.
    /// </summary>
    Redo,

    /// <summary>
    /// Restores the report to its seed definition.
    /// </summary>
    Reset
}