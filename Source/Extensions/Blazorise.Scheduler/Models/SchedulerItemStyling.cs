namespace Blazorise.Scheduler;

/// <summary>
/// Represents styling options for a scheduler item, including custom class names, styles, background color, and text formatting.
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
    /// Gets or sets the text alignment for the scheduler item.
    /// </summary>
    public TextAlignment TextAlignment { get; set; }

    /// <summary>
    /// Gets or sets the text transformation for the scheduler item.
    /// </summary>
    public TextTransform TextTransform { get; set; }

    /// <summary>
    /// Gets or sets the text decoration for the scheduler item.
    /// </summary>
    public TextDecoration TextDecoration { get; set; }

    /// <summary>
    /// Gets or sets the text weight for the scheduler item.
    /// </summary>
    public TextWeight TextWeight { get; set; }

    /// <summary>
    /// Gets or sets the custom text size for the scheduler item.
    /// </summary>
    public IFluentTextSize TextSize { get; set; }

    /// <summary>
    /// Gets or sets how item text behaves when it exceeds the available space.
    /// When null, preserves the view's default text overflow behavior.
    /// </summary>
    public TextOverflow? TextOverflow { get; set; }
}