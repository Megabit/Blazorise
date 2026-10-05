namespace Blazorise.Ribbon;

/// <summary>
/// Shares backstage visibility, selection, and page rendering policy with its items.
/// </summary>
public record RibbonBackstageState
{
    /// <summary>
    /// Gets whether backstage is visible.
    /// </summary>
    public bool Visible { get; init; }

    /// <summary>
    /// Gets the selected page name.
    /// </summary>
    public string SelectedItem { get; init; }

    /// <summary>
    /// Gets the page rendering policy.
    /// </summary>
    public TabsRenderMode RenderMode { get; init; }
}