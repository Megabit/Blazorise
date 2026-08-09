namespace Blazorise.CodeEditor;

/// <summary>
/// Defines the document range replaced by a completion item.
/// </summary>
public class CodeEditorCompletionRange
{
    /// <summary>
    /// Gets or sets the one-based start line number.
    /// </summary>
    public int StartLineNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the one-based start column.
    /// </summary>
    public int StartColumn { get; set; } = 1;

    /// <summary>
    /// Gets or sets the one-based end line number.
    /// </summary>
    public int EndLineNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the one-based end column.
    /// </summary>
    public int EndColumn { get; set; } = 1;
}