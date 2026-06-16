using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines completion behavior for a code editor instance.
/// </summary>
public class CodeEditorCompletionProvider
{
    /// <summary>
    /// Gets or sets the language identifier.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the characters that trigger completion.
    /// </summary>
    public IReadOnlyList<string> TriggerCharacters { get; set; }

    /// <summary>
    /// Gets or sets static completion items.
    /// </summary>
    public IReadOnlyList<CodeEditorCompletionItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the custom JavaScript method used to provide completion items.
    /// </summary>
    public string ProviderMethod { get; set; }
}