namespace Blazorise.Ribbon;

/// <summary>
/// Shares the ribbon's current state with its descendants.
/// </summary>
public record RibbonState
{
    /// <summary>
    /// Identifies the tab whose command panel is selected.
    /// </summary>
    public string SelectedTab { get; init; }

    /// <summary>
    /// Indicates whether the command surface is hidden while the tab strip remains available.
    /// </summary>
    public bool Collapsed { get; init; }

    /// <summary>
    /// Determines whether descendant commands use the classic group layout or a compact single row.
    /// </summary>
    public RibbonDisplayMode DisplayMode { get; init; }

    /// <summary>
    /// Determines when tab panels create and retain their content.
    /// </summary>
    public TabsRenderMode RenderMode { get; init; }
}