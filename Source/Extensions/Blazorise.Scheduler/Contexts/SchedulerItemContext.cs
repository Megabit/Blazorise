namespace Blazorise.Scheduler;

/// <summary>
/// Represents the context for a scheduled item, encapsulating the item itself.
/// </summary>
/// <typeparam name="TItem">The type of the item being scheduled, allowing for flexibility in the type of scheduled content.</typeparam>
public class SchedulerItemContext<TItem>
{
    /// <summary>
    /// A default constructor.
    /// </summary>
    /// <param name="item">An item that is currently rendered.</param>
    /// <param name="isRecurring">Indicates whether the item is recurring.</param>
    internal SchedulerItemContext( TItem item, bool isRecurring )
    {
        Item = item;
        IsRecurring = isRecurring;
    }

    /// <summary>
    /// Gets the reference to the item that is currently rendered.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Indicates whether an event or item occurs repeatedly.
    /// </summary>
    public bool IsRecurring { get; }
}
