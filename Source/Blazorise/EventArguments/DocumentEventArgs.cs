namespace Blazorise;

/// <summary>
/// Supplies normalized information for a browser document event.
/// </summary>
public class DocumentEventArgs
{
    /// <summary>
    /// Gets or sets the document event type.
    /// </summary>
    public DocumentEventType Type { get; set; }

    /// <summary>
    /// Gets or sets the browser event name.
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// Gets or sets the pointer identifier.
    /// </summary>
    public long PointerId { get; set; }

    /// <summary>
    /// Gets or sets the document client X coordinate.
    /// </summary>
    public double ClientX { get; set; }

    /// <summary>
    /// Gets or sets the document client Y coordinate.
    /// </summary>
    public double ClientY { get; set; }

    /// <summary>
    /// Gets or sets the keyboard key value.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets whether the control key was pressed.
    /// </summary>
    public bool CtrlKey { get; set; }

    /// <summary>
    /// Gets or sets whether the shift key was pressed.
    /// </summary>
    public bool ShiftKey { get; set; }

    /// <summary>
    /// Gets or sets whether the alt key was pressed.
    /// </summary>
    public bool AltKey { get; set; }

    /// <summary>
    /// Gets or sets whether the meta key was pressed.
    /// </summary>
    public bool MetaKey { get; set; }

    /// <summary>
    /// Gets or sets the matched selector for the subscription.
    /// </summary>
    public string MatchedSelector { get; set; }

    /// <summary>
    /// Gets or sets the target element tag name.
    /// </summary>
    public string TargetTagName { get; set; }

    /// <summary>
    /// Gets or sets the target element id.
    /// </summary>
    public string TargetId { get; set; }

    /// <summary>
    /// Gets or sets the target element class name.
    /// </summary>
    public string TargetClassName { get; set; }
}
