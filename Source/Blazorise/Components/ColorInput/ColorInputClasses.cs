namespace Blazorise;

/// <summary>
/// Component classes for <see cref="ColorInput"/>.
/// </summary>
public sealed record class ColorInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="ColorInput"/>.
/// </summary>
public sealed record class ColorInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}