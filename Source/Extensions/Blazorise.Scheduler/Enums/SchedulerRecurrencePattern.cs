namespace Blazorise.Scheduler;

/// <summary>
/// Defines recurrence patterns for scheduling events, including daily, weekly, monthly, and yearly options.
/// </summary>
public enum SchedulerRecurrencePattern
{
    /// <summary>
    /// 
    /// </summary>
    Never,

    /// <summary>
    /// Represents a daily frequency or occurrence. Typically used in contexts where events or actions happen every day.
    /// </summary>
    Daily,

    /// <summary>
    /// Represents a weekly time period or schedule. Typically used for organizing events or tasks on a weekly basis.
    /// </summary>
    Weekly,

    /// <summary>
    /// Represents a monthly frequency or interval. Typically used in contexts involving time-based calculations or scheduling.
    /// </summary>
    Monthly,

    /// <summary>
    /// Represents a yearly time period. Typically used for calculations or representations involving annual data.
    /// </summary>
    Yearly
}
