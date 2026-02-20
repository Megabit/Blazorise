namespace Blazorise;

/// <summary>
/// Component classes for <see cref="TimeInput{TValue}"/>.
/// </summary>
public sealed record class TimeInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="TimeInput{TValue}"/>.
/// </summary>
public sealed record class TimeInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the input wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}