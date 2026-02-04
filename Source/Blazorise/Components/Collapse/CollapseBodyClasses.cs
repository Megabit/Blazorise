namespace Blazorise;

/// <summary>
/// Component classes for <see cref="CollapseBody"/>.
/// </summary>
public sealed record class CollapseBodyClasses : ComponentClasses
{
    /// <summary>
    /// Targets the collapse body content element.
    /// </summary>
    public string Content { get; init; }
}

/// <summary>
/// Component styles for <see cref="CollapseBody"/>.
/// </summary>
public sealed record class CollapseBodyStyles : ComponentStyles
{
    /// <summary>
    /// Targets the collapse body content element.
    /// </summary>
    public string Content { get; init; }
}