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
    /// Copies and inserts the selected element.
    /// </summary>
    Duplicate,

    /// <summary>
    /// Starts inline editing for the selected text element.
    /// </summary>
    EditText,

    /// <summary>
    /// Moves the current element selection down.
    /// </summary>
    MoveDown,

    /// <summary>
    /// Moves the current element selection left.
    /// </summary>
    MoveLeft,

    /// <summary>
    /// Moves the current element selection right.
    /// </summary>
    MoveRight,

    /// <summary>
    /// Moves the current element selection up.
    /// </summary>
    MoveUp,

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