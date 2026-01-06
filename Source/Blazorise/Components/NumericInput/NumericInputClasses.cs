namespace Blazorise;

/// <summary>
/// Component classes for <see cref="NumericInput{TValue}"/>.
/// </summary>
public sealed record class NumericInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="NumericInput{TValue}"/>.
/// </summary>
public sealed record class NumericInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}