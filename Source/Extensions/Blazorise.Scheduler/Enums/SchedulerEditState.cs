namespace Blazorise.Scheduler;

/// <summary>
/// Represents the state of a scheduler edit operation.
/// </summary>
public enum SchedulerEditState
{
    /// <summary>
    /// No member is defined. There are no properties or methods to describe.
    /// </summary>
    None,

    /// <summary>
    /// Creates a new instance of the class. Initializes the object with default values.
    /// </summary>
    New,

    /// <summary>
    /// Allows for modifying existing content or data. Typically used to update information in a user interface.
    /// </summary>
    Edit,
}
