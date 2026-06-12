namespace Blazorise.Reporting.Internal;

/// <summary>
/// Identifies keyboard shortcuts handled by the report designer.
/// </summary>
public enum ReportDesignerShortcut
{
    /// <summary>
    /// Copies the selected element to the report clipboard.
    /// </summary>
    Copy,

    /// <summary>
    /// Cuts the selected element to the report clipboard.
    /// </summary>
    Cut,

    /// <summary>
    /// Deletes the current element selection.
    /// </summary>
    Delete,

    /// <summary>
    /// Starts inline editing for the selected text element.
    /// </summary>
    EditText,

    /// <summary>
    /// Pastes the report clipboard into the current selection context.
    /// </summary>
    Paste,

    /// <summary>
    /// Reapplies the next history action.
    /// </summary>
    Redo,

    /// <summary>
    /// Reverts the last history action.
    /// </summary>
    Undo,
}