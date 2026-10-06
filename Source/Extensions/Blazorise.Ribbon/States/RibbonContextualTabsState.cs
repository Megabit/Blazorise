namespace Blazorise.Ribbon;

/// <summary>
/// Shares a contextual group's identity, visibility, and accent with its tabs.
/// </summary>
internal record RibbonContextualTabsState
{
    /// <summary>
    /// Identifies the editing context that owns the tabs.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Specifies the shared contextual heading color.
    /// </summary>
    public Color Color { get; init; } = Blazorise.Color.Primary;

    /// <summary>
    /// Indicates whether the group's tabs are available in the ribbon.
    /// </summary>
    public bool IsActive { get; init; }
}