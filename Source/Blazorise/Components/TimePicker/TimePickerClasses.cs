namespace Blazorise;

/// <summary>
/// Component classes for <see cref="TimePicker{TValue}"/>.
/// </summary>
public sealed record class TimePickerClasses : ComponentClasses
{
    /// <summary>
    /// Targets the picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="TimePicker{TValue}"/>.
/// </summary>
public sealed record class TimePickerStyles : ComponentStyles
{
    /// <summary>
    /// Targets the picker wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}