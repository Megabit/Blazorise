namespace Blazorise.Gantt;

/// <summary>
/// Represents sortable columns in the Gantt tree pane.
/// </summary>
public enum GanttSortColumn
{
    /// <summary>
    /// Sort by task title.
    /// </summary>
    Title,

    /// <summary>
    /// Sort by task start date.
    /// </summary>
    Start,

    /// <summary>
    /// Sort by task end date.
    /// </summary>
    End,

    /// <summary>
    /// Sort by task duration.
    /// </summary>
    Duration,
}