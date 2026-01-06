namespace Blazorise;

/// <summary>
/// Base class for component-specific class name definitions.
/// </summary>
public abstract record class ComponentClasses
{
    /// <summary>
    /// Targets the main element rendered by a component.
    /// </summary>
    public string Main { get; init; }
}