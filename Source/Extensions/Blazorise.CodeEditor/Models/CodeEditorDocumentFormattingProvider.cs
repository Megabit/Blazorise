using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines document formatting behavior for a code editor instance.
/// </summary>
public class CodeEditorDocumentFormattingProvider
{
    /// <summary>
    /// Gets or sets the language identifier.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the .NET function used to format the document.
    /// </summary>
    /// <remarks>
    /// The function receives the current document value and returns the formatted value.
    /// </remarks>
    [JsonIgnore]
    public Func<string, Task<string>> Formatter { get; set; }

    /// <summary>
    /// Indicates whether the provider uses a .NET formatter.
    /// </summary>
    public bool UseFormatter => Formatter is not null;
}