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

    /// <summary>
    /// Targets the step buttons container element.
    /// </summary>
    public string Buttons { get; init; }

    /// <summary>
    /// Targets the increment button element.
    /// </summary>
    public string ButtonUp { get; init; }

    /// <summary>
    /// Targets the decrement button element.
    /// </summary>
    public string ButtonDown { get; init; }
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

    /// <summary>
    /// Targets the step buttons container element.
    /// </summary>
    public string Buttons { get; init; }

    /// <summary>
    /// Targets the increment button element.
    /// </summary>
    public string ButtonUp { get; init; }

    /// <summary>
    /// Targets the decrement button element.
    /// </summary>
    public string ButtonDown { get; init; }
}