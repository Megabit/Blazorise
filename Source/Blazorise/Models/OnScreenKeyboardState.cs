namespace Blazorise;

/// <summary>
/// Represents the current state of the on-screen keyboard.
/// </summary>
public class OnScreenKeyboardState
{
    /// <summary>
    /// Gets or sets a value indicating whether the keyboard is visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the current keyboard context.
    /// </summary>
    public OnScreenKeyboardContext Context { get; set; }
}