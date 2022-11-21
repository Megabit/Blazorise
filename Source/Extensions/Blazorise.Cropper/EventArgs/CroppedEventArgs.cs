namespace Blazorise.Cropper;

/// <summary>
/// Provides all the information for the cropper crop event.
/// </summary>
public class CroppedEventArgs
{
    /// <summary>
    /// A default <see cref="CroppedEventArgs"/> constructor.
    /// </summary>
    /// <param name="startX">The starting pageX value.</param>
    /// <param name="startY">The starting pageY value.</param>
    /// <param name="endX">The ending pageX value.</param>
    /// <param name="endY">The ending pageY value.</param>
    public CroppedEventArgs( int startX, int startY, int endX, int endY )
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
    }

    /// <summary>
    /// Gets the starting pageX value.
    /// </summary>
    public int StartX { get; }

    /// <summary>
    /// Gets the starting pageY value.
    /// </summary>
    public int StartY { get; }

    /// <summary>
    /// Gets the ending pageX value.
    /// </summary>
    public int EndX { get; }

    /// <summary>
    /// Gets the ending pageY value.
    /// </summary>
    public int EndY { get; }
}
