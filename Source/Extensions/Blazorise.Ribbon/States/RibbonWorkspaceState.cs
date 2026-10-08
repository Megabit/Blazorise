namespace Blazorise.Ribbon;

/// <summary>
/// Shares the backstage association and visibility within a ribbon workspace.
/// </summary>
public record RibbonWorkspaceState
{
    /// <summary>
    /// Indicates whether backstage is shown.
    /// </summary>
    public bool BackstageVisible { get; init; }

    /// <summary>
    /// Identifies the registered backstage surface for accessibility associations.
    /// </summary>
    public string BackstageElementId { get; init; }
}