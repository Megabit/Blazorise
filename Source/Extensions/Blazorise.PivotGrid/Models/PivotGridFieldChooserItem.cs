namespace Blazorise.PivotGrid;

/// <summary>
/// Internal item used by the PivotGrid field chooser drag and drop surface.
/// </summary>
internal sealed class PivotGridFieldChooserItem : PivotGridFieldState
{
    /// <summary>
    /// Gets or sets the stable drag item key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets whether the item belongs to the available fields source list.
    /// </summary>
    public bool IsAvailableField { get; set; }
}