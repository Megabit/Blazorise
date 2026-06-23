namespace Blazorise;

/// <summary>
/// Defines JavaScript serializable options for a document observer subscription.
/// </summary>
public class DocumentObserverJsSubscription
{
    /// <summary>
    /// Gets or sets the subscription identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the owner identifier used to coordinate event ownership.
    /// </summary>
    public string OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the observed browser event names.
    /// </summary>
    public string[] EventNames { get; set; }

    /// <summary>
    /// Gets or sets a selector that must match the event target or one of its ancestors.
    /// </summary>
    public string Selector { get; set; }

    /// <summary>
    /// Gets or sets a selector that excludes matching event targets or ancestors.
    /// </summary>
    public string ExcludeSelector { get; set; }

    /// <summary>
    /// Gets or sets the subscription dispatch priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets whether the listener uses the document capture phase.
    /// </summary>
    public bool Capture { get; set; }

    /// <summary>
    /// Gets or sets whether the JavaScript side should prevent the default browser action before invoking the handler.
    /// </summary>
    public bool PreventDefault { get; set; }

    /// <summary>
    /// Gets or sets whether the JavaScript side should stop propagation before invoking the handler.
    /// </summary>
    public bool StopPropagation { get; set; }

    /// <summary>
    /// Gets or sets whether high frequency events should be throttled with requestAnimationFrame.
    /// </summary>
    public bool Throttle { get; set; }

    /// <summary>
    /// Gets or sets whether this subscription ignores active pointer ownership.
    /// </summary>
    public bool IgnorePointerCapture { get; set; }

    /// <summary>
    /// Gets or sets whether the event should be forwarded to .NET.
    /// </summary>
    public bool DotNet { get; set; }
}
