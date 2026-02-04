namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Check{TValue}"/>.
/// </summary>
public sealed record class CheckClasses : ComponentClasses
{
    /// <summary>
    /// Targets the check wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="Check{TValue}"/>.
/// </summary>
public sealed record class CheckStyles : ComponentStyles
{
    /// <summary>
    /// Targets the check wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}