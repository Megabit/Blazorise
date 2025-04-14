using System;

namespace Blazorise.Scheduler;

/// <summary>
/// Holds information about a scheduled item, including its time frame and overflow status.
/// </summary>
/// <typeparam name="TItem">Represents the specific type of the scheduled item, allowing for flexibility in the kind of data stored.</typeparam>
public class SchedulerItemViewInfo<TItem>
{
    /// <summary>
    /// Represents information about a scheduled item, including its time frame and overflow status.
    /// </summary>
    /// <param name="item">Defines the specific item being scheduled.</param>
    /// <param name="viewStart">Indicates the starting time of the scheduled item.</param>
    /// <param name="viewEnd">Indicates the ending time of the scheduled item.</param>
    /// <param name="overflowingFromStart">Indicates whether the item overflows at the start time.</param>
    /// <param name="overflowingOnEnd">Indicates whether the item overflows at the end time.</param>
    /// <param name="recurrenceRule">Defines the recurrence rule for the scheduled item.</param>
    /// <param name="isRecurring">Indicates whether the item is recurring.</param>
    public SchedulerItemViewInfo( TItem item, DateTime viewStart, DateTime viewEnd, bool overflowingFromStart, bool overflowingOnEnd, SchedulerRecurrenceRule recurrenceRule, bool isRecurring )
    {
        Item = item;
        ViewStart = viewStart;
        ViewEnd = viewEnd;
        OverflowingFromStart = overflowingFromStart;
        OverflowingOnEnd = overflowingOnEnd;
        RecurrenceRule = recurrenceRule;
        IsRecurring = isRecurring;
    }

    /// <summary>
    /// Represents an item of type TItem. It can be accessed and modified through its getter and setter.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Represents the start date of the item.
    /// </summary>
    public DateTime ViewStart { get; }

    /// <summary>
    /// Represents the end date of the item.
    /// </summary>
    public DateTime ViewEnd { get; }

    /// <summary>
    /// Indicates whether the data is overflowing from the start.
    /// </summary>
    public bool OverflowingFromStart { get; }

    /// <summary>
    /// Indicates whether there is an overflow at the end.
    /// </summary>
    public bool OverflowingOnEnd { get; }

    /// <summary>
    /// Represents the recurrence rule for scheduling events.
    /// </summary>
    public SchedulerRecurrenceRule RecurrenceRule { get; set; }

    /// <summary>
    /// Defines whether the item is recurring.
    /// </summary>
    public bool IsRecurring { get; }

    public override string ToString()
    {
        return $"Item: {Item}, ViewStart: {ViewStart}, ViewEnd: {ViewEnd}, OverflowingFromStart: {OverflowingFromStart}, OverflowingOnEnd: {OverflowingOnEnd}, RecurrenceRule: {RecurrenceRule}, IsRecurring: {IsRecurring}";
    }
}
