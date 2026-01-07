namespace Blazorise;

/// <summary>
/// Base class for component-specific inline style definitions.
/// </summary>
public record class ComponentStyles
{
    /// <summary>
    /// Targets the main element rendered by a component.
    /// </summary>
    public string Main { get; init; }
}