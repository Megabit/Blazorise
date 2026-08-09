namespace Blazorise.Gantt;

/// <summary>
/// Styling configuration used to customize a timeline milestone marker.
/// </summary>
public class GanttMilestoneStyling
{
    /// <summary>
    /// Gets or sets a custom CSS class applied to the milestone marker.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets custom CSS style appended to the milestone marker.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the milestone text and line color.
    /// </summary>
    public TextColor TextColor { get; set; } = Blazorise.TextColor.Primary;

    /// <summary>
    /// Gets or sets the milestone line style.
    /// </summary>
    public GanttMilestoneLineStyle LineStyle { get; set; } = GanttMilestoneLineStyle.Dashed;

    /// <summary>
    /// Gets or sets the milestone line width in pixels.
    /// </summary>
    public double LineWidth { get; set; } = 2d;

    /// <summary>
    /// Gets or sets where the milestone label is rendered.
    /// </summary>
    public GanttMilestoneLabelPosition LabelPosition { get; set; } = GanttMilestoneLabelPosition.Bottom;
}