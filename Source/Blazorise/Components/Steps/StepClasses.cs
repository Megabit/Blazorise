namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Step"/>.
/// </summary>
public sealed record class StepClasses : ComponentClasses
{
    /// <summary>
    /// Targets the step content container element.
    /// </summary>
    public string Container { get; init; }

    /// <summary>
    /// Targets the marker element.
    /// </summary>
    public string Marker { get; init; }

    /// <summary>
    /// Targets the description element.
    /// </summary>
    public string Description { get; init; }
}

/// <summary>
/// Component styles for <see cref="Step"/>.
/// </summary>
public sealed record class StepStyles : ComponentStyles
{
    /// <summary>
    /// Targets the step content container element.
    /// </summary>
    public string Container { get; init; }

    /// <summary>
    /// Targets the marker element.
    /// </summary>
    public string Marker { get; init; }

    /// <summary>
    /// Targets the description element.
    /// </summary>
    public string Description { get; init; }
}