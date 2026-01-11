namespace Blazorise;

/// <summary>
/// Component classes for <see cref="FieldBody"/>.
/// </summary>
public sealed record class FieldBodyClasses : ComponentClasses
{
    /// <summary>
    /// Targets the body container element.
    /// </summary>
    public string Container { get; init; }
}

/// <summary>
/// Component styles for <see cref="FieldBody"/>.
/// </summary>
public sealed record class FieldBodyStyles : ComponentStyles
{
    /// <summary>
    /// Targets the body container element.
    /// </summary>
    public string Container { get; init; }
}