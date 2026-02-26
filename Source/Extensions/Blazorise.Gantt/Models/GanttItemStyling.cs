namespace Blazorise.Gantt;

/// <summary>
/// Styling configuration used to customize a timeline bar.
/// </summary>
public class GanttItemStyling
{
    /// <summary>
    /// Custom css class.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Custom css style.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Background color.
    /// </summary>
    public Background Background { get; set; } = Background.Info;

    /// <summary>
    /// Text color.
    /// </summary>
    public TextColor TextColor { get; set; } = TextColor.White;
}