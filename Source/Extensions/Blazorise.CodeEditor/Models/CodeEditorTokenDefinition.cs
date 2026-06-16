namespace Blazorise.CodeEditor;

/// <summary>
/// Defines a single declarative tokenizer rule.
/// </summary>
public class CodeEditorTokenDefinition
{
    /// <summary>
    /// Gets or sets the regular expression pattern.
    /// </summary>
    public string Pattern { get; set; }

    /// <summary>
    /// Gets or sets the token name.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Gets or sets the next tokenizer state.
    /// </summary>
    public string Next { get; set; }

    /// <summary>
    /// Gets or sets the bracket token action.
    /// </summary>
    public string Bracket { get; set; }
}