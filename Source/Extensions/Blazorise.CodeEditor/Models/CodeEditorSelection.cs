namespace Blazorise.CodeEditor;

/// <summary>
/// Represents a selection range in the code editor.
/// </summary>
public class CodeEditorSelection
{
    /// <summary>
    /// Gets or sets the starting line number.
    /// </summary>
    public int StartLineNumber { get; set; }

    /// <summary>
    /// Gets or sets the starting column.
    /// </summary>
    public int StartColumn { get; set; }

    /// <summary>
    /// Gets or sets the ending line number.
    /// </summary>
    public int EndLineNumber { get; set; }

    /// <summary>
    /// Gets or sets the ending column.
    /// </summary>
    public int EndColumn { get; set; }
}