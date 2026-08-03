namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="ContextMenu"/> component.
/// </summary>
public record ContextMenuState
{
    /// <summary>
    /// Gets a value indicating whether the context menu is visible.
    /// </summary>
    public bool Visible { get; init; }

    /// <summary>
    /// Gets the viewport client X coordinate, or <see langword="null"/> when anchored to a target.
    /// </summary>
    public double? ClientX { get; init; }

    /// <summary>
    /// Gets the viewport client Y coordinate, or <see langword="null"/> when anchored to a target.
    /// </summary>
    public double? ClientY { get; init; }
}