using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Represents JavaScript options for initializing or updating a code editor component.
/// </summary>
public class CodeEditorJSOptions
{
    /// <summary>Base URL from which Monaco worker and editor assets are loaded.</summary>
    public string AssetsPath { get; set; }
    /// <summary>Source text initially displayed by the editor.</summary>
    public string Value { get; set; }
    /// <summary>Language identifier used for tokenization and editor services.</summary>
    public string Language { get; set; }
    /// <summary>Color theme applied to the editor surface.</summary>
    public string Theme { get; set; }
    /// <summary>Whether source changes are blocked while navigation remains available.</summary>
    public bool ReadOnly { get; set; }
    /// <summary>Whether the editor rejects all user interaction.</summary>
    public bool Disabled { get; set; }
    /// <summary>Keyboard navigation order assigned to the editor.</summary>
    public int? TabIndex { get; set; }
    /// <summary>Accessible invalid-state value forwarded to the editor input.</summary>
    public string AriaInvalid { get; set; }
    /// <summary>Accessible required-state value forwarded to the editor input.</summary>
    public string AriaRequired { get; set; }
    /// <summary>Identifiers of elements describing the editor.</summary>
    public string AriaDescribedBy { get; set; }
    /// <summary>Identifiers of elements labelling the editor.</summary>
    public string AriaLabelledBy { get; set; }
    /// <summary>Whether value changes are reported while typing.</summary>
    public bool Immediate { get; set; }
    /// <summary>Whether rapid value changes are consolidated before notification.</summary>
    public bool Debounce { get; set; }
    /// <summary>Milliseconds waited before a debounced value notification.</summary>
    public int DebounceInterval { get; set; }
    /// <summary>Whether Monaco observes its container and recalculates layout automatically.</summary>
    public bool AutomaticLayout { get; set; }
    /// <summary>Whether the source minimap is visible.</summary>
    public bool Minimap { get; set; }
    /// <summary>Whether line numbers appear beside source text.</summary>
    public bool LineNumbers { get; set; }
    /// <summary>Whether long lines wrap within the viewport.</summary>
    public bool WordWrap { get; set; }
    /// <summary>Number of columns represented by one tab stop.</summary>
    public int TabSize { get; set; }
    /// <summary>Whether indentation inserts spaces instead of tab characters.</summary>
    public bool InsertSpaces { get; set; }
    /// <summary>Whether pasted source is formatted immediately.</summary>
    public bool FormatOnPaste { get; set; }
    /// <summary>Whether supported trigger characters format the active line.</summary>
    public bool FormatOnType { get; set; }
    /// <summary>Whether spaces and tabs are drawn as visible glyphs.</summary>
    public bool RenderWhitespace { get; set; }
    /// <summary>Whether scrolling may continue below the final source line.</summary>
    public bool ScrollBeyondLastLine { get; set; }
    /// <summary>CSS font-family stack used for source text.</summary>
    public string FontFamily { get; set; }
    /// <summary>Source text size in pixels.</summary>
    public int? FontSize { get; set; }
    /// <summary>Additional Monaco options merged after the typed settings.</summary>
    public Dictionary<string, object> AdditionalOptions { get; set; }
    /// <summary>Custom language definitions registered before editor creation.</summary>
    public IReadOnlyList<CodeEditorLanguageDefinition> Languages { get; set; }
    /// <summary>Completion provider exposed to Monaco for the active language.</summary>
    public CodeEditorCompletionProvider CompletionProvider { get; set; }
    /// <summary>Document formatter exposed to Monaco for the active language.</summary>
    public CodeEditorDocumentFormattingProvider FormattingProvider { get; set; }
}