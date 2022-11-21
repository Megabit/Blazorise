namespace Blazorise.Cropper;

/// <summary>
/// Provides properties for manipulating the layout and presentation of selection grid elements.
/// </summary>
public record CropperGridOptions
{
    /// <summary>
    /// Indicates the number of the rows.
    /// </summary>
    public int Rows { get; init; } = 3;

    /// <summary>
    /// Indicates the number of the columns.
    /// </summary>
    public int Columns { get; init; } = 3;

    /// <summary>
    /// Indicates whether this element is bordered.
    /// </summary>
    public bool Bordered { get; init; } = true;

    /// <summary>
    /// Indicates whether this element covers its parent element.
    /// </summary>
    public bool Covered { get; init; } = true;
}