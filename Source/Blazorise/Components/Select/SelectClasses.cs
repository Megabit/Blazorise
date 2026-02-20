namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Select{TValue}"/>.
/// </summary>
public sealed record class SelectClasses : ComponentClasses
{
    /// <summary>
    /// Targets the select wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="Select{TValue}"/>.
/// </summary>
public sealed record class SelectStyles : ComponentStyles
{
    /// <summary>
    /// Targets the select wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}