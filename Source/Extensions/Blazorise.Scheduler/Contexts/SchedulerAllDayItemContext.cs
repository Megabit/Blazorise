namespace Blazorise.Scheduler;

/// <summary>
/// Represents the context for an all-day item in a scheduler, providing access to the currently rendered item.
/// </summary>
/// <typeparam name="TItem">Represents the type of the all-day item being rendered in the scheduler.</typeparam>
public class SchedulerAllDayItemContext<TItem>
{
    /// <summary>
    /// A default constructor.
    /// </summary>
    /// <param name="item">An all-day item that is currently rendered.</param>
    /// <param name="isRecurring">Indicates whether the item is recurring.</param>
    internal SchedulerAllDayItemContext( TItem item, bool isRecurring )
    {
        Item = item;
        IsRecurring = isRecurring;
    }

    /// <summary>
    /// Gets the reference to the all-day item that is currently rendered.
    /// </summary>
    public TItem Item { get; private set; }

    /// <summary>
    /// Indicates whether an event or item occurs repeatedly.
    /// </summary>
    public bool IsRecurring { get; }
}
