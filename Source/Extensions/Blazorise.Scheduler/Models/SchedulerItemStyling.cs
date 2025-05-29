namespace Blazorise.Scheduler;

/// <summary>
/// Represents styling options for a scheduler item, including custom class names, styles, background color, and text color.
/// </summary>
public class SchedulerItemStyling
{
    /// <summary>
    /// Gets or sets the custom class names for the scheduler item.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the custom styles for the scheduler item.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the custom background color for the scheduler item.
    /// </summary>
    public Background Background { get; set; }

    /// <summary>
    /// Gets or sets the custom text color for the scheduler item.
    /// </summary>
    public TextColor TextColor { get; set; }

    /// <summary>
    /// Gets or sets the custom text size for the scheduler item.
    /// </summary>
    public IFluentTextSize TextSize { get; set; }
}
