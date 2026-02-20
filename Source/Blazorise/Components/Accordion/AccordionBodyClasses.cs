namespace Blazorise;

/// <summary>
/// Component classes for <see cref="AccordionBody"/>.
/// </summary>
public sealed record class AccordionBodyClasses : ComponentClasses
{
    /// <summary>
    /// Targets the accordion body content element.
    /// </summary>
    public string Content { get; init; }
}

/// <summary>
/// Component styles for <see cref="AccordionBody"/>.
/// </summary>
public sealed record class AccordionBodyStyles : ComponentStyles
{
    /// <summary>
    /// Targets the accordion body content element.
    /// </summary>
    public string Content { get; init; }
}