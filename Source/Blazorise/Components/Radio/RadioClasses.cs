namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Radio{TValue}"/>.
/// </summary>
public sealed record class RadioClasses : ComponentClasses
{
    /// <summary>
    /// Targets the radio wrapper element.
    /// </summary>
    public string Wrapper { get; init; }

    /// <summary>
    /// Targets the label rendered as a button.
    /// </summary>
    public string LabelButton { get; init; }
}

/// <summary>
/// Component styles for <see cref="Radio{TValue}"/>.
/// </summary>
public sealed record class RadioStyles : ComponentStyles
{
    /// <summary>
    /// Targets the radio wrapper element.
    /// </summary>
    public string Wrapper { get; init; }

    /// <summary>
    /// Targets the label rendered as a button.
    /// </summary>
    public string LabelButton { get; init; }
}