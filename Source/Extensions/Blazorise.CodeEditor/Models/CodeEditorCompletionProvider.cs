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
    /// <remarks>
    /// The method receives the editor, model, position, completion context, static suggestions, and cancellation token.
    /// It can return suggestions, a Monaco completion result, or a promise for either value.
    /// </remarks>
    public string ProviderMethod { get; set; }
}