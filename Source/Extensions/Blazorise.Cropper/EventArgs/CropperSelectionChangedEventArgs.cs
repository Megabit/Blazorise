namespace Blazorise.Cropper;

/// <summary>
/// Defines the position and size data of the selection.
/// </summary>
public class CropperSelectionChangedEventArgs
{
    /// <summary>
    /// A default <see cref="CropperCroppedEventArgs"/> constructor.
    /// </summary>
    /// <param name="x">The x-axis coordinate of the selection.</param>
    /// <param name="y">The y-axis coordinate of the selection.</param>
    /// <param name="width">The width of the selection.</param>
    /// <param name="height">The height of the selection.</param>
    public CropperSelectionChangedEventArgs( int x, int y, int width, int height )
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the x-axis coordinate of the selection.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the y-axis coordinate of the selection.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets the width of the selection.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the selection.
    /// </summary>
    public int Height { get; }
}
