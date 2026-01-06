namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Slider{TValue}"/>.
/// </summary>
public sealed record class SliderClasses : ComponentClasses
{
    /// <summary>
    /// Targets the slider wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="Slider{TValue}"/>.
/// </summary>
public sealed record class SliderStyles : ComponentStyles
{
    /// <summary>
    /// Targets the slider wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}