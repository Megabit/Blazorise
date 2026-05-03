namespace Blazorise.PivotGrid;

/// <summary>
/// Defines options for PivotGrid virtualized row rendering.
/// </summary>
public class PivotGridVirtualizeOptions
{
    /// <summary>
    /// Gets or sets the PivotGrid table container height when virtualization is enabled.
    /// </summary>
    public string Height { get; set; } = "500px";

    /// <summary>
    /// Gets or sets the PivotGrid table container maximum height when virtualization is enabled.
    /// </summary>
    public string MaxHeight { get; set; } = "500px";

    /// <summary>
    /// Gets or sets the expected virtualized item size in pixels.
    /// </summary>
    public float ItemSize { get; set; } = 50f;

    /// <summary>
    /// Gets or sets how many additional items will be rendered before and after the visible region.
    /// </summary>
    public int OverscanCount { get; set; } = 10;
}