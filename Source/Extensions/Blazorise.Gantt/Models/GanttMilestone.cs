using System;

namespace Blazorise.Gantt;

/// <summary>
/// Defines a milestone marker rendered on the Gantt timeline.
/// </summary>
public class GanttMilestone
{
    /// <summary>
    /// Gets or sets the milestone date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the milestone title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets whether the milestone is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

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

    /// <summary>
    /// Gets or sets a custom CSS class applied to the milestone marker.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets custom CSS style appended to the milestone marker.
    /// </summary>
    public string Style { get; set; }
}