namespace Blazorise.Cropper;

/// <summary>
/// Defines the behavior and initial shape of the crop selection.
/// </summary>
public record CropperSelectionOptions
{
    /// <summary>
    /// Fixed ratio maintained while resizing the selection.
    /// </summary>
    public CropperAspectRatio AspectRatio { get; init; } = CropperAspectRatio.Is1x1;

    /// <summary>
    /// Ratio used for the selection created during initialization.
    /// </summary>
    public CropperAspectRatio InitialAspectRatio { get; init; } = CropperAspectRatio.Is1x1;

    /// <summary>
    /// Fraction of the image covered by the initial selection, from <c>0</c> to <c>1</c>.
    /// </summary>
    public double? InitialCoverage { get; init; }

    /// <summary>
    /// The selection can be dragged to a new position.
    /// </summary>
    public bool Movable { get; init; } = true;

    /// <summary>
    /// Selection edges and handles can change its size.
    /// </summary>
    public bool Resizable { get; init; } = true;

    /// <summary>
    /// Mouse wheel or gesture zoom can be applied from the selection.
    /// </summary>
    public bool Zoomable { get; init; } = true;

    /// <summary>
    /// Keyboard input can move or resize the active selection.
    /// </summary>
    public bool Keyboard { get; init; } = true;

    /// <summary>
    /// The selection renders with a visible outline.
    /// </summary>
    public bool Outlined { get; init; } = true;
}