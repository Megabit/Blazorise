namespace Blazorise;

/// <summary>
/// Represents a single key rendered by the on-screen keyboard.
/// </summary>
public class OnScreenKeyboardKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnScreenKeyboardKey"/> class.
    /// </summary>
    public OnScreenKeyboardKey()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnScreenKeyboardKey"/> class.
    /// </summary>
    /// <param name="text">Text inserted by the key.</param>
    public OnScreenKeyboardKey( string text )
    {
        Text = text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnScreenKeyboardKey"/> class.
    /// </summary>
    /// <param name="keyType">Key type.</param>
    /// <param name="displayText">Text displayed on the key.</param>
    public OnScreenKeyboardKey( OnScreenKeyboardKeyType keyType, string displayText )
    {
        KeyType = keyType;
        DisplayText = displayText;
    }

    /// <summary>
    /// Gets or sets the type of the key.
    /// </summary>
    public OnScreenKeyboardKeyType KeyType { get; set; } = OnScreenKeyboardKeyType.Text;

    /// <summary>
    /// Gets or sets the inserted text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the text displayed by the key.
    /// </summary>
    public string DisplayText { get; set; }

    /// <summary>
    /// Gets or sets the accessible label for the key.
    /// </summary>
    public string AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the key width multiplier.
    /// </summary>
    public int Width { get; set; } = 1;
}