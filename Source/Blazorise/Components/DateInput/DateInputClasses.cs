namespace Blazorise;

/// <summary>
/// Component classes for <see cref="DateInput{TValue}"/>.
/// </summary>
public sealed record class DateInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="DateInput{TValue}"/>.
/// </summary>
public sealed record class DateInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}