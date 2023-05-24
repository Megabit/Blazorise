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
    public CropperSelectionChangedEventArgs( double x, double y, double width, double height )
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the x-axis coordinate of the selection.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Gets the y-axis coordinate of the selection.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Gets the width of the selection.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// Gets the height of the selection.
    /// </summary>
    public double Height { get; }
}
