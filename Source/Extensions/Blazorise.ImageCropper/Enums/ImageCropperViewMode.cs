namespace Blazorise.ImageCropper;

/// <summary>
/// Define the view mode of the cropper. If you set viewMode to 0, the crop box can extend outside the canvas,
/// while a value of 1, 2, or 3 will restrict the crop box to the size of the canvas. viewMode of 2 or 3 will
/// additionally restrict the canvas to the container. There is no difference between 2 and 3 when the proportions
/// of the canvas and the container are the same.
/// </summary>
public enum ImageCropperViewMode
{
    /// <summary>
    /// No restrictions
    /// </summary>
    Default = 0,

    /// <summary>
    /// Restrict the crop box not to exceed the size of the canvas.
    /// </summary>
    ExceedToCanvas = 1,

    /// <summary>
    /// Restrict the minimum canvas size to fit within the container. If the proportions of the canvas and the container differ,
    /// the minimum canvas will be surrounded by extra space in one of the dimensions.
    /// </summary>
    FitContainer = 2,

    /// <summary>
    /// Restrict the minimum canvas size to fill fit the container. If the proportions of the canvas and the container are different,
    /// the container will not be able to fit the whole canvas in one of the dimensions.
    /// </summary>
    FillFitContainer = 3
}