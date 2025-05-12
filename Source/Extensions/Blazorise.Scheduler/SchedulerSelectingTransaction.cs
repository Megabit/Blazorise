#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Scheduler.Extensions;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// A scheduler transaction representing a selection or inspection operation.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public class SchedulerSelectingTransaction<TItem> : SchedulerTransaction<TItem>
{
    private (DateTime, DateTime)? selectionSlot1;
    private (DateTime, DateTime)? selectionSlot2;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerSelectingTransaction{TItem}"/> class.
    /// </summary>
    public SchedulerSelectingTransaction( Scheduler<TItem> scheduler, SchedulerSection section, DateTime slotStart, DateTime slotEnd )
        : base( scheduler, section )
    {
        selectionSlot1 = (slotStart, slotEnd);
        selectionSlot2 = (slotStart, slotEnd);
    }

    /// <summary>
    /// Checks if it's safe to update based on the provided date range and current selection slots.
    /// </summary>
    /// <param name="section">Indicates the section of the scheduler.</param>
    /// <param name="slotStart">Indicates the start date for the selection range.</param>
    /// <param name="slotEnd">Indicates the end date for the selection range.</param>
    /// <returns>Returns true if the current selection slots match the specified date range, otherwise false.</returns>
    public bool IsSafeToUpdate( SchedulerSection section, DateTime slotStart, DateTime slotEnd )
        => Section == section && selectionSlot1 != null && selectionSlot1.Value.Item1.Day == slotStart.Day && selectionSlot2.Value.Item2.Day == slotEnd.Day;

    /// <summary>
    /// Updates the selection with a new time range defined by two date and time values.
    /// </summary>
    /// <param name="slotStart">Defines the beginning of the time range for the selection.</param>
    /// <param name="slotEnd">Defines the end of the time range for the selection.</param>
    public void UpdateSelection( DateTime slotStart, DateTime slotEnd )
    {
        selectionSlot2 = (slotStart, slotEnd);
    }

    /// <inheritdoc />
    protected override Task CommitImpl()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task RollbackImpl()
    {
        selectionSlot1 = null;
        selectionSlot2 = null;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the minimum start time from two selection slots.
    /// </summary>
    public DateTime Start => selectionSlot1.Value.Item1.Min( selectionSlot2.Value.Item1 );

    /// <summary>
    /// Gets the maximum end time from two selection slots.
    /// </summary>
    public DateTime End => selectionSlot1.Value.Item2.Max( selectionSlot2.Value.Item2 );
}
