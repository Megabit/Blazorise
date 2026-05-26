namespace Blazorise.Cropper;

/// <summary>
/// Describes the canvas and image format created by a crop operation.
/// </summary>
public class CropperCropOptions
{
    /// <summary>
    /// Output canvas width in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Output canvas height in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// MIME type for the encoded cropped image. Defaults to <c>image/png</c>.
    /// </summary>
    public string ImageType { get; set; } = "image/png";

    /// <summary>
    /// Compression quality for lossy formats, from <c>0</c> to <c>1</c>.
    /// </summary>
    /// <remarks>
    /// A user agent will use its default quality value if this option is not specified, or if the number is outside the allowed range.
    /// </remarks>
    public double? ImageQuality { get; set; } = 1d;
}