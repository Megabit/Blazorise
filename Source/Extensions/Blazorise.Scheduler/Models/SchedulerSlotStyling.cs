namespace Blazorise.Scheduler;

/// <summary>
/// Represents styling options for a scheduler slot, including custom class names, styles, background, and border.
/// </summary>
public class SchedulerSlotStyling
{
    /// <summary>
    /// Gets or sets the custom class names for the scheduler slot.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the custom styles for the scheduler slot.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the custom background for the scheduler slot.
    /// </summary>
    public Background Background { get; set; }

    /// <summary>
    /// Gets or sets the custom border for the scheduler slot.
    /// </summary>
    public IFluentBorder Border { get; set; }
}