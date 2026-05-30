namespace Blazorise.PivotGrid;

/// <summary>
/// Defines options for the built-in PivotGrid toolbar.
/// </summary>
public class PivotGridToolbarOptions
{
    /// <summary>
    /// Gets or sets whether the field chooser button is shown.
    /// </summary>
    public bool ShowFieldChooserButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the refresh button is shown.
    /// </summary>
    public bool ShowRefreshButton { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the expand all and collapse all buttons are shown.
    /// </summary>
    public bool ShowExpandCollapseButtons { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the reset layout button is shown.
    /// </summary>
    public bool ShowResetLayoutButton { get; set; } = true;
}