#region Using directives
using System;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Event arguments for the <see cref="Scheduler{TItem}.SlotsSelected"/> event.
/// </summary>
public class SchedulerSlotsSelectedEventArgs : EventArgs
{
    /// <summary>
    /// Represents the event arguments for selected scheduler slots, containing start and end times.
    /// </summary>
    /// <param name="start">Indicates the beginning time of the selected slot.</param>
    /// <param name="end">Indicates the ending time of the selected slot.</param>
    public SchedulerSlotsSelectedEventArgs( DateTime start, DateTime end )
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Indicates the beginning time of the selected slot.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Indicates the ending time of the selected slot.
    /// </summary>
    public DateTime End { get; }
}
