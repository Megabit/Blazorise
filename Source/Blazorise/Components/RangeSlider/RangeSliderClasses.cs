namespace Blazorise;

/// <summary>
/// Component classes for <see cref="RangeSlider{TValue}"/>.
/// </summary>
public sealed record class RangeSliderClasses : ComponentClasses
{
    /// <summary>
    /// Targets the inactive range track element.
    /// </summary>
    public string Track { get; init; }

    /// <summary>
    /// Targets the selected range track element.
    /// </summary>
    public string Range { get; init; }

    /// <summary>
    /// Targets both range inputs.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    /// Targets the start value range input.
    /// </summary>
    public string StartInput { get; init; }

    /// <summary>
    /// Targets the end value range input.
    /// </summary>
    public string EndInput { get; init; }

    /// <summary>
    /// Targets both tooltip elements.
    /// </summary>
    public string Tooltip { get; init; }
}

/// <summary>
/// Component styles for <see cref="RangeSlider{TValue}"/>.
/// </summary>
public sealed record class RangeSliderStyles : ComponentStyles
{
    /// <summary>
    /// Targets the inactive range track element.
    /// </summary>
    public string Track { get; init; }

    /// <summary>
    /// Targets the selected range track element.
    /// </summary>
    public string Range { get; init; }

    /// <summary>
    /// Targets both range inputs.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    /// Targets the start value range input.
    /// </summary>
    public string StartInput { get; init; }

    /// <summary>
    /// Targets the end value range input.
    /// </summary>
    public string EndInput { get; init; }

    /// <summary>
    /// Targets both tooltip elements.
    /// </summary>
    public string Tooltip { get; init; }
}
