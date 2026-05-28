namespace Blazorise;

/// <summary>
/// Defines how the on-screen keyboard is shown for an input component.
/// </summary>
public enum OnScreenKeyboardShowMode
{
    /// <summary>
    /// Uses the configured global show mode when set on an input component.
    /// </summary>
    Default,

    /// <summary>
    /// Shows the keyboard when the input receives focus.
    /// </summary>
    Focus,

    /// <summary>
    /// Shows the keyboard only when requested programmatically.
    /// </summary>
    Manual,
}