namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Tabs"/>.
/// </summary>
public sealed record class TabsClasses : ComponentClasses
{
    /// <summary>
    /// Targets the content container element.
    /// </summary>
    public string Content { get; init; }
}

/// <summary>
/// Component styles for <see cref="Tabs"/>.
/// </summary>
public sealed record class TabsStyles : ComponentStyles
{
    /// <summary>
    /// Targets the content container element.
    /// </summary>
    public string Content { get; init; }
}