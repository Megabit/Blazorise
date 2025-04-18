using System;
using Blazorise.Scheduler.Utilities;

namespace Blazorise.Scheduler;

/// <summary>
/// Holds information about a scheduled item, including its time frame and overflow status.
/// </summary>
/// <typeparam name="TItem">Represents the specific type of the scheduled item, allowing for flexibility in the kind of data stored.</typeparam>
public class SchedulerItemInfo<TItem>
{
    /// <summary>
    /// Represents information about a scheduled item, including its time frame and overflow status.
    /// </summary>
    /// <param name="item">Defines the specific item being scheduled.</param>
    /// <param name="start">Indicates the starting time of the scheduled item.</param>
    /// <param name="end">Indicates the ending time of the scheduled item.</param>
    /// <param name="allDay"></param>
    /// <param name="recurrenceRule">Defines the recurrence rule for the scheduled item.</param>
    /// <param name="isRecurring">Indicates whether the item is recurring.</param>
    public SchedulerItemInfo( TItem item, DateTime start, DateTime end, bool allDay, string recurrenceRule, bool isRecurring )
    {
        Item = item;
        Start = start;
        End = end;
        Duration = end - start;
        AllDay = allDay;
        AllDayByDuration = Duration.Days >= 1;

        if ( !string.IsNullOrEmpty( recurrenceRule ) )
        {
            RecurrenceRule = RecurringRuleParser.Parse( recurrenceRule );
        }

        IsRecurring = isRecurring;
    }

    public SchedulerItemInfo( TItem item, DateTime start, DateTime end, bool allDay, bool isRecurring )
    {
        Item = item;
        Start = start;
        End = end;
        Duration = end - start;
        AllDay = allDay;
        AllDayByDuration = Duration.Days >= 1;
        RecurrenceRule = null;
        IsRecurring = isRecurring;
    }

    public SchedulerItemInfo( TItem item, DateTime start, DateTime end, bool allDay, SchedulerRecurrenceRule recurrenceRule, bool isRecurring )
    {
        Item = item;
        Start = start;
        End = end;
        Duration = end - start;
        AllDay = allDay;
        AllDayByDuration = Duration.Days >= 1;
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
    public DateTime Start { get; }

    /// <summary>
    /// Represents the end date of the item.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Gets the duration of the scheduled item.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Indicates whether an event lasts all day.
    /// </summary>
    public bool AllDay { get; }

    /// <summary>
    /// Indicates whether an event lasts all day based on its duration.
    /// </summary>
    public bool AllDayByDuration { get; }

    /// <summary>
    /// Represents the recurrence rule for scheduling events.
    /// </summary>
    public SchedulerRecurrenceRule RecurrenceRule { get; set; }

    /// <summary>
    /// Defines whether the item is recurring.
    /// </summary>
    public bool IsRecurring { get; set; }
}
