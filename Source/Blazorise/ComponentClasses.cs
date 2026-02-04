namespace Blazorise;

/// <summary>
/// Base class for component-specific class name definitions.
/// </summary>
public record class ComponentClasses
{
    /// <summary>
    /// Targets the element rendered by the component itself.
    /// </summary>
    public string Self { get; init; }
}