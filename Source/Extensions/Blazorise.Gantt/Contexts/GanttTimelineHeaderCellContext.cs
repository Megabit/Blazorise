using System;

namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering timeline header cells.
/// </summary>
public class GanttTimelineHeaderCellContext
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    /// <param name="key">Unique slot key.</param>
    /// <param name="label">Display label.</param>
    /// <param name="start">Slot start time.</param>
    /// <param name="end">Slot end time.</param>
    /// <param name="index">Slot index in the current view range.</param>
    public GanttTimelineHeaderCellContext( string key, string label, DateTime start, DateTime end, int index )
    {
        Key = key;
        Label = label;
        Start = start;
        End = end;
        Index = index;
    }

    /// <summary>
    /// Gets unique slot key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets display label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets slot start time.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets slot end time.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Gets slot index in the current view range.
    /// </summary>
    public int Index { get; }
}