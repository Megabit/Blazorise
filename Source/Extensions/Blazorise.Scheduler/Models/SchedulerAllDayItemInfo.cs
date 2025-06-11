#region Using Directives
using System;
using Blazorise.Scheduler.Utilities; 
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents information about an all-day item in a scheduler, including its time range and overflow status.
/// </summary>
/// <typeparam name="TItem">Represents the type of the item being scheduled, allowing for flexibility in the kind of data stored.</typeparam>
public class SchedulerAllDayItemInfo<TItem>
{
    /// <summary>
    /// Represents information about an all-day item in a scheduler, including its time range and overflow status.
    /// </summary>
    /// <param name="item">The item to be scheduled, which contains relevant details for the all-day event.</param>
    /// <param name="start">The starting date and time of the all-day item.</param>
    /// <param name="end">The ending date and time of the all-day item.</param>
    /// <param name="viewStart">The starting date and time of the all-day item in the view.</param>
    /// <param name="viewEnd">The ending date and time of the all-day item in the view.</param>
    /// <param name="overflowingFromStart">Indicates whether the item overflows from the start time.</param>
    /// <param name="overflowingOnEnd">Indicates whether the item overflows at the end time.</param>
    public SchedulerAllDayItemInfo( TItem item, DateTime start, DateTime end, DateTime viewStart, DateTime viewEnd, bool overflowingFromStart, bool overflowingOnEnd )
    {
        Key = Guid.NewGuid().ToString( "N" );
        Item = item;
        Start = start;
        End = end;
        Duration = end - start;
        ViewStart = viewStart;
        ViewEnd = viewEnd;
        OverflowingFromStart = overflowingFromStart;
        OverflowingOnEnd = overflowingOnEnd;
    }

    /// <summary>
    /// Represents information about an all-day item in a scheduler, including its timing and recurrence details.
    /// </summary>
    /// <param name="item">The item to be scheduled, which contains the relevant data for the all-day event.</param>
    /// <param name="start">Indicates the start time of the all-day item.</param>
    /// <param name="end">Indicates the end time of the all-day item.</param>
    /// <param name="viewStart">Specifies the start time of the view in which the item is displayed.</param>
    /// <param name="viewEnd">Specifies the end time of the view in which the item is displayed.</param>
    /// <param name="overflowingFromStart">Indicates whether the item overflows from the start of the view.</param>
    /// <param name="overflowingOnEnd">Indicates whether the item overflows at the end of the view.</param>
    /// <param name="isRecurring">Indicates whether the item occurs repeatedly over time.</param>
    public SchedulerAllDayItemInfo( TItem item, DateTime start, DateTime end, DateTime viewStart, DateTime viewEnd, bool overflowingFromStart, bool overflowingOnEnd, bool isRecurring )
        : this( item, start, end, viewStart, viewEnd, overflowingFromStart, overflowingOnEnd )
    {
        IsRecurring = isRecurring;
    }

    /// <summary>
    /// Initializes an instance of the all-day item in the scheduler with specified parameters and handles recurrence
    /// rules.
    /// </summary>
    /// <param name="item">Represents the item to be scheduled, containing relevant details for the all-day event.</param>
    /// <param name="start">Indicates the start date and time of the all-day event.</param>
    /// <param name="end">Specifies the end date and time of the all-day event.</param>
    /// <param name="viewStart">Defines the start date and time of the view in which the event is displayed.</param>
    /// <param name="viewEnd">Sets the end date and time of the view in which the event is displayed.</param>
    /// <param name="overflowingFromStart">Indicates whether the event overflows from the start of the view.</param>
    /// <param name="overflowingOnEnd">Indicates whether the event overflows at the end of the view.</param>
    /// <param name="recurrenceRule">Contains a string that defines the recurrence pattern for the event if it repeats.</param>
    /// <param name="isRecurring">Indicates whether the event is a recurring event.</param>
    public SchedulerAllDayItemInfo( TItem item, DateTime start, DateTime end, DateTime viewStart, DateTime viewEnd, bool overflowingFromStart, bool overflowingOnEnd, string recurrenceRule, bool isRecurring )
        : this( item, start, end, viewStart, viewEnd, overflowingFromStart, overflowingOnEnd )
    {
        if ( !string.IsNullOrEmpty( recurrenceRule ) )
        {
            RecurrenceRule = RecurringRuleParser.Parse( recurrenceRule );
        }

        IsRecurring = isRecurring;
    }

    /// <summary>
    /// Constructs an object representing an all-day item in a scheduler with recurrence information.
    /// </summary>
    /// <param name="item">Represents the specific item to be scheduled.</param>
    /// <param name="start">Indicates the start date and time of the all-day item.</param>
    /// <param name="end">Indicates the end date and time of the all-day item.</param>
    /// <param name="viewStart">Specifies the start date and time of the current view in the scheduler.</param>
    /// <param name="viewEnd">Specifies the end date and time of the current view in the scheduler.</param>
    /// <param name="overflowingFromStart">Indicates whether the item overflows from the start of the view.</param>
    /// <param name="overflowingOnEnd">Indicates whether the item overflows at the end of the view.</param>
    /// <param name="recurrenceRule">Defines the rules for recurring occurrences of the item.</param>
    /// <param name="isRecurring">Indicates if the item is part of a recurring series.</param>
    public SchedulerAllDayItemInfo( TItem item, DateTime start, DateTime end, DateTime viewStart, DateTime viewEnd, bool overflowingFromStart, bool overflowingOnEnd, SchedulerRecurrenceRule recurrenceRule, bool isRecurring )
        : this( item, start, end, viewStart, viewEnd, overflowingFromStart, overflowingOnEnd )
    {
        RecurrenceRule = recurrenceRule;
        IsRecurring = isRecurring;
    }

    /// <summary>
    /// Gets the unique identifier associated with this instance.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Represents an item of type TItem. It can be accessed and modified through its getter and setter.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Represents the start date of the all-day item.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Represents the end date of the all-day item.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Gets the duration of the scheduled item.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Represents the start date of the all-day item in the view. This is needed to display items that overflow the view start date.
    /// </summary>
    public DateTime ViewStart { get; }

    /// <summary>
    /// Represents the end date of the all-day item in the view. This is needed to display items that overflow the view end date.
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
    public bool IsRecurring { get; set; }
}
