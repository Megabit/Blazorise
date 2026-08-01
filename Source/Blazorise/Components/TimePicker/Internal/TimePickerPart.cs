namespace Blazorise;

/// <summary>
/// Identifies the active field in the time selection menu.
/// </summary>
internal enum TimePickerPart
{
    /// <summary>
    /// The hour field.
    /// </summary>
    Hour,

    /// <summary>
    /// The minute field.
    /// </summary>
    Minute,

    /// <summary>
    /// The second field.
    /// </summary>
    Second,

    /// <summary>
    /// The ante or post meridiem field.
    /// </summary>
    Meridiem,
}