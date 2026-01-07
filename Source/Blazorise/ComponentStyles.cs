namespace Blazorise;

/// <summary>
/// Base class for component-specific inline style definitions.
/// </summary>
public record class ComponentStyles
{
    /// <summary>
    /// Targets the element rendered by the component itself.
    /// </summary>
    public string Self { get; init; }
}