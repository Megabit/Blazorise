#region Using directives
using System;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Provides data for a completed drag-and-drop operation in the <see cref="Scheduler{TItem}"/> component.
/// </summary>
public class SchedulerItemDroppedEventArgs<TItem>
{
    /// <summary>
    /// Represents the arguments for an event when a scheduler item is dropped, including the item and its time range.
    /// </summary>
    /// <param name="item">The object that represents the item being dropped in the scheduler.</param>
    /// <param name="start">The starting time of the scheduler item being dropped.</param>
    /// <param name="end">The ending time of the scheduler item being dropped.</param>
    public SchedulerItemDroppedEventArgs( TItem item, DateTime start, DateTime end )
    {
        Item = item;
        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the item that was dropped.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the new start time of the item.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the new end time of the item.
    /// </summary>
    public DateTime End { get; }
}