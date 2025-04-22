#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
#endregion

namespace Blazorise.Scheduler;

public class SchedulerTransaction<TItem>
{
    private Scheduler<TItem> scheduler;

    private SchedulerTransactionState state = SchedulerTransactionState.Pending;

    public SchedulerTransaction( Scheduler<TItem> scheduler, TItem item, SchedulerDragArea dragArea )
    {
        this.scheduler = scheduler;

        OriginalItem = item;
        Item = item.DeepClone();
        DragArea = dragArea;
    }

    public async Task Commit()
    {
        if ( state == SchedulerTransactionState.Committed )
            return;

        state = SchedulerTransactionState.InProgress;

        try
        {
            var isRecurrenceItem = scheduler.IsRecurrenceItem( Item );

            // When we are editing a recurrence item, we need to get the parent item so that we
            // can save it by using the same saving pipeline as for every editing, i.e. SaveImpl.
            var editItemClone = isRecurrenceItem
                ? scheduler.GetParentItem( Item ).DeepClone()
                : Item;

            if ( isRecurrenceItem && editItemClone is not null )
            {
                scheduler.AddRecurrenceException( editItemClone, Item );
            }

            if ( await scheduler.SaveImpl( editItemClone ) )
            {
                if ( Committed is not null )
                    await Committed.Invoke();

                state = SchedulerTransactionState.Committed;
            }
            else
            {
                await Rollback();
            }
        }
        catch
        {
            await Rollback();
            throw;
        }
    }

    public async Task Rollback()
    {
        if ( state == SchedulerTransactionState.RolledBack )
            return;

        try
        {
            Item = OriginalItem.DeepClone();

            if ( Canceled is not null )
                await Canceled.Invoke();
        }
        finally
        {
            state = SchedulerTransactionState.RolledBack;
        }
    }

    /// <summary>
    /// Stores the original item of type TItem. It is initialized at the time of object creation and cannot be modified afterward.
    /// </summary>
    private TItem OriginalItem { get; init; }

    public TItem Item { get; private set; }

    /// <summary>
    /// Defines the area or context where the drag originates.
    /// </summary>
    public SchedulerDragArea DragArea { get; }

    public SchedulerTransactionState State => state;

    public Func<Task> Committed { get; set; }

    public Func<Task> Canceled { get; set; }
}
