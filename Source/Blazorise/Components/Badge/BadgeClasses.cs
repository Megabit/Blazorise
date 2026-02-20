namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Badge"/>.
/// </summary>
public sealed record class BadgeClasses : ComponentClasses
{
    /// <summary>
    /// Targets the close action element.
    /// </summary>
    public string Close { get; init; }
}

/// <summary>
/// Component styles for <see cref="Badge"/>.
/// </summary>
public sealed record class BadgeStyles : ComponentStyles
{
    /// <summary>
    /// Targets the close action element.
    /// </summary>
    public string Close { get; init; }
}