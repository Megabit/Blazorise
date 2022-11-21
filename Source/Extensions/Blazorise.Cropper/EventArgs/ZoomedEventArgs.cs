namespace Blazorise.Cropper;

/// <summary>
/// Provides all the information for the cropper zoom event.
/// </summary>
public class ZoomedEventArgs
{
    /// <summary>
    /// A default <see cref="ZoomedEventArgs"/> constructor.
    /// </summary>
    /// <param name="scale">The scaling factor.</param>
    public ZoomedEventArgs( double scale )
    {
        Scale = scale;
    }

    /// <summary>
    /// Gets the scaling factor.
    /// </summary>
    public double Scale { get; }
}
