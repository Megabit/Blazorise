namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Carousel"/>.
/// </summary>
public sealed record class CarouselClasses : ComponentClasses
{
    /// <summary>
    /// Targets the indicators container element.
    /// </summary>
    public string Indicators { get; init; }

    /// <summary>
    /// Targets the slides container element.
    /// </summary>
    public string Slides { get; init; }
}

/// <summary>
/// Component styles for <see cref="Carousel"/>.
/// </summary>
public sealed record class CarouselStyles : ComponentStyles
{
    /// <summary>
    /// Targets the indicators container element.
    /// </summary>
    public string Indicators { get; init; }

    /// <summary>
    /// Targets the slides container element.
    /// </summary>
    public string Slides { get; init; }
}