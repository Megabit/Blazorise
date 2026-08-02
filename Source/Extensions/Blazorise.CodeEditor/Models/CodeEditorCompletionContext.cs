namespace Blazorise.CodeEditor;

/// <summary>
/// Contains the editor state available while providing contextual completion items.
/// </summary>
public class CodeEditorCompletionContext
{
    /// <summary>
    /// Gets or sets the current document value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the content of the current line.
    /// </summary>
    public string LineText { get; set; }

    /// <summary>
    /// Gets or sets the one-based line number of the cursor.
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the one-based column of the cursor.
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Gets or sets the word immediately before the cursor.
    /// </summary>
    public string Word { get; set; }

    /// <summary>
    /// Gets or sets the character that triggered completion, when applicable.
    /// </summary>
    public string TriggerCharacter { get; set; }
}