#region Using directives
using System;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Event arguments for the <see cref="Scheduler{TItem}.SlotClicked"/> event.
/// </summary>
public class SchedulerSlotClickedEventArgs : EventArgs
{
    /// <summary>
    /// Represents the event arguments for a scheduler slot click, containing the start and end times of the slot.
    /// </summary>
    /// <param name="start">Indicates the beginning time of the scheduler slot that was clicked.</param>
    /// <param name="end">Indicates the ending time of the scheduler slot that was clicked.</param>
    /// <param name="allDay">Indicates whether an event lasts all day.</param>
    public SchedulerSlotClickedEventArgs( DateTime start, DateTime end, bool allDay )
    {
        Start = start;
        End = end;
        AllDay = allDay;
    }

    /// <summary>
    /// Indicates the beginning time of the scheduler slot that was clicked.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Indicates the ending time of the scheduler slot that was clicked.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Indicates whether an event lasts all day.
    /// </summary>
    public bool AllDay { get; }
}