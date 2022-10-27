namespace Blazorise.ImageCropper;

/// <summary>
/// Image Cropper crop options
/// </summary>
public class CropOptions
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
    /// The minimum destination width of the output canvas, the default value is 0.
    /// </summary>
    public int MinWidth { get; set; }
    /// <summary>
    /// The minimum destination height of the output canvas, the default value is 0.
    /// </summary>
    public int MinHeight { get; set; }
    /// <summary>
    /// The maximum destination width of the output canvas, the default value is Infinity.
    /// </summary>
    public int? MaxWidth { get; set; }
    /// <summary>
    /// The maximum destination height of the output canvas, the default value is Infinity.
    /// </summary>
    public int? MaxHeight { get; set; }
    /// <summary>
    /// A color to fill any alpha values in the output canvas, the default value is the transparent.
    /// </summary>
    public string FillColor { get; set; } = "transparent";
    /// <summary>
    /// Set to change if images are smoothed (true, default) or not (false).
    /// </summary>
    public bool ImageSmoothingEnabled { get; set; } = true;
    /// <summary>
    /// Set the quality of image smoothing, one of "low" (default), "medium", or "high".
    /// </summary>
    public ImageSmoothingQuality ImageSmoothingQuality { get; set; } = ImageSmoothingQuality.Low;
}