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
    /// <remarks>
    /// Runtime assets are loaded once per page. The first code editor determines the asset path.
    /// </remarks>
    public string AssetsPath { get; set; } = "_content/Blazorise.CodeEditor/vendors/monaco/min/vs";

    /// <summary>
    /// Gets or sets globally registered custom languages.
    /// </summary>
    /// <remarks>
    /// Custom language registrations are global to the page. Use one definition per language identifier.
    /// </remarks>
    public IReadOnlyList<CodeEditorLanguageDefinition> Languages { get; set; }
}