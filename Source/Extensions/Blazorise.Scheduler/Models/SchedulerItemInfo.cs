#region Using Directives
using System;
using Blazorise.Scheduler.Utilities;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Holds information about a scheduled item, including its time frame and overflow status.
/// </summary>
/// <typeparam name="TItem">Represents the specific type of the scheduled item, allowing for flexibility in the kind of data stored.</typeparam>
public class SchedulerItemInfo<TItem>
{
    /// <summary>
    /// Initializes a scheduler item with specified details including time and recurrence settings.
    /// </summary>
    /// <param name="item">Represents the specific item to be scheduled.</param>
    /// <param name="start">Indicates the starting time of the scheduled item.</param>
    /// <param name="end">Indicates the ending time of the scheduled item.</param>
    /// <param name="allDay">Specifies whether the scheduled item lasts all day.</param>
    /// <param name="isRecurring">Indicates if the scheduled item repeats at regular intervals.</param>
    public SchedulerItemInfo( TItem item, DateTime start, DateTime end, bool allDay, bool isRecurring )
    {
        Item = item;
        Start = start;
        End = end;
        Duration = end - start;
        AllDay = allDay;
        AllDayByDuration = Duration.Days >= 1;
        IsRecurring = isRecurring;
    }

    /// <summary>
    /// Initializes a new instance of the SchedulerItemInfo class with specified parameters for scheduling.
    /// </summary>
    /// <param name="item">Represents the item to be scheduled, containing relevant details for the event.</param>
    /// <param name="start">Indicates the starting date and time of the scheduled event.</param>
    /// <param name="end">Specifies the ending date and time of the scheduled event.</param>
    /// <param name="allDay">Indicates whether the event lasts all day without specific start and end times.</param>
    /// <param name="recurrenceRule">Defines the pattern for recurring events, allowing for complex scheduling.</param>
    /// <param name="isRecurring">Indicates if the event is part of a recurring series, affecting its scheduling behavior.</param>
    public SchedulerItemInfo( TItem item, DateTime start, DateTime end, bool allDay, string recurrenceRule, bool isRecurring )
        : this( item, start, end, allDay, isRecurring )
    {
        if ( !string.IsNullOrEmpty( recurrenceRule ) )
        {
            RecurrenceRule = RecurringRuleParser.Parse( recurrenceRule );
        }
    }

    /// <summary>
    /// Initializes a new instance of the SchedulerItemInfo class with specified parameters for scheduling.
    /// </summary>
    /// <param name="item">Represents the item to be scheduled.</param>
    /// <param name="start">Indicates the starting date and time of the scheduled item.</param>
    /// <param name="end">Indicates the ending date and time of the scheduled item.</param>
    /// <param name="allDay">Specifies whether the scheduled item lasts all day.</param>
    /// <param name="recurrenceRule">Defines the rules for recurring events in the schedule.</param>
    /// <param name="isRecurring">Indicates if the scheduled item is part of a recurring series.</param>
    public SchedulerItemInfo( TItem item, DateTime start, DateTime end, bool allDay, SchedulerRecurrenceRule recurrenceRule, bool isRecurring )
        : this( item, start, end, allDay, isRecurring )
    {
        RecurrenceRule = recurrenceRule;
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
