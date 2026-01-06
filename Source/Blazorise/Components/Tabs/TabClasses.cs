namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Tab"/>.
/// </summary>
public sealed record class TabClasses : ComponentClasses
{
    /// <summary>
    /// Targets the link element.
    /// </summary>
    public string Link { get; init; }
}

/// <summary>
/// Component styles for <see cref="Tab"/>.
/// </summary>
public sealed record class TabStyles : ComponentStyles
{
    /// <summary>
    /// Targets the link element.
    /// </summary>
    public string Link { get; init; }
}