using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazorise.Tests.TestServices;

/// <summary>
/// A custom task scheduler that runs all scheduled tasks in the same thread
/// </summary>
public class CurrentThreadTaskScheduler : TaskScheduler
{
    protected override void QueueTask(Task task) => 
        TryExecuteTask(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => 
        TryExecuteTask(task);

    protected override IEnumerable<Task> GetScheduledTasks() => 
        Enumerable.Empty<Task>();

    public override int MaximumConcurrencyLevel => 1;
}