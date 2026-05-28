namespace Blazorise;

/// <summary>
/// Defines how the on-screen keyboard enter key should behave.
/// </summary>
public enum OnScreenKeyboardEnterKeyBehavior
{
    /// <summary>
    /// Uses the default behavior for the focused input component.
    /// </summary>
    Default,

    /// <summary>
    /// Inserts a new line into the focused input component.
    /// </summary>
    NewLine,

    /// <summary>
    /// Submits the closest Blazorise validations or form.
    /// </summary>
    Submit,

    /// <summary>
    /// Hides the on-screen keyboard.
    /// </summary>
    Hide,

    /// <summary>
    /// Dispatches an Enter keydown event from the focused input component.
    /// </summary>
    KeyDown,
}