using System.Collections.Generic;

namespace Blazorise.CodeEditor;

/// <summary>
/// Defines options for the <see cref="CodeEditor"/> component.
/// </summary>
public class CodeEditorOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the editor should automatically layout when its container size changes.
    /// </summary>
    public bool AutomaticLayout { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the minimap should be visible.
    /// </summary>
    public bool Minimap { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether line numbers should be visible.
    /// </summary>
    public bool LineNumbers { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether long lines should wrap.
    /// </summary>
    public bool WordWrap { get; set; }

    /// <summary>
    /// Gets or sets the tab size.
    /// </summary>
    public int TabSize { get; set; } = 4;

    /// <summary>
    /// Gets or sets a value indicating whether spaces should be inserted instead of tabs.
    /// </summary>
    public bool InsertSpaces { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether formatting should run on paste.
    /// </summary>
    public bool FormatOnPaste { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether formatting should run while typing.
    /// </summary>
    public bool FormatOnType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether whitespace characters should be rendered.
    /// </summary>
    public bool RenderWhitespace { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor should allow scrolling past the last line.
    /// </summary>
    public bool ScrollBeyondLastLine { get; set; } = true;

    /// <summary>
    /// Gets or sets the editor font family.
    /// </summary>
    public string FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the editor font size.
    /// </summary>
    public int? FontSize { get; set; }

    /// <summary>
    /// Gets or sets additional editor-specific options.
    /// </summary>
    public Dictionary<string, object> AdditionalOptions { get; set; }
}