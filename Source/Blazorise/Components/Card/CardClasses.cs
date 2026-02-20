namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Card"/>.
/// </summary>
public sealed record class CardClasses : ComponentClasses
{
    /// <summary>
    /// Targets wrapper elements around the card.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="Card"/>.
/// </summary>
public sealed record class CardStyles : ComponentStyles
{
    /// <summary>
    /// Targets wrapper elements around the card.
    /// </summary>
    public string Wrapper { get; init; }
}