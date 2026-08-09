using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    /// Gets or sets the callback used to provide contextual completion items.
    /// </summary>
    [JsonIgnore]
    public Func<CodeEditorCompletionContext, Task<IReadOnlyList<CodeEditorCompletionItem>>> ItemsProvider { get; set; }

    /// <summary>
    /// Gets whether a contextual completion items provider is configured.
    /// </summary>
    public bool UseItemsProvider => ItemsProvider is not null;
}