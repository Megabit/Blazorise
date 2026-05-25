namespace Blazorise;

/// <summary>
/// Provides context for rendering a custom on-screen keyboard key.
/// </summary>
public class OnScreenKeyboardKeyContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnScreenKeyboardKeyContext"/> class.
    /// </summary>
    /// <param name="key">The rendered keyboard key.</param>
    /// <param name="displayText">The resolved display text.</param>
    /// <param name="active">Indicates whether the key is active.</param>
    /// <param name="shift">Indicates whether shift is enabled.</param>
    public OnScreenKeyboardKeyContext( OnScreenKeyboardKey key, string displayText, bool active, bool shift )
        : this( key, displayText, null, active, shift, false )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnScreenKeyboardKeyContext"/> class.
    /// </summary>
    /// <param name="key">The rendered keyboard key.</param>
    /// <param name="displayText">The resolved display text.</param>
    /// <param name="active">Indicates whether the key is active.</param>
    /// <param name="shift">Indicates whether shift is enabled.</param>
    /// <param name="specialCharacters">Indicates whether the special characters keyboard is enabled.</param>
    public OnScreenKeyboardKeyContext( OnScreenKeyboardKey key, string displayText, bool active, bool shift, bool specialCharacters )
        : this( key, displayText, null, active, shift, specialCharacters )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnScreenKeyboardKeyContext"/> class.
    /// </summary>
    /// <param name="key">The rendered keyboard key.</param>
    /// <param name="displayText">The resolved display text.</param>
    /// <param name="ariaLabel">The resolved accessible label.</param>
    /// <param name="active">Indicates whether the key is active.</param>
    /// <param name="shift">Indicates whether shift is enabled.</param>
    /// <param name="specialCharacters">Indicates whether the special characters keyboard is enabled.</param>
    public OnScreenKeyboardKeyContext( OnScreenKeyboardKey key, string displayText, string ariaLabel, bool active, bool shift, bool specialCharacters )
    {
        Key = key;
        DisplayText = displayText;
        AriaLabel = ariaLabel;
        Active = active;
        Shift = shift;
        SpecialCharacters = specialCharacters;
    }

    /// <summary>
    /// Gets the rendered keyboard key.
    /// </summary>
    public OnScreenKeyboardKey Key { get; }

    /// <summary>
    /// Gets the resolved display text for the key.
    /// </summary>
    public string DisplayText { get; }

    /// <summary>
    /// Gets the resolved accessible label for the key.
    /// </summary>
    public string AriaLabel { get; }

    /// <summary>
    /// Gets whether the key is active.
    /// </summary>
    public bool Active { get; }

    /// <summary>
    /// Gets whether shift is enabled.
    /// </summary>
    public bool Shift { get; }

    /// <summary>
    /// Gets whether the special characters keyboard is enabled.
    /// </summary>
    public bool SpecialCharacters { get; }
}