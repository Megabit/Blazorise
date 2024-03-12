namespace Blazorise.Cropper;

/// <summary>
/// Image Cropper crop options.
/// </summary>
public class CropperCropOptions
{
    /// <summary>
    /// The destination width of the output canvas.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The destination height  of the output canvas.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// A string indicating the image format. The default type is image/png; this image format will be also used if the specified type is not supported.
    /// </summary>
    public string ImageType { get; set; } = "image/png";

    /// <summary>
    /// A Number between 0 and 1 indicating the image quality to be used when creating images using file formats that support lossy compression (such as image/jpeg or image/webp).
    /// </summary>
    /// <remarks>
    /// A user agent will use its default quality value if this option is not specified, or if the number is outside the allowed range.
    /// </remarks>
    public double? ImageQuality { get; set; } = 1d;
}