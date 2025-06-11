﻿#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Scheduler.Extensions;
using Blazorise.Scheduler.Utilities;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// A scheduler transaction representing a selection or inspection operation.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public class SchedulerSelectingTransaction<TItem> : SchedulerTransaction<TItem>
{
    #region Members

    private (DateTime, DateTime) selectionSlot1;
    private (DateTime, DateTime) selectionSlot2;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerSelectingTransaction{TItem}"/> class.
    /// </summary>
    public SchedulerSelectingTransaction( Scheduler<TItem> scheduler, TItem item, SchedulerSection section, DateTime slotStart, DateTime slotEnd )
        : base( scheduler, item, section )
    {
        selectionSlot1 = (slotStart, slotEnd);
        selectionSlot2 = (slotStart, slotEnd);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Checks if it's safe to update based on the provided date range and current selection slots.
    /// </summary>
    /// <param name="section">Indicates the section of the scheduler.</param>
    /// <param name="slotStart">Indicates the start date for the selection range.</param>
    /// <param name="slotEnd">Indicates the end date for the selection range.</param>
    /// <returns>Returns true if the current selection slots match the specified date range, otherwise false.</returns>
    public bool IsSafeToUpdate( SchedulerSection section, DateTime slotStart, DateTime slotEnd )
        => Section == section && selectionSlot1.Item1.Day == slotStart.Day && selectionSlot2.Item2.Day == slotEnd.Day;

    /// <summary>
    /// Updates the selection with a new time range defined by two date and time values.
    /// </summary>
    /// <param name="slotStart">Defines the beginning of the time range for the selection.</param>
    /// <param name="slotEnd">Defines the end of the time range for the selection.</param>
    public void UpdateSelection( DateTime slotStart, DateTime slotEnd )
    {
        var newSelection = (slotStart, slotEnd);

        if ( selectionSlot2 != newSelection )
        {
            selectionSlot2 = newSelection;
        }
    }

    /// <inheritdoc />
    protected override async Task CommitImpl()
    {
        if ( HasSelection )
        {
            await scheduler.NotifySlotsSelected( Start, End );
        }
    }

    /// <inheritdoc />
    protected override async Task RollbackImpl()
    {
        selectionSlot1 = default;
        selectionSlot2 = default;

        await base.RollbackImpl();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the minimum start time from two selection slots.
    /// </summary>
    public DateTime Start => DateUtils.Min( selectionSlot1.Item1, selectionSlot2.Item1 );

    /// <summary>
    /// Gets the maximum end time from two selection slots.
    /// </summary>
    public DateTime End => DateUtils.Max( selectionSlot1.Item2, selectionSlot2.Item2 );

    /// <summary>
    /// Indicates whether the selection spans more than one distinct slot.
    /// </summary>
    public bool HasSelection => selectionSlot1.Item1 != selectionSlot2.Item1 || selectionSlot1.Item2 != selectionSlot2.Item2;

    #endregion
}
