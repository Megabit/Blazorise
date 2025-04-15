#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.DeepCloner;
#endregion

namespace Blazorise.Scheduler;

public class SchedulerTransaction<TItem>
{
    private Func<Task> Commited;

    private Func<Task> Canceled;

    public SchedulerTransaction( TItem item )
    {
        Item = item;
    }

    public Task Commit()
    {
        if ( Canceled is not null )
            return Canceled.Invoke();

        return Task.CompletedTask;
    }

    public Task Rollback()
    {
        if ( Canceled is not null )
            return Canceled.Invoke();

        return Task.CompletedTask;
    }

    public TItem Item { get; init; }
}
