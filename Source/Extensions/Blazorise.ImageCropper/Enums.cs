namespace Blazorise.ImageCropper;

/// <summary>
/// Cropped Image Smoothing Quality
/// </summary>
public enum ImageSmoothingQuality
{
    /// <summary>
    /// Low quality.
    /// </summary>
    Low,
    /// <summary>
    /// Medium quality.
    /// </summary>
    Medium,
    /// <summary>
    /// High quality.
    /// </summary>
    High
}

/// <summary>
/// Define the view mode of the cropper. If you set viewMode to 0, the crop box can extend outside the canvas,
/// while a value of 1, 2, or 3 will restrict the crop box to the size of the canvas. viewMode of 2 or 3 will
/// additionally restrict the canvas to the container. There is no difference between 2 and 3 when the proportions
/// of the canvas and the container are the same.
/// </summary>
public enum ViewMode
{
    /// <summary>
    /// No restrictions
    /// </summary>
    Mode0 = 0,

    /// <summary>
    /// Restrict the crop box not to exceed the size of the canvas.
    /// </summary>
    Mode1 = 1,

    /// <summary>
    /// Restrict the minimum canvas size to fit within the container. If the proportions of the canvas and the container differ,
    /// the minimum canvas will be surrounded by extra space in one of the dimensions.
    /// </summary>
    Mode2 = 2,

    /// <summary>
    /// Restrict the minimum canvas size to fill fit the container. If the proportions of the canvas and the container are different,
    /// the container will not be able to fit the whole canvas in one of the dimensions.
    /// </summary>
    Mode3 = 3
}