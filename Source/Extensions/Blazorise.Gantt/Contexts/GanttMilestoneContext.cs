using System;

namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering timeline milestone content.
/// </summary>
public class GanttMilestoneContext
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    /// <param name="milestone">The milestone marker.</param>
    /// <param name="selectedView">The currently selected view.</param>
    /// <param name="viewStart">The visible timeline range start.</param>
    /// <param name="viewEnd">The visible timeline range end.</param>
    public GanttMilestoneContext( GanttMilestone milestone, GanttView selectedView, DateTime viewStart, DateTime viewEnd )
    {
        Milestone = milestone;
        SelectedView = selectedView;
        ViewStart = viewStart;
        ViewEnd = viewEnd;
    }

    /// <summary>
    /// Gets the milestone marker.
    /// </summary>
    public GanttMilestone Milestone { get; }

    /// <summary>
    /// Gets the milestone date.
    /// </summary>
    public DateTime Date => Milestone.Date;

    /// <summary>
    /// Gets the milestone title.
    /// </summary>
    public string Title => Milestone.Title;

    /// <summary>
    /// Gets the currently selected view.
    /// </summary>
    public GanttView SelectedView { get; }

    /// <summary>
    /// Gets the visible timeline range start.
    /// </summary>
    public DateTime ViewStart { get; }

    /// <summary>
    /// Gets the visible timeline range end.
    /// </summary>
    public DateTime ViewEnd { get; }
}