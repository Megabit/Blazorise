namespace Blazorise.Gantt;

/// <summary>
/// Defines the internal edit state of the Gantt chart item.
/// </summary>
public enum GanttEditState
{
    /// <summary>
    /// No active edit operation.
    /// </summary>
    None,

    /// <summary>
    /// The item is being inserted.
    /// </summary>
    New,

    /// <summary>
    /// The item is being edited.
    /// </summary>
    Edit,
}