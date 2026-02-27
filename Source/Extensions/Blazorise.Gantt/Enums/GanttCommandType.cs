namespace Blazorise.Gantt;

/// <summary>
/// Defines supported edit commands for Gantt component.
/// </summary>
public enum GanttCommandType
{
    /// <summary>
    /// Creates a new top-level item.
    /// </summary>
    New,

    /// <summary>
    /// Creates a new child item.
    /// </summary>
    AddChild,

    /// <summary>
    /// Edits an existing item.
    /// </summary>
    Edit,

    /// <summary>
    /// Deletes an existing item.
    /// </summary>
    Delete,
}