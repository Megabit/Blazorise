namespace Blazorise.CodeEditor;

/// <summary>
/// Defines completion item kinds.
/// </summary>
public enum CodeEditorCompletionItemKind
{
    /// <summary>Callable instance method.</summary>
    Method = 0,
    /// <summary>Standalone or local function.</summary>
    Function = 1,
    /// <summary>Type constructor.</summary>
    Constructor = 2,
    /// <summary>Type or object field.</summary>
    Field = 3,
    /// <summary>Local or scoped variable.</summary>
    Variable = 4,
    /// <summary>Class type.</summary>
    Class = 5,
    /// <summary>Structure type.</summary>
    Struct = 6,
    /// <summary>Interface contract.</summary>
    Interface = 7,
    /// <summary>Module or namespace-like container.</summary>
    Module = 8,
    /// <summary>Readable or writable property.</summary>
    Property = 9,
    /// <summary>Event member.</summary>
    Event = 10,
    /// <summary>Language operator.</summary>
    Operator = 11,
    /// <summary>Unit-valued suggestion.</summary>
    Unit = 12,
    /// <summary>Literal or computed value.</summary>
    Value = 13,
    /// <summary>Named constant.</summary>
    Constant = 14,
    /// <summary>Enumeration type.</summary>
    Enum = 15,
    /// <summary>Named enumeration value.</summary>
    EnumMember = 16,
    /// <summary>Reserved language keyword.</summary>
    Keyword = 17,
    /// <summary>Plain text suggestion.</summary>
    Text = 18,
    /// <summary>Color value or swatch.</summary>
    Color = 19,
    /// <summary>File-system file.</summary>
    File = 20,
    /// <summary>Reference to another symbol.</summary>
    Reference = 21,
    /// <summary>Application-specific color value.</summary>
    CustomColor = 22,
    /// <summary>File-system folder.</summary>
    Folder = 23,
    /// <summary>Generic type parameter.</summary>
    TypeParameter = 24,
    /// <summary>User identity.</summary>
    User = 25,
    /// <summary>Issue or work item.</summary>
    Issue = 26,
    /// <summary>Expandable code snippet.</summary>
    Snippet = 27
}