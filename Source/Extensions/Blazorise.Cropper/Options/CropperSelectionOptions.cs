namespace Blazorise.Cropper;

/// <summary>
/// The Cropper selection interface provides properties for manipulating the layout and presentation.
/// </summary>
public record CropperSelectionOptions
{
    /// <summary>
    /// Indicates the aspect ratio of the selection, must a positive number.
    /// </summary>
    public CropperAspectRatio AspectRatio { get; init; } = CropperAspectRatio.Is1x1;

    /// <summary>
    /// Indicates the initial aspect ratio of the selection, must a positive number.
    /// </summary>
    public CropperAspectRatio InitialAspectRatio { get; init; } = CropperAspectRatio.Is1x1;

    /// <summary>
    /// Indicates the initial coverage of the selection, must a positive number between 0 (0%) and 1 (100%).
    /// </summary>
    public double? InitialCoverage { get; init; }

    /// <summary>
    /// Indicates whether the selection is movable.
    /// </summary>
    public bool Movable { get; init; } = true;

    /// <summary>
    /// Indicates whether the selection is resizable.
    /// </summary>
    public bool Resizable { get; init; } = true;

    /// <summary>
    /// Indicates whether the selection is zoomable.
    /// </summary>
    public bool Zoomable { get; init; } = true;

    /// <summary>
    /// Indicates whether keyboard control is supported.
    /// </summary>
    public bool Keyboard { get; init; } = true;

    /// <summary>
    /// Indicates whether show the outlined or not.
    /// </summary>
    public bool Outlined { get; init; } = true;
}