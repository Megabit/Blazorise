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
    {
        Key = key;
        DisplayText = displayText;
        Active = active;
        Shift = shift;
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
    /// Gets whether the key is active.
    /// </summary>
    public bool Active { get; }

    /// <summary>
    /// Gets whether shift is enabled.
    /// </summary>
    public bool Shift { get; }
}