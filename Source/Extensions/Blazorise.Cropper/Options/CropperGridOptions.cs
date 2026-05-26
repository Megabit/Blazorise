namespace Blazorise.Cropper;

/// <summary>
/// Defines the grid overlay drawn inside the crop selection.
/// </summary>
public record CropperGridOptions
{
    /// <summary>
    /// Number of horizontal grid divisions.
    /// </summary>
    public int Rows { get; init; } = 3;

    /// <summary>
    /// Number of vertical grid divisions.
    /// </summary>
    public int Columns { get; init; } = 3;

    /// <summary>
    /// The grid renders border lines around its cells.
    /// </summary>
    public bool Bordered { get; init; } = true;

    /// <summary>
    /// The grid fills the full selection area.
    /// </summary>
    public bool Covered { get; init; } = true;
}