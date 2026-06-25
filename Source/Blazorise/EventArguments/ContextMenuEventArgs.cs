namespace Blazorise;

/// <summary>
/// Supplies context menu event information.
/// </summary>
public class ContextMenuEventArgs
{
    /// <summary>
    /// Initializes a new instance of <see cref="ContextMenuEventArgs"/>.
    /// </summary>
    /// <param name="clientX">The document client X coordinate.</param>
    /// <param name="clientY">The document client Y coordinate.</param>
    /// <param name="documentEventArgs">The originating document event.</param>
    public ContextMenuEventArgs( double clientX, double clientY, DocumentEventArgs documentEventArgs )
    {
        ClientX = clientX;
        ClientY = clientY;
        DocumentEventArgs = documentEventArgs;
    }

    /// <summary>
    /// Gets the document client X coordinate.
    /// </summary>
    public double ClientX { get; }

    /// <summary>
    /// Gets the document client Y coordinate.
    /// </summary>
    public double ClientY { get; }

    /// <summary>
    /// Gets the originating document event.
    /// </summary>
    public DocumentEventArgs DocumentEventArgs { get; }
}