using System;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines completion item insert text rules.
/// </summary>
[Flags]
public enum CodeEditorCompletionItemInsertTextRule
{
    /// <summary>Uses Monaco's default insertion behavior.</summary>
    None = 0,
    /// <summary>Preserves whitespace in the proposed insertion text.</summary>
    KeepWhitespace = 1,
    /// <summary>Interprets insertion text as a Monaco snippet.</summary>
    InsertAsSnippet = 4
}