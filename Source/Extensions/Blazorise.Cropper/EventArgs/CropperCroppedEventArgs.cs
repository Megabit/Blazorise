namespace Blazorise.Cropper;

/// <summary>
/// Provides all the information for the cropper crop event.
/// </summary>
public class CropperCroppedEventArgs
{
    /// <summary>
    /// A default <see cref="CropperCroppedEventArgs"/> constructor.
    /// </summary>
    /// <param name="startX">The starting pageX value.</param>
    /// <param name="startY">The starting pageY value.</param>
    /// <param name="endX">The ending pageX value.</param>
    /// <param name="endY">The ending pageY value.</param>
    public CropperCroppedEventArgs( double startX, double startY, double endX, double endY )
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
    }

    /// <summary>
    /// Gets the starting pageX value.
    /// </summary>
    public double StartX { get; }

    /// <summary>
    /// Gets the starting pageY value.
    /// </summary>
    public double StartY { get; }

    /// <summary>
    /// Gets the ending pageX value.
    /// </summary>
    public double EndX { get; }

    /// <summary>
    /// Gets the ending pageY value.
    /// </summary>
    public double EndY { get; }
}
