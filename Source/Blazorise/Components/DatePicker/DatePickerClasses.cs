namespace Blazorise;

/// <summary>
/// Component classes for <see cref="DatePicker{TValue}"/>.
/// </summary>
public sealed record class DatePickerClasses : ComponentClasses
{
    /// <summary>
    /// Targets the picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="DatePicker{TValue}"/>.
/// </summary>
public sealed record class DatePickerStyles : ComponentStyles
{
    /// <summary>
    /// Targets the picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}