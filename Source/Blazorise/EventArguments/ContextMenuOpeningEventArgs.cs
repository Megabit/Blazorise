namespace Blazorise;

/// <summary>
/// Supplies cancellable context menu opening information.
/// </summary>
public class ContextMenuOpeningEventArgs : ContextMenuEventArgs
{
    /// <summary>
    /// Initializes a new instance of <see cref="ContextMenuOpeningEventArgs"/>.
    /// </summary>
    /// <param name="clientX">The document client X coordinate.</param>
    /// <param name="clientY">The document client Y coordinate.</param>
    /// <param name="documentEventArgs">The originating document event.</param>
    public ContextMenuOpeningEventArgs( double clientX, double clientY, DocumentEventArgs documentEventArgs )
        : base( clientX, clientY, documentEventArgs )
    {
    }

    /// <summary>
    /// Gets or sets whether the context menu opening should be canceled.
    /// </summary>
    public bool Cancel { get; set; }
}