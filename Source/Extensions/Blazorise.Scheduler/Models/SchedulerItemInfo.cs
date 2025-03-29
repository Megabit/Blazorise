using System;

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
    /// <param name="overflowingFromStart">Indicates whether the item overflows at the start time.</param>
    /// <param name="overflowingOnEnd">Indicates whether the item overflows at the end time.</param>
    public SchedulerItemInfo( TItem item, DateTime start, DateTime end, bool overflowingFromStart, bool overflowingOnEnd )
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
    /// Represents the start date of the item.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Represents the end date of the item.
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
