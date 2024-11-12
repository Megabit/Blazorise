namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing an input mask on a text input field.
/// </summary>
public class InputMaskJSOptions
{
    /// <summary>
    /// Gets or sets the mask pattern that defines the input format.
    /// </summary>
    public string Mask { get; set; }

    /// <summary>
    /// Gets or sets a regular expression pattern to match valid inputs.
    /// </summary>
    public string Regex { get; set; }

    /// <summary>
    /// Gets or sets an alias that can be used to pre-define certain mask settings.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the format for displaying the input.
    /// </summary>
    public string InputFormat { get; set; }

    /// <summary>
    /// Gets or sets the format for outputting the masked value.
    /// </summary>
    public string OutputFormat { get; set; }

    /// <summary>
    /// Gets or sets the placeholder character(s) used within the mask.
    /// </summary>
    public string MaskPlaceholder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the mask is displayed when the input is focused.
    /// </summary>
    public bool ShowMaskOnFocus { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the mask is displayed when the input is hovered.
    /// </summary>
    public bool ShowMaskOnHover { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the input accepts numeric input only.
    /// </summary>
    public bool NumericInput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the input is right-aligned.
    /// </summary>
    public bool RightAlign { get; set; }

    /// <summary>
    /// Gets or sets the character used as a decimal separator in numeric inputs.
    /// </summary>
    public string DecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets the character used as a group separator in numeric inputs.
    /// </summary>
    public string GroupSeparator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the input can be null.
    /// </summary>
    public bool Nullable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically unmask the value when retrieving it.
    /// </summary>
    public bool AutoUnmask { get; set; }

    /// <summary>
    /// Gets or sets the behavior of the caret position when clicking inside the input.
    /// </summary>
    public string PositionCaretOnClick { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the mask should be cleared when the input loses focus.
    /// </summary>
    public bool ClearMaskOnLostFocus { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether incomplete input should be cleared automatically.
    /// </summary>
    public bool ClearIncomplete { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the input is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the input is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }
}