namespace Blazorise.Scheduler;

/// <summary>
/// Defines different time intervals for dividing scheduling slots within the <see cref="Scheduler{TItem}"/> component.
/// </summary>
public enum SchedulerSlotSize
{
    /// <summary>
    /// Represents 15-minute time slots.
    /// </summary>
    Is15Minutes,

    /// <summary>
    /// Represents 30-minute time slots.
    /// </summary>
    Is30Minutes,

    /// <summary>
    /// Represents 1-hour time slots.
    /// </summary>
    Is1Hour,
}
