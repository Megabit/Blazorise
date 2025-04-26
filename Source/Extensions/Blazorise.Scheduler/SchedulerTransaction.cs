#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a transactional context for editing or dragging a scheduler item.
/// Provides support for commit, rollback, and recurrence exception handling.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public class SchedulerTransaction<TItem>
{
    #region Members

    private Scheduler<TItem> scheduler;

    private SchedulerTransactionState state = SchedulerTransactionState.Pending;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerTransaction{TItem}"/> class.
    /// </summary>
    /// <param name="scheduler">The parent scheduler instance.</param>
    /// <param name="item">The item being edited or moved.</param>
    /// <param name="dragArea">The drag area context from which the transaction originated.</param>
    public SchedulerTransaction( Scheduler<TItem> scheduler, TItem item, SchedulerDragArea dragArea )
    {
        this.scheduler = scheduler;

        OriginalItem = item;
        Item = item.DeepClone();
        DragArea = dragArea;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Attempts to commit the transaction. If the item is a recurrence exception,
    /// it updates the parent and adds an exception entry before saving.
    /// </summary>
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

            if ( scheduler.DropAllowed is not null
                && await scheduler.DropAllowed.Invoke( new SchedulerDragEventArgs<TItem>( editItemClone ) ) == false )
            {
                await Rollback();

                return;
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

    /// <summary>
    /// Cancels the transaction and reverts the working copy to the original item state.
    /// </summary>
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

    #endregion

    #region Properties

    /// <summary>
    /// Gets the original item that was passed into the transaction.
    /// </summary>
    private TItem OriginalItem { get; init; }

    /// <summary>
    /// Gets the mutable working copy of the item used during the transaction.
    /// </summary>
    public TItem Item { get; private set; }

    /// <summary>
    /// Gets the drag area context from which this transaction was initiated.
    /// </summary>
    public SchedulerDragArea DragArea { get; }

    /// <summary>
    /// Gets the current state of the transaction.
    /// </summary>
    public SchedulerTransactionState State => state;

    /// <summary>
    /// Gets or sets the callback to be invoked when the transaction is successfully committed.
    /// </summary>
    public Func<Task> Committed { get; set; }

    /// <summary>
    /// Gets or sets the callback to be invoked if the transaction is canceled or rolled back.
    /// </summary>
    public Func<Task> Canceled { get; set; }

    #endregion
}
