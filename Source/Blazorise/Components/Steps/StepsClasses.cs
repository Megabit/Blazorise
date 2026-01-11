namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Steps"/>.
/// </summary>
public sealed record class StepsClasses : ComponentClasses
{
    /// <summary>
    /// Targets the content container element.
    /// </summary>
    public string Content { get; init; }
}

/// <summary>
/// Component styles for <see cref="Steps"/>.
/// </summary>
public sealed record class StepsStyles : ComponentStyles
{
    /// <summary>
    /// Targets the content container element.
    /// </summary>
    public string Content { get; init; }
}