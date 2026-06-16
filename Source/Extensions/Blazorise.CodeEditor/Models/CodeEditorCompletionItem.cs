using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines a completion item.
/// </summary>
public class CodeEditorCompletionItem
{
    /// <summary>
    /// Gets or sets the completion label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the text inserted when completion is accepted.
    /// </summary>
    public string InsertText { get; set; }

    /// <summary>
    /// Gets or sets the completion item kind.
    /// </summary>
    public CodeEditorCompletionItemKind Kind { get; set; } = CodeEditorCompletionItemKind.Text;

    /// <summary>
    /// Gets or sets the completion item detail text.
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// Gets or sets the completion item documentation.
    /// </summary>
    public string Documentation { get; set; }

    /// <summary>
    /// Gets or sets the text used to filter the item.
    /// </summary>
    public string FilterText { get; set; }

    /// <summary>
    /// Gets or sets the text used to sort the item.
    /// </summary>
    public string SortText { get; set; }

    /// <summary>
    /// Gets or sets characters that commit the completion item.
    /// </summary>
    public IReadOnlyList<string> CommitCharacters { get; set; }

    /// <summary>
    /// Gets or sets insert text rules.
    /// </summary>
    public CodeEditorCompletionItemInsertTextRule InsertTextRules { get; set; }
}