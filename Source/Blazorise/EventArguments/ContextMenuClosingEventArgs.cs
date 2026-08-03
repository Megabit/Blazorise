namespace Blazorise;

/// <summary>
/// Supplies cancellable context menu closing information.
/// </summary>
public class ContextMenuClosingEventArgs : ContextMenuEventArgs
{
    /// <summary>
    /// Initializes a new instance of <see cref="ContextMenuClosingEventArgs"/>.
    /// </summary>
    /// <param name="clientX">The viewport client X coordinate, or <see langword="null"/> when anchored to a target.</param>
    /// <param name="clientY">The viewport client Y coordinate, or <see langword="null"/> when anchored to a target.</param>
    /// <param name="documentEventArgs">The originating document event.</param>
    /// <param name="closeReason">The reason the context menu is being closed.</param>
    public ContextMenuClosingEventArgs( double? clientX, double? clientY, DocumentEventArgs documentEventArgs, CloseReason closeReason )
        : base( clientX, clientY, documentEventArgs )
    {
        CloseReason = closeReason;
    }

    /// <summary>
    /// Gets the reason the context menu is being closed.
    /// </summary>
    public CloseReason CloseReason { get; }

    /// <summary>
    /// Gets or sets whether the context menu closing should be canceled.
    /// </summary>
    public bool Cancel { get; set; }
}