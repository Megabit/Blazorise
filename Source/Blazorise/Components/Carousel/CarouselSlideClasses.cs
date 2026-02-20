namespace Blazorise;

/// <summary>
/// Component classes for <see cref="CarouselSlide"/>.
/// </summary>
public sealed record class CarouselSlideClasses : ComponentClasses
{
    /// <summary>
    /// Targets the indicator element.
    /// </summary>
    public string Indicator { get; init; }
}

/// <summary>
/// Component styles for <see cref="CarouselSlide"/>.
/// </summary>
public sealed record class CarouselSlideStyles : ComponentStyles
{
    /// <summary>
    /// Targets the indicator element.
    /// </summary>
    public string Indicator { get; init; }
}