namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing a memo edit (multi-line text editor) component.
/// </summary>
public class MemoEditJSOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the Tab key should be replaced with spaces or used to navigate.
    /// When true, pressing Tab will insert spaces according to <see cref="TabSize"/>.
    /// </summary>
    public bool ReplaceTab { get; set; }

    /// <summary>
    /// Gets or sets the number of spaces to insert when the Tab key is pressed, if <see cref="ReplaceTab"/> is enabled.
    /// </summary>
    public int TabSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use spaces instead of actual Tab characters.
    /// </summary>
    public bool SoftTabs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor should automatically adjust its height based on content.
    /// </summary>
    public bool AutoSize { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of a memo edit (multi-line text editor) component.
/// </summary>
public class MemoEditUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the Tab replacement setting.
    /// </summary>
    public JSOptionChange<bool> ReplaceTab { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the Tab size.
    /// </summary>
    public JSOptionChange<int> TabSize { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the soft tabs setting.
    /// </summary>
    public JSOptionChange<bool> SoftTabs { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the auto-size behavior of the editor.
    /// </summary>
    public JSOptionChange<bool> AutoSize { get; set; }
}