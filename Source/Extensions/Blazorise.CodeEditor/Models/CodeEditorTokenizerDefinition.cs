using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines a Monarch tokenizer for a custom code editor language.
/// </summary>
public class CodeEditorTokenizerDefinition
{
    /// <summary>
    /// Gets or sets a value indicating whether token matching should ignore casing.
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tokenizer regular expressions are unicode aware.
    /// </summary>
    public bool Unicode { get; set; }

    /// <summary>
    /// Gets or sets the fallback token name.
    /// </summary>
    public string DefaultToken { get; set; }

    /// <summary>
    /// Gets or sets the root tokenizer rules.
    /// </summary>
    public IReadOnlyList<CodeEditorTokenDefinition> Tokens { get; set; }
}