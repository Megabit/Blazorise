#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a base transactional context for scheduler operations (dragging, selecting, etc.).
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public abstract class SchedulerTransaction<TItem>
{
    #region Members

    /// <summary>
    /// Holds a reference to a Scheduler instance that manages scheduling tasks for items of type TItem.
    /// </summary>
    protected readonly Scheduler<TItem> scheduler;

    private SchedulerTransactionState state = SchedulerTransactionState.Pending;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerTransaction{TItem}"/> class.
    /// </summary>
    /// <param name="scheduler">The parent scheduler instance.</param>
    /// <param name="item">The item being processed in the transaction.</param>
    /// <param name="section">The context from which the transaction originated.</param>
    protected SchedulerTransaction( Scheduler<TItem> scheduler, TItem item, SchedulerSection section )
    {
        this.scheduler = scheduler;

        OriginalItem = item;
        Item = item.DeepClone();
        Section = section;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Commits the transaction and applies any necessary logic. Must be implemented by derived classes.
    /// </summary>
    public async Task Commit()
    {
        if ( State == SchedulerTransactionState.Committed )
            return;

        SetState( SchedulerTransactionState.InProgress );

        try
        {
            await CommitImpl();

            if ( Committed is not null )
                await Committed.Invoke();

            SetState( SchedulerTransactionState.Committed );
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
    public virtual async Task Rollback()
    {
        if ( state == SchedulerTransactionState.RolledBack )
            return;

        try
        {
            await RollbackImpl();

            if ( Canceled is not null )
                await Canceled.Invoke();
        }
        finally
        {
            state = SchedulerTransactionState.RolledBack;
        }
    }

    /// <summary>
    /// An abstract method that must be implemented to handle the commit operation asynchronously.
    /// </summary>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    protected abstract Task CommitImpl();

    /// <summary>
    /// An abstract method that defines the implementation for rolling back an operation. It must be overridden in derived classes.
    /// </summary>
    /// <returns>Returns a Task representing the asynchronous rollback operation.</returns>
    protected virtual Task RollbackImpl()
    {
        Item = OriginalItem.DeepClone();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the internal transaction state.
    /// </summary>
    protected void SetState( SchedulerTransactionState newState ) => state = newState;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the original item that was passed into the transaction.
    /// </summary>
    protected TItem OriginalItem { get; init; }

    /// <summary>
    /// Gets the mutable working copy of the item used during the transaction.
    /// </summary>
    public TItem Item { get; protected set; }

    /// <summary>
    /// Gets the drag area context from which this transaction was initiated.
    /// </summary>
    public SchedulerSection Section { get; }

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
