namespace Blazorise.Gantt;

/// <summary>
/// Defines tree pane columns that can be customized through templates.
/// </summary>
public enum GanttTreeColumn
{
    /// <summary>
    /// WBS (work breakdown structure) column.
    /// </summary>
    Wbs,

    /// <summary>
    /// Task title column.
    /// </summary>
    Title,

    /// <summary>
    /// Start date column.
    /// </summary>
    Start,

    /// <summary>
    /// End date column.
    /// </summary>
    End,

    /// <summary>
    /// Duration column.
    /// </summary>
    Duration,

    /// <summary>
    /// Command column.
    /// </summary>
    Command,
}