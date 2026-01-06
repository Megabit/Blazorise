namespace Blazorise;

/// <summary>
/// Component classes for <see cref="NumericPicker{TValue}"/>.
/// </summary>
public sealed record class NumericPickerClasses : ComponentClasses
{
    /// <summary>
    /// Targets the numeric picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="NumericPicker{TValue}"/>.
/// </summary>
public sealed record class NumericPickerStyles : ComponentStyles
{
    /// <summary>
    /// Targets the numeric picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}