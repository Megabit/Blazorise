namespace Blazorise;

/// <summary>
/// Supplies context menu event information.
/// </summary>
public class ContextMenuEventArgs
{
    /// <summary>
    /// Initializes a new instance of <see cref="ContextMenuEventArgs"/>.
    /// </summary>
    /// <param name="clientX">The viewport client X coordinate, or <see langword="null"/> when anchored to a target.</param>
    /// <param name="clientY">The viewport client Y coordinate, or <see langword="null"/> when anchored to a target.</param>
    /// <param name="documentEventArgs">The originating document event.</param>
    public ContextMenuEventArgs( double? clientX, double? clientY, DocumentEventArgs documentEventArgs )
    {
        ClientX = clientX;
        ClientY = clientY;
        DocumentEventArgs = documentEventArgs;
    }

    /// <summary>
    /// Gets the viewport client X coordinate, or <see langword="null"/> when anchored to a target.
    /// </summary>
    public double? ClientX { get; }

    /// <summary>
    /// Gets the viewport client Y coordinate, or <see langword="null"/> when anchored to a target.
    /// </summary>
    public double? ClientY { get; }

    /// <summary>
    /// Gets the originating document event.
    /// </summary>
    public DocumentEventArgs DocumentEventArgs { get; }
}