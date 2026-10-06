namespace Blazorise.Ribbon;

/// <summary>
/// Shares backstage visibility, selection, and page rendering policy with its items.
/// </summary>
public record RibbonBackstageState
{
    /// <summary>
    /// Indicates whether backstage is currently shown.
    /// </summary>
    public bool Visible { get; init; }

    /// <summary>
    /// Identifies the selected backstage page.
    /// </summary>
    public string SelectedItem { get; init; }

    /// <summary>
    /// Determines when backstage pages create and retain their content.
    /// </summary>
    public TabsRenderMode RenderMode { get; init; }
}