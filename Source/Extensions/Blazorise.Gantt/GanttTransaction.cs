#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Represents a base transaction for Gantt chart item operations.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public abstract class GanttTransaction<TItem>
{
    #region Members

    private GanttTransactionState state = GanttTransactionState.Pending;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    protected GanttTransaction( Gantt<TItem> gantt, TItem item )
    {
        Gantt = gantt;
        OriginalItem = item;
        Item = item.DeepClone();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Commits transaction state.
    /// </summary>
    public async Task<bool> Commit()
    {
        if ( State == GanttTransactionState.Committed )
            return true;

        SetState( GanttTransactionState.InProgress );

        try
        {
            var result = await CommitImpl();

            if ( result )
            {
                if ( Committed is not null )
                    await Committed.Invoke();

                SetState( GanttTransactionState.Committed );
            }
            else
            {
                await Rollback();
            }

            return result;
        }
        catch
        {
            await Rollback();
            throw;
        }
    }

    /// <summary>
    /// Rolls back transaction state.
    /// </summary>
    public virtual async Task Rollback()
    {
        if ( State == GanttTransactionState.RolledBack )
            return;

        try
        {
            await RollbackImpl();

            if ( Canceled is not null )
                await Canceled.Invoke();
        }
        finally
        {
            SetState( GanttTransactionState.RolledBack );
        }
    }

    /// <summary>
    /// Commits transaction implementation.
    /// </summary>
    protected abstract Task<bool> CommitImpl();

    /// <summary>
    /// Rollback implementation.
    /// </summary>
    protected virtual Task RollbackImpl()
    {
        Item = OriginalItem.DeepClone();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets transaction state.
    /// </summary>
    protected void SetState( GanttTransactionState value )
    {
        state = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gantt chart instance.
    /// </summary>
    protected Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Original item.
    /// </summary>
    protected TItem OriginalItem { get; }

    /// <summary>
    /// Working transaction item.
    /// </summary>
    public TItem Item { get; protected set; }

    /// <summary>
    /// Current transaction state.
    /// </summary>
    public GanttTransactionState State => state;

    /// <summary>
    /// Callback triggered when transaction is committed.
    /// </summary>
    public Func<Task> Committed { get; set; }

    /// <summary>
    /// Callback triggered when transaction is canceled.
    /// </summary>
    public Func<Task> Canceled { get; set; }

    #endregion
}