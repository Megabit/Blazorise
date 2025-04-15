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

    public SchedulerTransaction( Scheduler<TItem> scheduler, TItem item )
    {
        this.scheduler = scheduler;

        OriginalItem = item;
        Item = item.DeepClone();
    }

    public async Task Commit()
    {
        if ( state == SchedulerTransactionState.Committed )
            return;

        state = SchedulerTransactionState.InProgress;

        try
        {
            scheduler.CopyItemValues( Item, OriginalItem );

            if ( Committed != null )
                await Committed.Invoke();

            state = SchedulerTransactionState.Committed;
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
            if ( Canceled != null )
                await Canceled.Invoke();
        }
        finally
        {
            state = SchedulerTransactionState.RolledBack;
        }
    }

    private TItem OriginalItem { get; init; }

    public TItem Item { get; set; }

    public SchedulerTransactionState State => state;

    public Func<Task> Committed { get; set; }

    public Func<Task> Canceled { get; set; }
}
