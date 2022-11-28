namespace Blazorise.Cropper;

/// <summary>
/// Provides properties for manipulating the layout and presentation of image elements.
/// </summary>
public record CropperImageOptions
{
    /// <summary>
    /// Indicates whether this element is rotatable.
    /// </summary>
    public bool Rotatable { get; init; } = true;

    /// <summary>
    /// Indicates whether this element is scalable.
    /// </summary>
    public bool Scalable { get; init; } = true;

    /// <summary>
    /// Indicates whether this element is skewable.
    /// </summary>
    public bool Skewable { get; init; }

    /// <summary>
    /// Indicates whether this element is translatable.
    /// </summary>
    public bool Translatable { get; init; } = true;
}