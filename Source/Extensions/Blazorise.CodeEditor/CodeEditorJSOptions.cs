using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Represents JavaScript options for initializing or updating a code editor component.
/// </summary>
public class CodeEditorJSOptions
{
    public string AssetsPath { get; set; }
    public string Value { get; set; }
    public string Language { get; set; }
    public string Theme { get; set; }
    public bool ReadOnly { get; set; }
    public bool Disabled { get; set; }
    public int? TabIndex { get; set; }
    public string AriaInvalid { get; set; }
    public string AriaRequired { get; set; }
    public string AriaDescribedBy { get; set; }
    public string AriaLabelledBy { get; set; }
    public bool Immediate { get; set; }
    public bool Debounce { get; set; }
    public int DebounceInterval { get; set; }
    public bool AutomaticLayout { get; set; }
    public bool Minimap { get; set; }
    public bool LineNumbers { get; set; }
    public bool WordWrap { get; set; }
    public int TabSize { get; set; }
    public bool InsertSpaces { get; set; }
    public bool FormatOnPaste { get; set; }
    public bool FormatOnType { get; set; }
    public bool RenderWhitespace { get; set; }
    public bool ScrollBeyondLastLine { get; set; }
    public string FontFamily { get; set; }
    public int? FontSize { get; set; }
    public string ConfigureEditorMethod { get; set; }
    public Dictionary<string, object> AdditionalOptions { get; set; }
    public IReadOnlyList<CodeEditorLanguageDefinition> Languages { get; set; }
    public CodeEditorCompletionProvider CompletionProvider { get; set; }
    public CodeEditorDocumentFormattingProvider FormattingProvider { get; set; }
}