using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines global options for the CodeEditor extension.
/// </summary>
public class CodeEditorExtensionOptions
{
    /// <summary>
    /// Gets or sets the base URL where the code editor runtime assets are located.
    /// </summary>
    public string AssetsPath { get; set; } = "_content/Blazorise.CodeEditor/vendors/monaco/min/vs";

    /// <summary>
    /// Gets or sets globally registered custom languages.
    /// </summary>
    public IReadOnlyList<CodeEditorLanguageDefinition> Languages { get; set; }
}