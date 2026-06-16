using System;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines completion item insert text rules.
/// </summary>
[Flags]
public enum CodeEditorCompletionItemInsertTextRule
{
    None = 0,
    KeepWhitespace = 1,
    InsertAsSnippet = 4
}