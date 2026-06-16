using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines a custom code editor language.
/// </summary>
public class CodeEditorLanguageDefinition
{
    /// <summary>
    /// Gets or sets the language identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the language aliases.
    /// </summary>
    public IReadOnlyList<string> Aliases { get; set; }

    /// <summary>
    /// Gets or sets the file extensions associated with the language.
    /// </summary>
    public IReadOnlyList<string> Extensions { get; set; }

    /// <summary>
    /// Gets or sets the MIME types associated with the language.
    /// </summary>
    public IReadOnlyList<string> MimeTypes { get; set; }

    /// <summary>
    /// Gets or sets the declarative tokenizer definition.
    /// </summary>
    public CodeEditorTokenizerDefinition Tokenizer { get; set; }

    /// <summary>
    /// Gets or sets the custom JavaScript method used to configure advanced language features.
    /// </summary>
    public string ConfigureLanguageMethod { get; set; }
}