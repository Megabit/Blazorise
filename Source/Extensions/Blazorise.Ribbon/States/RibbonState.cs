namespace Blazorise.Ribbon;

/// <summary>
/// Shares the ribbon's current state with its descendants.
/// </summary>
public record RibbonState
{
    /// <summary>
    /// Gets the selected tab name.
    /// </summary>
    public string SelectedTab { get; init; }

    /// <summary>
    /// Gets whether the command surface is collapsed.
    /// </summary>
    public bool Collapsed { get; init; }

    /// <summary>
    /// Gets the command surface layout.
    /// </summary>
    public RibbonDisplayMode DisplayMode { get; init; }

    /// <summary>
    /// Gets the panel content rendering policy.
    /// </summary>
    public TabsRenderMode RenderMode { get; init; }
}