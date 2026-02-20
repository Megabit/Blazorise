namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Layout"/>.
/// </summary>
public sealed record class LayoutClasses : ComponentClasses
{
    /// <summary>
    /// Targets the loading overlay element.
    /// </summary>
    public string Loading { get; init; }
}

/// <summary>
/// Component styles for <see cref="Layout"/>.
/// </summary>
public sealed record class LayoutStyles : ComponentStyles
{
    /// <summary>
    /// Targets the loading overlay element.
    /// </summary>
    public string Loading { get; init; }
}