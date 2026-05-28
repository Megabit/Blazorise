namespace Blazorise;

/// <summary>
/// Defines how keys are arranged inside each on-screen keyboard row.
/// </summary>
public enum OnScreenKeyboardKeyLayout
{
    /// <summary>
    /// Keys stretch to fill the available row width.
    /// </summary>
    Stretch,

    /// <summary>
    /// Keys keep a fixed visual width and are centered in each row.
    /// </summary>
    Centered,
}