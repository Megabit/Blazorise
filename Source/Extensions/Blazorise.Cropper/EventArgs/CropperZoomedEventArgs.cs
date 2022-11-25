namespace Blazorise.Cropper;

/// <summary>
/// Provides all the information for the cropper zoom event.
/// </summary>
public class CropperZoomedEventArgs
{
    /// <summary>
    /// A default <see cref="CropperZoomedEventArgs"/> constructor.
    /// </summary>
    /// <param name="scale">The scaling factor.</param>
    public CropperZoomedEventArgs( double scale )
    {
        Scale = scale;
    }

    /// <summary>
    /// Gets the scaling factor.
    /// </summary>
    public double Scale { get; }
}
