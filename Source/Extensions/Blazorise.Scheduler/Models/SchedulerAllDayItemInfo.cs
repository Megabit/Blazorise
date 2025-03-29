using System;

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
    /// <param name="overflowingFromStart">Indicates whether the item overflows from the start time.</param>
    /// <param name="overflowingOnEnd">Indicates whether the item overflows at the end time.</param>
    public SchedulerAllDayItemInfo( TItem item, DateTime start, DateTime end, bool overflowingFromStart, bool overflowingOnEnd )
    {
        Item = item;
        Start = start;
        End = end;
        OverflowingFromStart = overflowingFromStart;
        OverflowingOnEnd = overflowingOnEnd;
    }

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
    /// Indicates whether the data is overflowing from the start.
    /// </summary>
    public bool OverflowingFromStart { get; }

    /// <summary>
    /// Indicates whether there is an overflow at the end.
    /// </summary>
    public bool OverflowingOnEnd { get; }
}
