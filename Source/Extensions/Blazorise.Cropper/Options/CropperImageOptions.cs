namespace Blazorise.Cropper;

/// <summary>
/// Controls which image transform operations are available.
/// </summary>
public record CropperImageOptions
{
    /// <summary>
    /// Rotation commands can change the image angle.
    /// </summary>
    public bool Rotatable { get; init; } = true;

    /// <summary>
    /// Scale commands can resize the image on the X or Y axis.
    /// </summary>
    public bool Scalable { get; init; } = true;

    /// <summary>
    /// Skew commands can slant the image horizontally or vertically.
    /// </summary>
    public bool Skewable { get; init; }

    /// <summary>
    /// Move commands can reposition the image within the cropper.
    /// </summary>
    public bool Translatable { get; init; } = true;
}