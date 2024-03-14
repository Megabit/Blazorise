namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="TabsContent"/> component.
/// </summary>
public record TabsContentState
{
    /// <summary>
    /// Gets or sets currently selected panel name.
    /// </summary>
    public string SelectedPanel { get; init; }

    /// <summary>
    /// Gets or sets the tabs rendering mode.
    /// </summary>
    public TabsRenderMode RenderMode { get; init; }
}