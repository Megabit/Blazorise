namespace Blazorise;

/// <summary>
/// Defines the type of an on-screen keyboard key.
/// </summary>
public enum OnScreenKeyboardKeyType
{
    /// <summary>
    /// A key that inserts text.
    /// </summary>
    Text,

    /// <summary>
    /// A key that inserts a space.
    /// </summary>
    Space,

    /// <summary>
    /// A key that removes the last character.
    /// </summary>
    Backspace,

    /// <summary>
    /// A key that clears the current value.
    /// </summary>
    Clear,

    /// <summary>
    /// A key that confirms the current value.
    /// </summary>
    Enter,

    /// <summary>
    /// A key that toggles character casing.
    /// </summary>
    Shift,

    /// <summary>
    /// A key that toggles the special characters keyboard.
    /// </summary>
    SpecialCharacters,
}